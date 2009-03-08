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

''' <summary>
''' Encapsulates enumerations and methods that deal with the Racal key table.
''' </summary>
''' <remarks>
''' This class can be used to parse/validate a key table code and to determine if
''' a key function such as generation, import or export is allowed given a 
''' specific key type.
''' </remarks>
Public Class KeyTypeTable

    ''' <summary>
    ''' Key function.
    ''' </summary>
    ''' <remarks>
    ''' Enumerates the Racal key functions.
    ''' </remarks>
    Public Enum KeyFunction
        ''' <summary>
        ''' Key generation.
        ''' </summary>
        ''' <remarks>
        ''' Designates a key generation.
        ''' </remarks>
        Generate = 0

        ''' <summary>
        ''' Key import.
        ''' </summary>
        ''' <remarks>
        ''' Designates a key import.
        ''' </remarks>
        Import = 1

        ''' <summary>
        ''' Key export.
        ''' </summary>
        ''' <remarks>
        ''' Designates a key export.
        ''' </remarks>
        Export = 2
    End Enum

    ''' <summary>
    ''' Enumerates requirements for key functions.
    ''' </summary>
    ''' <remarks>
    ''' Enumerates requirements for key functions.
    ''' </remarks>
    Public Enum AuthorizedStateRequirement
        ''' <summary>
        ''' Not allowed.
        ''' </summary>
        ''' <remarks>
        ''' Designates that a key function is not allowed.
        ''' </remarks>
        NotAllowed = 0

        ''' <summary>
        ''' Allowed only in authorized state.
        ''' </summary>
        ''' <remarks>
        ''' Designates that a key function is allowed only if the HSM is in the authorized state.
        ''' </remarks>
        NeedsAuthorizedState = 1

        ''' <summary>
        ''' Allowed unconditionally.
        ''' </summary>
        ''' <remarks>
        ''' Designates that a key function is allowed independantly on the authorized state.
        ''' </remarks>
        DoesNotNeedAuthorizedState = 2
    End Enum

    Private Shared Reqs() As AuthStateReqs = {New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair04_05, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair06_07, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair14_15, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair16_17, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair22_23, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair26_27, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair28_29, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair30_31, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair04_05, "1", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair28_29, "1", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair04_05, "2", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair28_29, "2", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair28_29, "3", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair14_15, "4", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair28_29, "4", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Generate, LMKPairs.LMKPair.Pair28_29, "5", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair06_07, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair14_15, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair16_17, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair22_23, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair26_27, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair28_29, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair30_31, "0", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair04_05, "1", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair28_29, "1", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair04_05, "2", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair28_29, "2", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair28_29, "3", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair14_15, "4", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair28_29, "4", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Import, LMKPairs.LMKPair.Pair28_29, "5", AuthorizedStateRequirement.DoesNotNeedAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair06_07, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair14_15, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair16_17, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair22_23, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair26_27, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair28_29, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair30_31, "0", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair04_05, "1", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair28_29, "1", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair04_05, "2", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair28_29, "2", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair28_29, "3", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair14_15, "4", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair28_29, "4", AuthorizedStateRequirement.NeedsAuthorizedState), _
                                              New AuthStateReqs(KeyFunction.Export, LMKPairs.LMKPair.Pair28_29, "5", AuthorizedStateRequirement.NeedsAuthorizedState)}

    ''' <summary>
    ''' Parses a key type code.
    ''' </summary>
    ''' <remarks>
    ''' This method parses a key type code according to the Racal key type table.
    ''' </remarks>
    Public Shared Sub ParseKeyTypeCode(ByVal keyTypeCode As String, _
                                       ByRef LMKKeyPair As LMKPairs.LMKPair, _
                                       ByRef [Variant] As String)

        If keyTypeCode Is Nothing OrElse keyTypeCode.Length <> 3 Then Throw New Exceptions.XInvalidKeyType("Invalid key type")

        Dim lmkpair As String, var As String
        var = keyTypeCode.Substring(0, 1)
        lmkpair = keyTypeCode.Substring(1, 2)

        Try
            If Convert.ToInt32(var) < 0 OrElse Convert.ToInt32(var) > 9 Then
                Throw New Exceptions.XInvalidKeyType("Invalid Variant in key type (" + var + ")")
            End If
        Catch ex As Exception
            Throw New Exceptions.XInvalidKeyType("Invalid Variant in key type (" + var + ")")
        End Try

        [Variant] = var

        Select Case lmkpair
            Case "00"
                LMKKeyPair = LMKPairs.LMKPair.Pair04_05
            Case "01"
                LMKKeyPair = LMKPairs.LMKPair.Pair06_07
            Case "02"
                LMKKeyPair = LMKPairs.LMKPair.Pair14_15
            Case "03"
                LMKKeyPair = LMKPairs.LMKPair.Pair16_17
            Case "04"
                LMKKeyPair = LMKPairs.LMKPair.Pair18_19
            Case "05"
                LMKKeyPair = LMKPairs.LMKPair.Pair20_21
            Case "06"
                LMKKeyPair = LMKPairs.LMKPair.Pair22_23
            Case "07"
                LMKKeyPair = LMKPairs.LMKPair.Pair24_25
            Case "08"
                LMKKeyPair = LMKPairs.LMKPair.Pair26_27
            Case "09"
                LMKKeyPair = LMKPairs.LMKPair.Pair28_29
            Case "0A"
                LMKKeyPair = LMKPairs.LMKPair.Pair30_31
            Case "0B"
                LMKKeyPair = LMKPairs.LMKPair.Pair32_33
            Case "0C"
                LMKKeyPair = LMKPairs.LMKPair.Pair34_35
            Case "0D"
                LMKKeyPair = LMKPairs.LMKPair.Pair36_37
            Case "0E"
                LMKKeyPair = LMKPairs.LMKPair.Pair38_39
            Case Else
                Throw New Exceptions.XInvalidKeyType("Invalid Variant in key type (" + var + ")")
        End Select

    End Sub

    ''' <summary>
    ''' Determines if a key function is allowed.
    ''' </summary>
    ''' <remarks>
    ''' Determines if a key function is allowed and whether the authorized
    ''' state is required for the specified function.
    ''' </remarks>
    Public Shared Function GetAuthorizedStateRequirement(ByVal NeededFunction As KeyFunction, _
                                                         ByVal LMKKeyPair As LMKPairs.LMKPair, _
                                                         ByVal [Variant] As String) As AuthorizedStateRequirement

        For i As Integer = 0 To Reqs.GetUpperBound(0)
            If Reqs(i).Func = NeededFunction AndAlso Reqs(i).LMKKeyPair = LMKKeyPair AndAlso Reqs(i).var = [Variant] Then
                Return Reqs(i).Requirement
            End If
        Next

        Return AuthorizedStateRequirement.NotAllowed

    End Function

    Private Class AuthStateReqs

        Public Func As KeyFunction
        Public LMKKeyPair As LMKPairs.LMKPair
        Public var As String
        Public Requirement As AuthorizedStateRequirement

        Public Sub New(ByVal Func As KeyFunction, ByVal LMKKeyPair As LMKPairs.LMKPair, ByVal [Variant] As String, ByVal Req As AuthorizedStateRequirement)
            Me.Func = Func
            Me.LMKKeyPair = LMKKeyPair
            Me.var = [Variant]
            Me.Requirement = Req
        End Sub

    End Class

End Class
