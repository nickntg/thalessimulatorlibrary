''
'' This program is free software; you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation; either version 2 of the License, or
'' (at your option) any later version.
''
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
''
'' You should have received a copy of the GNU General Public License
'' along with this program; if not, write to the Free Software
'' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'' 

Namespace Message.XML

    ''' <summary>
    ''' This class parses a message using a list of message field definitions.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MessageParser

        ''' <summary>
        ''' Parses a message based on message field definitions.
        ''' </summary>
        ''' <param name="msg">Message to parse.</param>
        ''' <param name="fields">Instance of message field definitions to use.</param>
        ''' <param name="KVPairs">Key/Value pairs with parsed values.</param>
        ''' <param name="result">Resulting Thales error code. 00 is returned
        ''' if parsing was successful and no errors were encountered.</param>
        ''' <remarks></remarks>
        Public Shared Sub Parse(ByVal msg As Message, ByVal fields As MessageFields, _
                                ByRef KVPairs As XML.MessageKeyValuePairs, ByRef result As String)

            'We don't want to skip any field.
            For Each fld As MessageField In fields.Fields
                fld.Skip = False
            Next

            Dim fldIdx As Integer = 0
            While fldIdx <= fields.Fields.Count - 1
                Dim fld As MessageField = fields.Fields(fldIdx)

                Dim repetitions As Integer = 1
                If fld.Repetitions <> "" Then
                    'Number or field?
                    If IsNumeric(fld.Repetitions) Then
                        repetitions = Convert.ToInt32(fld.Repetitions)
                    Else
                        repetitions = Convert.ToInt32(KVPairs.Item(fld.Repetitions))
                    End If

                    'Do we do static repetitions?
                    If fld.StaticRepetitions Then
                        '
                        ' Yes.
                        '
                        ' This is what we must do for static repetitions:
                        '
                        ' 1. Scan ahead and see if subsequent fields also require static repetitions.
                        '    If so, we'll treat all fields as a group.
                        ' 2. Save the aforementioned field group and remove them from the fields.
                        ' 3. Insert the aforementioned field group in the fields for the number
                        '    of repetitions. Ensure that the dynamically inserted fields do not
                        '    require any number of repetitions or static repetitions.
                        ' 4. Let the parsing continue as usual.
                        '

                        'Look ahead.
                        Dim nextNonStaticRepField As Integer = fldIdx + 1
                        While nextNonStaticRepField <= fields.Fields.Count - 1 AndAlso fields.Fields(nextNonStaticRepField).StaticRepetitions = True
                            nextNonStaticRepField += 1
                        End While

                        'Save the fields.
                        Dim dynamicFields As New List(Of MessageField)
                        For i As Integer = fldIdx To nextNonStaticRepField - 1
                            dynamicFields.Add(fields.Fields(i))
                        Next

                        'Remove them from the original fields list.
                        For i As Integer = fldIdx To nextNonStaticRepField - 1
                            fields.Fields.RemoveAt(fldIdx)
                        Next

                        'Insert them for all repetitions.
                        Dim insertPos As Integer = fldIdx
                        Dim fieldList As New List(Of String)
                        For i As Integer = 1 To repetitions
                            For j As Integer = 0 To dynamicFields.Count - 1
                                Dim newFld As MessageField = dynamicFields(j).Clone
                                newFld.Repetitions = ""
                                newFld.StaticRepetitions = False

                                'Save the ORIGINAL field name.
                                If fieldList.Contains(newFld.Name) = False Then fieldList.Add(newFld.Name)

                                'Alter the field name to signify the repetition number.
                                newFld.Name = newFld.Name + " #" + i.ToString

                                'If a field is dependent upon a field that is repeated, make sure
                                'to correct the dependency.
                                If fieldList.Contains(newFld.DependentField) Then
                                    newFld.DependentField = newFld.DependentField + " #" + i.ToString
                                End If

                                'Do the same as above for the dynamic length.
                                If fieldList.Contains(newFld.DynamicLength) Then
                                    newFld.DynamicLength = newFld.DynamicLength + " #" + i.ToString
                                End If

                                'Add the field.
                                fields.Fields.Insert(insertPos, newFld)
                                insertPos += 1
                            Next
                        Next

                        'We've dynamically inserted the required fields, hence no repetition is required.
                        repetitions = 1

                        'We update the field because it has changed.
                        fld = fields.Fields(fldIdx)
                    End If
                End If

                For j As Integer = 1 To repetitions
                    ' Criteria to process field.
                    ' 1. If we should not skip this field...
                    ' 2. If there is a dependent field which has already been processed
                    ' 3. If the value of the dependent field matches what we expect...
                    ' 4. If there is no dependent field...
                    ' 5. If the dependent field has not been processed and we don't have 
                    '    a dependent value...
                    '
                    ' ((1) AND (2) AND (3)) OR (4) OR (5).
                    '
                    ' (1) ==> Several fields may depend upon one dependent field. When one of
                    '         these fields is parse, we don't want to parse the others. Therefore,
                    '         when one field with a dependent field is parsed, we mark all other
                    '         fields that depend on the dependent field with the Skip=True flag.
                    ' (2) ==> If this field depends on another, we want to try and parse it only
                    '         if the dependent field has been already parsed.
                    ' (3) ==> If this field depends on another field's presence and value, we
                    '         want to parse the field only if the above conditions are met.
                    ' (4) ==> Self-explanatory.
                    ' (5) ==> The current field depends on the presence of another field which has
                    '         not been parsed, but not its value.
                    If ((fld.Skip = False) AndAlso _
                       (fld.DependentField <> "" AndAlso KVPairs.ContainsKey(fld.DependentField)) AndAlso _
                       (fld.DependentValue.Count = 0 OrElse fld.DependentValue.Contains(KVPairs.Item(fld.DependentField)))) OrElse _
                       (fld.DependentField = "") OrElse _
                       (fld.DependentField <> "" AndAlso KVPairs.ContainsKey(fld.DependentField) = False AndAlso fld.DependentValue.Count = 0) Then
                        Dim val As String = ""

                        If fld.SkipUntilValid Then
                            Try
                                'If we're supposed to skip until we encounter a valid value, keep on reading.
                                Do
                                    val = msg.MessageData.Substring(msg.CurrentIndex, fld.Length)
                                    If fld.ValidValues.Contains(val) Then
                                        Exit Do
                                    Else
                                        'We advance by one only!
                                        msg.AdvanceIndex(1)
                                    End If
                                Loop Until fld.ValidValues.Contains(val)
                            Catch ex As ArgumentOutOfRangeException
                                If fld.AllowNotFoundValid Then
                                    val = ""
                                Else
                                    Throw ex
                                End If
                            End Try
                        ElseIf fld.ParseUntilValue <> "" Then
                            'Parse until a specific value is found. Note that we're looking
                            'for a single-character value only.
                            Dim tempVal As String = ""
                            Do
                                val = msg.MessageData.Substring(msg.CurrentIndex, 1)
                                If fld.ParseUntilValue = val Then
                                    msg.DecreaseIndex(1)
                                    Exit Do
                                Else
                                    tempVal += val
                                    msg.AdvanceIndex(1)
                                End If
                            Loop
                            val = tempVal
                        Else
                            'Else, read the current field.

                            'Get the dynamic length, if it's appropriate.
                            If fld.DynamicLength <> "" Then
                                'Find out if the dynamic field is numeric or otherwise and perform the necessary conversion.
                                For Each scannedFld As XML.MessageField In fields.Fields
                                    If scannedFld.Name = fld.DynamicLength Then
                                        If scannedFld.MessageFieldType = MessageFieldTypes.Hexadecimal Then
                                            fld.Length = Convert.ToInt32(KVPairs.Item(fld.DynamicLength), 16)
                                        Else
                                            fld.Length = Convert.ToInt32(KVPairs.Item(fld.DynamicLength))
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If

                            If fld.Length <> 0 Then
                                'Normal read.
                                If (fld.MessageFieldType <> MessageFieldTypes.Binary) Then
                                    val = msg.MessageData.Substring(msg.CurrentIndex, fld.Length)
                                Else
                                    'For binary, we expect 2 hex characters.
                                    val = msg.MessageData.Substring(msg.CurrentIndex, fld.Length * 2)
                                End If
                            Else
                                'Read the rest of the message.
                                val = msg.MessageData.Substring(msg.CurrentIndex, msg.CharsLeft)
                            End If
                        End If

                        If fld.OptionValues.Count = 0 OrElse fld.OptionValues.Contains(val) Then

                            Try
                                'Check valid values.
                                If fld.ValidValues.Count > 0 AndAlso fld.ValidValues.Contains(val) = False Then
                                    Log.Logger.MinorDebug(String.Format("Invalid value detected for field [{0}].", fld.Name))
                                    Log.Logger.MinorDebug(String.Format("Received [{0}] but can be one of [{1}]. ", val, GetCommaSeparetedListWithValues(fld.ValidValues)))
                                    Throw New Exception(String.Format("Invalid value [{0}] for field [{1}].", val, fld.Name))
                                End If

                                'Check format.
                                Select Case fld.MessageFieldType
                                    Case MessageFieldTypes.Hexadecimal, MessageFieldTypes.Binary
                                        If Utility.IsHexString(val) = False Then
                                            Log.Logger.MinorDebug(String.Format("Invalid value detected for field [{0}].", fld.Name))
                                            Log.Logger.MinorDebug(String.Format("Received [{0}] but expected a hexadecimal value.", val))
                                            Throw New Exception(String.Format("Invalid value [{0}] for field [{1}].", val, fld.Name))
                                        End If
                                    Case MessageFieldTypes.Numeric
                                        If IsNumeric(val) = False Then
                                            Log.Logger.MinorDebug(String.Format("Invalid value detected for field [{0}].", fld.Name))
                                            Log.Logger.MinorDebug(String.Format("Received [{0}] but expected a numeric value.", val))
                                            Throw New Exception(String.Format("Invalid value [{0}] for field [{1}].", val, fld.Name))
                                        End If
                                End Select
                            Catch ex As Exception
                                'If a rejection code is specified, use it.
                                If fld.RejectionCode <> "" Then
                                    result = fld.RejectionCode
                                    Exit Sub
                                Else
                                    'Otherwise, just throw an exception.
                                    Throw ex
                                End If
                            End Try

                            'Log.Logger.MinorDebug(String.Format("[{0}]=[{1}]", fld.Name, val))

                            'Add this value.
                            If repetitions = 1 Then
                                KVPairs.Add(fld.Name, val)
                            Else
                                KVPairs.Add(fld.Name + " #" + j.ToString, val)
                            End If

                            'Advance the message index.
                            If fld.MessageFieldType <> MessageFieldTypes.Binary Then
                                msg.AdvanceIndex(fld.Length)
                            Else
                                msg.AdvanceIndex(fld.Length * 2)
                            End If

                            'Mark this field as parsed if the repetitions are done.
                            If j = repetitions Then fld.Skip = True

                            'If there were a dependent field, then mark all other
                            'fields that had the same dependency so we won't try to parse them.
                            If fld.DependentField <> "" Then
                                For z As Integer = fldIdx + 1 To fields.Fields.Count - 1
                                    If fields.Fields(z).DependentField = fld.DependentField AndAlso fields.Fields(z).ExclusiveDependency Then
                                        fields.Fields(z).Skip = True
                                    End If
                                Next
                            End If
                        End If
                    End If

                    If msg.CharsLeft = 0 Then Exit For
                Next

                If msg.CharsLeft = 0 Then Exit While

                fldIdx += 1
            End While

            result = ErrorCodes.ER_00_NO_ERROR
        End Sub

        ''' <summary>
        ''' Return a comma-separated string which contains all values of a list of strings.
        ''' </summary>
        ''' <param name="lst">List of strings.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetCommaSeparetedListWithValues(ByVal lst As List(Of String)) As String
            Dim s As String = ""
            For i As Integer = 0 To lst.Count - 1
                If i < lst.Count - 1 Then
                    s = s + lst.Item(i) + ","
                Else
                    s = s + lst.Item(i)
                End If
            Next
            Return s
        End Function

    End Class

End Namespace