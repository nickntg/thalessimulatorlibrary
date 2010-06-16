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
    ''' This class provides a placeholder for a list of message field definitions.
    ''' It also provides a shared method that reads the list of message field definitions.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MessageFields

        Private m_fields As New List(Of MessageField)

        ''' <summary>
        ''' Get/set the list of field definitions.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Fields() As List(Of MessageField)
            Get
                Return m_fields
            End Get
        End Property

        Private m_isDynamic As Boolean = False

        ''' <summary>
        ''' Determines whether this field collection can dynamically change.
        ''' This can happen when fields exist that have a dynamic length that
        ''' corresponds to an internal variable.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsDynamic() As Boolean
            Get
                Return m_isDynamic
            End Get
            Set(ByVal value As Boolean)
                m_isDynamic = value
            End Set
        End Property


        ''' <summary>
        ''' Read a list of field definitions from an XML file and return an instance
        ''' of this class.
        ''' </summary>
        ''' <param name="xmlFile">Full path name of file to read.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ReadXMLFields(ByVal xmlFile As String) As MessageFields
            Return RecursiveReadXMLFields(Convert.ToString(Resources.GetResource(Resources.HOST_COMMANDS_XML_DEFS)) + xmlFile)
        End Function

        'These methods do not serve any useful purpose, but where used to troubleshoot Mono problems.
        ' ''Private Shared Sub DumpToFile(ByVal ds As DataSet)
        ' ''    ds.WriteXmlSchema("schema.txt")
        ' ''    Using SW As IO.StreamWriter = New IO.StreamWriter("dataset.txt")
        ' ''        For Each t As DataTable In ds.Tables

        ' ''            Try
        ' ''                SW.WriteLine(t.TableName)

        ' ''                For Each c As DataColumn In t.Columns
        ' ''                    SW.WriteLine(c.ColumnName)
        ' ''                Next

        ' ''                For Each r As DataRow In t.Columns
        ' ''                    Dim line As String = ""
        ' ''                    For i As Integer = 0 To r.ItemArray.GetUpperBound(0)
        ' ''                        line = line + SON(r.Item(i)) + ","
        ' ''                    Next
        ' ''                    SW.WriteLine(line)
        ' ''                Next
        ' ''            Catch ex As Exception

        ' ''            End Try
        ' ''        Next
        ' ''    End Using
        ' ''End Sub

        ' ''Private Shared Function SON(ByVal o As Object) As String
        ' ''    If o IsNot Nothing AndAlso IsDBNull(o) = False Then
        ' ''        Return Convert.ToString(o)
        ' ''    Else
        ' ''        Return ""
        ' ''    End If
        ' ''End Function

        ''' <summary>
        ''' Recursive version of ReadXMLFields.
        ''' </summary>
        ''' <param name="xmlFile">Full path name of file to read.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function RecursiveReadXMLFields(ByVal xmlFile As String) As MessageFields
            'Initialize an instance.
            Dim fields As New MessageFields

            Using ds As DataSet = New DataSet
                'Read a dataset from the XML file.
                ds.ReadXml(xmlFile)

                'Process all fields.
                For Each dr As DataRow In ds.Tables("Field").Rows
                    Dim fld As New MessageField

                    'Get the name.
                    fld.Name = Convert.ToString(dr.Item("Name"))

                    'Get the dynamic length.
                    If ContainsNonNullColumn(dr, "DynamicFieldLength") Then fld.DynamicLength = Convert.ToString(dr.Item("DynamicFieldLength"))

                    'Get value until which parsing will continue.
                    If ContainsNonNullColumn(dr, "ParseUntilValue") Then fld.ParseUntilValue = Convert.ToString(dr.Item("ParseUntilValue"))

                    'Get dependent field and dependent value.
                    If ContainsNonNullColumn(dr, "DependentField") Then fld.DependentField = Convert.ToString(dr.Item("DependentField"))
                    If ContainsNonNullColumn(dr, "DependentValue") Then fld.SetDependentValue(Convert.ToString(dr.Item("DependentValue")))

                    'Get exclusive dependency flag.
                    If ContainsNonNullColumn(dr, "ExclusiveDependency") Then fld.ExclusiveDependency = Convert.ToBoolean(dr.Item("ExclusiveDependency"))

                    'Get rejection code.
                    If ContainsNonNullColumn(dr, "RejectionCodeIfInvalid") Then fld.RejectionCode = Convert.ToString(dr.Item("RejectionCodeIfInvalid"))

                    'Get the number of repetitions.
                    If ContainsNonNullColumn(dr, "Repetitions") Then fld.Repetitions = Convert.ToString(dr.Item("Repetitions"))

                    'Get the nature of repetitions.
                    If ContainsNonNullColumn(dr, "StaticRepetitions") Then fld.StaticRepetitions = Convert.ToBoolean(dr.Item("StaticRepetitions"))

                    'Get whether we'll skip until a valid value.
                    If ContainsNonNullColumn(dr, "SkipUntilValid") Then fld.SkipUntilValid = Convert.ToBoolean(dr.Item("SkipUntilValid"))

                    'OptionValue and ValidValue will appear as elements if there is only one instance
                    'in the XML file, so we parse them here as well.
                    If ContainsNonNullColumn(dr, "OptionValue") Then fld.OptionValues.Add(Convert.ToString(dr.Item("OptionValue")))
                    If ContainsNonNullColumn(dr, "ValidValue") Then fld.ValidValues.Add(Convert.ToString(dr.Item("ValidValue")))

                    If ContainsNonNullColumn(dr, "field_id") Then
                        'There are other related tables, so we'll check them for option and valid values.
                        Dim id As Integer = Convert.ToInt32(dr.Item("field_id"))

                        If ds.Tables("OptionValue") IsNot Nothing Then
                            For Each drOption As DataRow In ds.Tables("OptionValue").Select("field_id=" + id.ToString)
                                Try
                                    fld.OptionValues.Add(Convert.ToString(drOption.Item("OptionValue_Text")))
                                Catch ex As Exception
                                    'Under Mono, OptionValue_Text appears as OptionValue_Column.
                                    fld.OptionValues.Add(Convert.ToString(drOption.Item("OptionValue_Column")))
                                End Try
                            Next
                        End If

                        If ds.Tables("ValidValue") IsNot Nothing Then
                            For Each drValid As DataRow In ds.Tables("ValidValue").Select("field_id=" + id.ToString)
                                Try
                                    fld.ValidValues.Add(Convert.ToString(drValid.Item("ValidValue_Text")))
                                Catch ex As Exception
                                    'Under Mono, ValidValue_Text appears as ValidValue_Column.
                                    fld.ValidValues.Add(Convert.ToString(drValid.Item("ValidValue_Column")))
                                End Try
                            Next
                        End If
                    End If

                    If ContainsNonNullColumn(dr, "IncludeFile") Then
                        'If there is an include file, we need to get into that as well.
                        'We assume that the file resides in the same directory.
                        Dim FI As New IO.FileInfo(xmlFile)
                        Dim includeFields As New MessageFields
                        includeFields = RecursiveReadXMLFields(Utility.AppendDirectorySeparator(FI.Directory.FullName) + Convert.ToString(dr.Item("IncludeFile")))

                        'The include file was parsed. Add all field definitions
                        'of the include file to our current field definitions.
                        For Each inclFld As MessageField In includeFields.Fields
                            'Take care to replace the #replace# tag with the field name.
                            inclFld.Name = inclFld.Name.Replace("#replace#", fld.Name)

                            'Do the same replacement for the dependent field.
                            If Not String.IsNullOrEmpty(inclFld.DependentField) Then
                                inclFld.DependentField = inclFld.DependentField.Replace("#replace#", fld.Name)
                            End If

                            '...and the dynamic length field.
                            If Not String.IsNullOrEmpty(inclFld.DynamicLength) Then
                                inclFld.DynamicLength = inclFld.DynamicLength.Replace("#replace#", fld.Name)
                            End If

                            'Option and valid values of the current field are appended.
                            inclFld.OptionValues.AddRange(fld.OptionValues)
                            inclFld.ValidValues.AddRange(fld.ValidValues)

                            'Note that if there are dependent field values for fld but not for inclFld
                            'we copy those from fld to inclFld as well.
                            If String.IsNullOrEmpty(fld.DependentField) = False AndAlso String.IsNullOrEmpty(inclFld.DependentField) Then
                                inclFld.DependentField = fld.DependentField
                                inclFld.DependentValue = fld.DependentValue
                                inclFld.ExclusiveDependency = fld.ExclusiveDependency
                            End If

                            'The same is true for the repetitions.
                            If String.IsNullOrEmpty(fld.Repetitions) = False AndAlso String.IsNullOrEmpty(inclFld.Repetitions) Then
                                inclFld.Repetitions = fld.Repetitions
                                inclFld.StaticRepetitions = fld.StaticRepetitions
                            End If

                            fields.Fields.Add(inclFld)
                        Next
                    Else
                        'Get length.
                        Dim len As String = Convert.ToString(dr.Item("Length"))
                        If IsNumeric(len) Then
                            'When we want to parse unti we find a value, we set a length
                            'of 1. This is because we want to look for a value of a 
                            'single-character.
                            If fld.ParseUntilValue = "" Then
                                fld.Length = Convert.ToInt32(len)
                            Else
                                fld.Length = 1
                            End If

                        Else
                            Select Case len
                                Case Resources.DOUBLE_LENGTH_ZMKS
                                    fields.IsDynamic = True
                                    If Convert.ToBoolean(Resources.GetResource(Resources.DOUBLE_LENGTH_ZMKS)) = True Then
                                        fld.Length = 32
                                    Else
                                        fld.Length = 16
                                    End If
                                Case Resources.CLEAR_PIN_LENGTH
                                    fields.IsDynamic = True
                                    fld.Length = Convert.ToInt32(Resources.GetResource(Resources.CLEAR_PIN_LENGTH)) + 1
                                Case Else
                                    Throw New Exceptions.XInvalidConfiguration(String.Format("Invalid length element [{0}]", len))
                            End Select
                        End If

                        'Get field type.
                        fld.MessageFieldType = CType([Enum].Parse(GetType(MessageFieldTypes), Convert.ToString(dr.Item("Type"))), MessageFieldTypes)

                        'Add this field.
                        fields.Fields.Add(fld)
                    End If
                Next

                Return fields
            End Using

        End Function

        ''' <summary>
        ''' Determine if a column exist in a data row and, if it does, whether it contains a value or not.
        ''' </summary>
        ''' <param name="dr">Data row to check.</param>
        ''' <param name="columnName">Column name.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function ContainsNonNullColumn(ByVal dr As DataRow, ByVal columnName As String) As Boolean
            Try
                Return Not IsDBNull(dr.Item(columnName))
            Catch ex As ArgumentException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Returns a copy of this instance.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clone() As MessageFields
            Dim o As New MessageFields
            For Each field As MessageField In m_fields
                o.Fields.Add(field.Clone)
            Next
            o.IsDynamic = Me.IsDynamic
            Return o
        End Function

    End Class

End Namespace