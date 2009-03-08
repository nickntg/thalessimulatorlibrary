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
''' Placeholder class for various utility methods.
''' </summary>
''' <remarks>
''' Placeholder class for various utility methods.
''' </remarks>
Public Class Utility

    Private Shared rndMachine As Random = New Random

    ''' <summary>
    ''' Defines a parity structure.
    ''' </summary>
    ''' <remarks>
    ''' Defines a parity structure.
    ''' </remarks>
    Public Enum ParityCheck
        ''' <summary>
        ''' Odd parity.
        ''' </summary>
        ''' <remarks>
        ''' Odd parity.
        ''' </remarks>
        OddParity = 0

        ''' <summary>
        ''' Even parity.
        ''' </summary>
        ''' <remarks>
        ''' Even parity.
        ''' </remarks>
        EvenParity = 1

        ''' <summary>
        ''' No parity.
        ''' </summary>
        ''' <remarks>
        ''' No parity.
        ''' </remarks>
        NoParity = 2
    End Enum

    ''' <summary>
    ''' Converts a hexadecimal string to a byte array.
    ''' </summary>
    ''' <remarks>
    ''' Converts a hexadecimal string to a byte array.
    ''' </remarks>
    Public Shared Sub HexStringToByteArray(ByVal s As String, ByRef bData() As Byte)

        Dim i As Integer, j As Integer

        i = 0
        j = 0
        While i <= s.Length - 1
            bData(j) = Convert.ToByte(s.Substring(i, 2), 16)
            i += 2
            j += 1
        End While

    End Sub

    ''' <summary>
    ''' Converts a byte array to a hexadecimal string.
    ''' </summary>
    ''' <remarks>
    ''' Converts a byte array to a hexadecimal string.
    ''' </remarks>
    Public Shared Sub ByteArrayToHexString(ByVal bData() As Byte, ByRef s As String)

        Dim i As Integer, sb As New Text.StringBuilder

        For i = 0 To bData.GetUpperBound(0)
            sb.AppendFormat("{0:X2}", bData(i))
        Next
        s = sb.ToString
        sb = Nothing

    End Sub

    ''' <summary>
    ''' Creates a random hexadecimal single length key.
    ''' </summary>
    ''' <remarks>
    ''' Creates a random hexadecimal single length key. If <b>EnforceParity</b> is False
    ''' or <b>Parity</b> is set to no parity, the key is not changed to enforce any bit parity.
    ''' </remarks>
    Public Shared Function RandomKey(ByVal EnforceParity As Boolean, ByVal Parity As ParityCheck) As String
        Dim i As Integer
        Dim s As String, sb As New Text.StringBuilder

        For i = 1 To 16
            sb.AppendFormat("{0:X2}", rndMachine.Next(0, 16))
        Next
        s = sb.ToString
        sb = Nothing

        If EnforceParity = True Then
            If Parity <> ParityCheck.NoParity Then
                s = MakeParity(s, Parity)
            End If
        End If

        Return s

    End Function

    ''' <summary>
    ''' Determines if a string is comprised of hexadecimal characters.
    ''' </summary>
    ''' <remarks>
    ''' Determines if a string is comprised of hexadecimal characters.
    ''' </remarks>
    Public Shared Function IsHexString(ByVal s As String) As Boolean

        If s Is Nothing OrElse s = "" Then Return False

        s = RemoveKeyType(s)

        Dim i As Integer
        Dim bFound As Boolean

        s = s.ToUpper
        i = 0
        bFound = False
        While (i < s.Length) And (bFound = False)
            If Not Char.IsDigit(s.Chars(i)) Then
                If (s.Chars(i) < "A") Or (s.Chars(i) > "F") Then
                    bFound = True
                    Exit While
                End If
            End If
            i += 1
        End While

        IsHexString = Not bFound

    End Function

    ''' <summary>
    ''' Checks the parity of a hexadecimal string.
    ''' </summary>
    ''' <remarks>
    ''' Checks the parity of a hexadecimal string. If no parity is specified,
    ''' the method returns True.
    ''' </remarks>
    Public Shared Function IsParityOK(ByVal hexString As String, ByVal parity As ParityCheck) As Boolean

        If parity = ParityCheck.NoParity Then Return True

        hexString = RemoveKeyType(hexString)

        Dim i As Integer = 0
        While i < hexString.Length
            Dim b As String = toBinary(hexString.Substring(i, 2))
            i += 2
            Dim l As Integer = 0
            For j As Integer = 0 To b.Length - 1
                If b.Substring(j, 1) = "1" Then l += 1
            Next
            If ((l Mod 2 = 0) AndAlso (parity = ParityCheck.OddParity)) OrElse _
               ((l Mod 2 = 1) AndAlso (parity = ParityCheck.EvenParity)) Then
                Return False
            End If

        End While
        Return True

    End Function

    ''' <summary>
    ''' Changes a hexadecimal string to achieve bit parity.
    ''' </summary>
    ''' <remarks>
    ''' Changes the 8th bit of hex words in order to conform to the specified parity.
    ''' If no parity is specified, the input is return unchanged.
    ''' </remarks>
    Public Shared Function MakeParity(ByVal hexString As String, ByVal parity As ParityCheck) As String

        If parity = ParityCheck.NoParity Then Return hexString

        Dim head As String = ""
        If hexString <> RemoveKeyType(hexString) Then
            head = hexString.Substring(0, 1)
            hexString = RemoveKeyType(hexString)
        End If

        Dim i As Integer = 0, r As String = ""

        While i < hexString.Length
            Dim b As String = toBinary(hexString.Substring(i, 2))
            i += 2
            Dim l As Integer = b.Replace("0", "").Length

            If ((l Mod 2 = 0) AndAlso (parity = ParityCheck.OddParity)) OrElse _
               ((l Mod 2 = 1) AndAlso (parity = ParityCheck.EvenParity)) Then
                If b.Substring(7, 1) = "1" Then
                    r = r + b.Substring(0, 7) + "0"
                Else
                    r = r + b.Substring(0, 7) + "1"
                End If
            Else
                r = r + b
            End If
        End While

        Return head + fromBinary(r)

    End Function

    ''' <summary>
    ''' Performs an XOR operation on two hexadecimal strings.
    ''' </summary>
    ''' <remarks>
    ''' Performs an XOR operation on two hexadecimal strings. Both strings are assumed
    ''' to be 16 characters long.
    ''' </remarks>
    Public Shared Function XORHexStrings(ByVal s1 As String, ByVal s2 As String) As String
        Return XORHexStringsFull(s1.Substring(0, 16), s2.Substring(0, 16))
    End Function

    ''' <summary>
    ''' Performs an XOR operation on two hexadecimal strings.
    ''' </summary>
    ''' <remarks>
    ''' Performs an XOR operation on two hexadecimal strings. XOR operation continues
    ''' up to the length of the first string parameter.
    ''' </remarks>
    Public Shared Function XORHexStringsFull(ByVal s1 As String, ByVal s2 As String) As String
        Dim i As Integer
        Dim s As String

        s = ""
        s1 = RemoveKeyType(s1)
        s2 = RemoveKeyType(s2)
        For i = 0 To s1.Length - 1
            s = s + Hex$(CLng("&H" + s1.Substring(i, 1)) Xor CLng("&H" + s2.Substring(i, 1)))
        Next

        Return s
    End Function

    ''' <summary>
    ''' Converts a hexadecimal string to binary.
    ''' </summary>
    ''' <remarks>
    ''' Converts a hexadecimal string to binary.
    ''' </remarks>
    Public Shared Function toBinary(ByVal hexString As String) As String
        Dim r As String = ""
        For i As Integer = 0 To hexString.Length - 1
            r = r + Convert.ToString(CInt("&H" + hexString.Substring(i, 1)), 2).PadLeft(4, "0"c)
        Next
        Return r
    End Function

    ''' <summary>
    ''' Converts a binary string to hexadecimal.
    ''' </summary>
    ''' <remarks>
    ''' Converts a binary string to hexadecimal.
    ''' </remarks>
    Public Shared Function fromBinary(ByVal binaryString As String) As String
        Dim r As String = ""
        For i As Integer = 0 To binaryString.Length - 1 Step 4
            r = r + Convert.ToByte(binaryString.Substring(i, 4), 2).ToString("X1")
        Next
        Return r
    End Function

    ''' <summary>
    ''' Removes a key type code from a hex string.
    ''' </summary>
    ''' <remarks>
    ''' Removes a key type code from a hex string.
    ''' </remarks>
    Public Shared Function RemoveKeyType(ByVal keyString As String) As String
        If keyString Is Nothing OrElse keyString = "" Then Return keyString
        If keyString.StartsWith(Core.KeySchemeTable.GetKeySchemeValue(KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi)) OrElse _
           keyString.StartsWith(Core.KeySchemeTable.GetKeySchemeValue(KeySchemeTable.KeyScheme.DoubleLengthKeyVariant)) OrElse _
           keyString.StartsWith(Core.KeySchemeTable.GetKeySchemeValue(KeySchemeTable.KeyScheme.SingleDESKey)) OrElse _
           keyString.StartsWith(Core.KeySchemeTable.GetKeySchemeValue(KeySchemeTable.KeyScheme.TripleLengthKeyAnsi)) OrElse _
           keyString.StartsWith(Core.KeySchemeTable.GetKeySchemeValue(KeySchemeTable.KeyScheme.TripleLengthKeyVariant)) Then
            Return keyString.Substring(1)
        Else
            Return keyString
        End If
    End Function

End Class
