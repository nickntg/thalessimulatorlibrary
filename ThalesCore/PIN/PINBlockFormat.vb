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

Namespace PIN

    ''' <summary>
    ''' Class to facilitate PIN block handling.
    ''' </summary>
    ''' <remarks>
    ''' Objects of this class can be used to construct a PIN block and determine a
    ''' PIN from a PIN block.
    ''' </remarks>
    Public Class PINBlockFormat

        ''' <summary>
        ''' This enumeration abstracts PIN block formats.
        ''' </summary>
        ''' <remarks>
        ''' This enumeration abstracts PIN block formats.
        ''' </remarks>
        Public Enum PIN_Block_Format
            ''' <summary>
            ''' ANSI X9.8 format, code 01.
            ''' </summary>
            ''' <remarks>
            ''' ANSI X9.8 format, code 01.
            ''' </remarks>
            AnsiX98 = 1
            ''' <summary>
            ''' Docutel format, code 02.
            ''' </summary>
            ''' <remarks>
            ''' Docutel format, code 02.
            ''' </remarks>
            Docutel = 2
            ''' <summary>
            ''' Diebold format, code 03.
            ''' </summary>
            ''' <remarks>
            ''' Diebold format, code 03.
            ''' </remarks>
            Diebold = 3
            ''' <summary>
            ''' PLUS network format, code 04.
            ''' </summary>
            ''' <remarks>
            ''' PLUS network format, code 04.
            ''' </remarks>
            Plus = 4
            ''' <summary>
            ''' ISO 9564-1 Format 1, code 05.
            ''' </summary>
            ''' <remarks>
            ''' ISO 9564-1 Format 1, code 05.
            ''' </remarks>
            ISO9564_1 = 5
            ''' <summary>
            ''' Invalid PIN Block code.
            ''' </summary>
            ''' <remarks>
            ''' Invalid PIN Block code.
            ''' </remarks>
            InvalidPBCode = 9999
        End Enum

        ''' <summary>
        ''' Parses a text PIN Block Format code.
        ''' </summary>
        ''' <remarks>
        ''' Parses a text PIN Block Format code.
        ''' </remarks>
        Public Shared Function ToPINBlockFormat(ByVal PBFormat As String) As PIN_Block_Format
            Select Case PBFormat
                Case "01"
                    Return PIN_Block_Format.AnsiX98
                Case "02"
                    Return PIN_Block_Format.Docutel
                Case "03"
                    Return PIN_Block_Format.Diebold
                Case "04"
                    Return PIN_Block_Format.Plus
                Case "05"
                    Return PIN_Block_Format.ISO9564_1
                Case Else
                    Return PIN_Block_Format.InvalidPBCode
            End Select
        End Function

        ''' <summary>
        ''' Creates a text PIN Block Format code.
        ''' </summary>
        ''' <remarks>
        ''' Creates a text PIN Block Format code.
        ''' </remarks>
        Public Shared Function FromPINBlockFormat(ByVal PBFormat As PIN_Block_Format) As String
            Select Case PBFormat
                Case PIN_Block_Format.AnsiX98
                    Return "01"
                Case PIN_Block_Format.Diebold
                    Return "03"
                Case PIN_Block_Format.Docutel
                    Return "02"
                Case PIN_Block_Format.Plus
                    Return "04"
                Case PIN_Block_Format.ISO9564_1
                    Return "05"
                Case Else
                    Return ""
            End Select
        End Function

        ''' <summary>
        ''' This method constructs a clear PIN block.
        ''' </summary>
        ''' <remarks>
        ''' This method constructs a clear PIN block.
        ''' </remarks>
        Public Shared Function ToPINBlock(ByVal ClearPIN As String, ByVal AccountNumber_Or_PaddingString As String, _
                                          ByVal Format As PIN_Block_Format) As String

            Select Case Format
                Case PIN_Block_Format.AnsiX98
                    If AccountNumber_Or_PaddingString.Length < 12 Then Throw New Exceptions.XInvalidAccount("Account length must be equal or greater to 12")
                    Dim s1 As String = (ClearPIN.Length.ToString().PadLeft(2, "0"c) + ClearPIN).PadRight(16, "F"c)
                    Dim s2 As String = AccountNumber_Or_PaddingString.PadLeft(16, "0"c)
                    Return Core.Utility.XORHexStrings(s1, s2)
                Case PIN_Block_Format.Diebold
                    Return ClearPIN.PadRight(16, "F"c)
                Case PIN_Block_Format.Docutel
                    If ClearPIN.Length > 6 Then Throw New Exceptions.XInvalidPINLength("PIN length must be less or equal to 6")
                    Dim s1 As String = ClearPIN.Length.ToString() + ClearPIN.PadLeft(6, "0"c)
                    Return s1 + AccountNumber_Or_PaddingString.Substring(0, 16 - s1.Length)
                Case PIN_Block_Format.ISO9564_1
                    Dim s1 As String = ("0" + ClearPIN.Length.ToString() + ClearPIN).PadLeft(16, "F"c)
                    Dim s2 As String = "0000" + AccountNumber_Or_PaddingString.Substring(0, 12)
                    Return Core.Utility.XORHexStrings(s1, s2)
                Case PIN_Block_Format.Plus
                    Throw New Exceptions.XUnsupportedPINBlockFormat("Unsupported PIN block format PLUS")
                Case Else
                    Throw New Exceptions.XUnsupportedPINBlockFormat("Unsupported PIN block format " + Format.ToString())
            End Select

        End Function

        ''' <summary>
        ''' This method determines a PIN from a PIN block.
        ''' </summary>
        ''' <remarks>
        ''' This method determines a PIN from a clear PIN block.
        ''' </remarks>
        Public Shared Function ToPIN(ByVal PINBlock As String, ByVal AccountNumber_Or_PaddingString As String, _
                                     ByVal Format As PIN_Block_Format) As String

            Select Case Format
                Case PIN_Block_Format.AnsiX98
                    Dim s2 As String = AccountNumber_Or_PaddingString.PadLeft(16, "0"c)
                    Dim s1 As String = Core.Utility.XORHexStrings(s2, PINBlock)
                    Dim PINLength As Integer = Convert.ToInt32(s1.Substring(0, 2))
                    Return s1.Substring(2, PINLength)
                Case PIN_Block_Format.Diebold
                    Return PINBlock.Replace("F", "")
                Case PIN_Block_Format.Docutel
                    Dim PINLength As Integer = Convert.ToInt32(PINBlock.Substring(0, 1))
                    Return PINBlock.Substring(1, PINLength)
                Case PIN_Block_Format.ISO9564_1
                    Dim s2 As String = "0000" + AccountNumber_Or_PaddingString.Substring(0, 12)
                    Dim s1 As String = Core.Utility.XORHexStrings(s2, PINBlock)
                    Dim PINLength As Integer = Convert.ToInt32(s1.Substring(1, 1))
                    Return s1.Substring(2, PINLength)
                Case PIN_Block_Format.Plus
                    Throw New Exceptions.XUnsupportedPINBlockFormat("Unsupported PIN block format PLUS")
                Case Else
                    Throw New Exceptions.XUnsupportedPINBlockFormat("Unsupported PIN block format " + Format.ToString())
            End Select

        End Function

    End Class

End Namespace
