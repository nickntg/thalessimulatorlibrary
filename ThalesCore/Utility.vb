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

Imports ThalesSim.Core.Cryptography

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

        Dim i As Integer = 0, j As Integer = 0

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

        Dim sb As New Text.StringBuilder

        For i As Integer = 0 To bData.GetUpperBound(0)
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
        Dim s As String, sb As New Text.StringBuilder

        For i As Integer = 1 To 16
            sb.AppendFormat("{0:X1}", rndMachine.Next(0, 16))
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
        If String.IsNullOrEmpty(s) Then Return False

        s = RemoveKeyType(s).ToUpper

        For i As Integer = 0 To s.Length - 1
            If Not Char.IsDigit(s.Chars(i)) Then
                If (s.Chars(i) < "A"c) Or (s.Chars(i) > "F"c) Then
                    Return False
                End If
            End If

        Next

        Return True
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
        Dim s As String = ""

        s1 = RemoveKeyType(s1)
        s2 = RemoveKeyType(s2)
        For i As Integer = 0 To s1.Length - 1
            s = s + (Convert.ToInt32(s1.Substring(i, 1), 16) Xor Convert.ToInt32(s2.Substring(i, 1), 16)).ToString("X")
        Next

        Return s
    End Function

    ''' <summary>
    ''' Performs an OR operation on two hexadecimal strings with an offset.
    ''' </summary>
    ''' <remarks>
    ''' Performs an OR operation on two hexadecimal strings. OR operation continues
    ''' up to the length of the first string parameter.
    ''' </remarks>
    Public Shared Function ORHexStringsFull(ByVal b As String, ByVal mask As String, ByVal offset As Integer) As String
        Dim l As Integer
        Dim i As Integer
        Dim s As String = b
        Dim z(s.Length) As Char
        z = s.ToCharArray()
        l = Math.Min(b.Length - offset, mask.Length)
        For i = 0 To l - 1
            z(offset) = Convert.ToChar(String.Format("{0:X}", Convert.ToInt32(s.Substring(offset, 1), 16) Or Convert.ToInt32(mask.Substring(i, 1), 16)))
            offset += 1
        Next
        s = z
        Return s
    End Function

    ''' <summary>
    ''' Performs a Shift Right operation on a hexadecimal string.
    ''' </summary>
    ''' <remarks>
    ''' Performs a Shift Right operation on a hexadecimal string.
    ''' </remarks>
    Public Shared Function SHRHexString(ByRef b As String) As String
        Dim r As Integer = Convert.ToInt32(b, 16)
        r = r >> 1
        Dim s As String = b

        s = String.Format("{0:X}", r).PadLeft(b.Length, "0"c)
        Return s
    End Function

    ''' <summary>
    ''' Performs an AND operation on two hexadecimal strings.
    ''' </summary>
    ''' <remarks>
    ''' Performs an AND operation on two hexadecimal strings. OR operation continues
    ''' up to the length of the first string parameter.
    ''' </remarks>
    Public Shared Function ANDHexStrings(ByVal b As String, ByVal mask As String) As String
        Return ANDHexStringsOffset(b, mask, 0)
    End Function

    ''' <summary>
    ''' Performs an AND operation on two hexadecimal strings with an offset.
    ''' </summary>
    ''' <remarks>
    ''' Performs an AND operation on two hexadecimal strings. OR operation continues
    ''' up to the length of the first string parameter.
    ''' </remarks>
    Public Shared Function ANDHexStringsOffset(ByVal b As String, ByVal mask As String, ByVal offset As Integer) As String
        Dim l As Integer
        Dim s As String = b
        Dim z(s.Length) As Char

        z = s.ToCharArray()
        l = Math.Min(b.Length - offset, mask.Length)

        For i As Integer = 0 To l - 1
            z(offset) = Convert.ToChar(String.Format("{0:X}", (Convert.ToInt32(s.Substring(offset, 1), 16) And Convert.ToInt32(mask.Substring(i, 1), 16))))
            offset += 1
        Next
        s = z
        Return s
    End Function

    ''' <summary>
    ''' Performs check for any non-zero in a byte array.
    ''' </summary>
    ''' <remarks>
    ''' Performs check for any non-zero in a byte array
    ''' </remarks>
    Private Shared Function arrayNotZero(ByVal b() As Byte) As Boolean
        For i As Integer = 0 To b.Length - 1
            If b(i) <> 0 Then
                Return True
            End If
        Next
        Return False
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
            r = r + Convert.ToString(Convert.ToInt32(hexString.Substring(i, 1), 16), 2).PadLeft(4, "0"c)
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

    Public Shared Function AddNoCarry(ByVal str1 As String, ByVal str2 As String) As String
        Dim output As String = ""
        For cnt As Integer = 0 To str1.Length - 1
            output = output + Convert.ToString((Convert.ToInt32(str1.Substring(cnt, 1)) + Convert.ToInt32(str2.Substring(cnt, 1))) Mod 10)
        Next
        Return output
    End Function

    Public Shared Function SubtractNoBorrow(ByVal str1 As String, ByVal str2 As String) As String
        Dim output As String = ""
        Dim i As Integer
        For cnt As Integer = 0 To str1.Length - 1
            i = Convert.ToInt32(str1.Substring(cnt, 1)) - Convert.ToInt32(str2.Substring(cnt, 1))
            '    i = Asc(str1.Chars(cnt)) - Asc(str2.Chars(cnt))
            If i < 0 Then
                i = 10 + i
            End If
            output = output + Convert.ToString(i)
        Next
        Return output
    End Function

    ''' <summary>
    ''' Decimalises a string.
    ''' </summary>
    ''' <remarks>
    ''' Decimalises a string.
    ''' </remarks>
    Public Shared Function Decimalise(ByVal undecimalisedString As String, ByVal decimalisationTable As String) As String
        Dim output As String = ""
        For cnt As Integer = 0 To undecimalisedString.Length - 1
            Dim ch As Char = undecimalisedString.Chars(cnt)
            If ch >= "0"c And ch <= "9"c Then
                output = output + ch
            Else
                Dim rep_index As Integer = (System.Text.ASCIIEncoding.Default.GetBytes(ch)(0) - 65) + 10
                output = output + decimalisationTable.Chars(rep_index)
            End If
        Next
        Return output
    End Function

    ''' <summary>
    ''' Removes a key type code from a hex string.
    ''' </summary>
    ''' <remarks>
    ''' Removes a key type code from a hex string.
    ''' </remarks>
    Public Shared Function RemoveKeyType(ByVal keyString As String) As String
        If String.IsNullOrEmpty(keyString) Then Return keyString
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

    ''' <summary>
    ''' Returns a random hex key.
    ''' </summary>
    ''' <remarks>
    ''' Creates a single, double or triple length random hex key.
    ''' </remarks>
    Public Shared Function CreateRandomKey(ByVal ks As KeySchemeTable.KeyScheme) As String
        Select Case ks
            Case KeySchemeTable.KeyScheme.SingleDESKey
                Return Utility.RandomKey(True, Utility.ParityCheck.OddParity)
            Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant
                Return Utility.MakeParity(Utility.RandomKey(False, Utility.ParityCheck.OddParity) + Utility.RandomKey(False, Utility.ParityCheck.OddParity), Utility.ParityCheck.OddParity)
            Case KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                Return Utility.MakeParity(Utility.RandomKey(False, Utility.ParityCheck.OddParity) + Utility.RandomKey(False, Utility.ParityCheck.OddParity) + Utility.RandomKey(False, Utility.ParityCheck.OddParity), Utility.ParityCheck.OddParity)
            Case Else
                Throw New InvalidOperationException("Invalid key scheme [" + ks.ToString + "]")
        End Select
    End Function

    ''' <summary>
    ''' Encrypts a key under an LMK pair and a variant.
    ''' </summary>
    ''' <remarks>
    ''' This method encrypts a key under an LMK pair.
    ''' </remarks>
    Public Shared Function EncryptUnderLMK(ByVal clearKey As String, _
                                           ByVal Target_KeyScheme As KeySchemeTable.KeyScheme, _
                                           ByVal LMKKeyPair As LMKPairs.LMKPair, _
                                           ByVal variantNumber As String) As String

        Dim result As String = ""

        Select Case Target_KeyScheme
            Case KeySchemeTable.KeyScheme.SingleDESKey, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.Unspecified
                result = TripleDES.TripleDESEncrypt(New HexKey(LMK.LMKStorage.LMKVariant(LMKKeyPair, Convert.ToInt32(variantNumber))), clearKey)
            Case KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                result = TripleDES.TripleDESEncryptVariant(New HexKey(LMK.LMKStorage.LMKVariant(LMKKeyPair, Convert.ToInt32(variantNumber))), clearKey)
        End Select

        Select Case Target_KeyScheme
            Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                result = KeySchemeTable.GetKeySchemeValue(Target_KeyScheme) + result
        End Select

        Return result

    End Function

    ''' <summary>
    ''' Decrypts a key encrypted under an LMK pair and a variant.
    ''' </summary>
    ''' <remarks>
    ''' This method decrypts a key encrypted under an LMK pair and a variant.
    ''' </remarks>
    Public Shared Function DecryptUnderLMK(ByVal encryptedKey As String, _
                                           ByVal Target_KeyScheme As KeySchemeTable.KeyScheme, _
                                           ByVal LMKKeyPair As LMKPairs.LMKPair, _
                                           ByVal variantNumber As String) As String

        Dim result As String = ""

        Select Case Target_KeyScheme
            Case KeySchemeTable.KeyScheme.SingleDESKey, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi
                result = TripleDES.TripleDESDecrypt(New HexKey(LMK.LMKStorage.LMKVariant(LMKKeyPair, Convert.ToInt32(variantNumber))), encryptedKey)
            Case KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                result = TripleDES.TripleDESDecryptVariant(New HexKey(LMK.LMKStorage.LMKVariant(LMKKeyPair, Convert.ToInt32(variantNumber))), encryptedKey)
        End Select

        Select Case Target_KeyScheme
            Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                result = KeySchemeTable.GetKeySchemeValue(Target_KeyScheme) + result
        End Select

        Return result

    End Function

    ''' <summary>
    ''' Decrypts a ZMK encrypted under LMK pair 04-05 and a variant.
    ''' </summary>
    ''' <remarks>
    ''' Decrypts a ZMK encrypted under LMK pair 04-05 and a variant.
    ''' </remarks>
    Public Shared Function DecryptZMKEncryptedUnderLMK(ByVal encryptedZMK As String, ByVal ks As Core.KeySchemeTable.KeyScheme, ByVal var As Integer) As String

        Select Case ks
            Case Core.KeySchemeTable.KeyScheme.SingleDESKey, Core.KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, Core.KeySchemeTable.KeyScheme.TripleLengthKeyAnsi
                Return TripleDES.TripleDESDecrypt(New HexKey(Cryptography.LMK.LMKStorage.LMKVariant(Core.LMKPairs.LMKPair.Pair04_05, var)), encryptedZMK)
            Case Core.KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, Core.KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                Return TripleDES.TripleDESDecryptVariant(New HexKey(Cryptography.LMK.LMKStorage.LMKVariant(Core.LMKPairs.LMKPair.Pair04_05, var)), encryptedZMK)
            Case Else
                Throw New InvalidOperationException("Invalid key scheme [" + ks.ToString + "]")
        End Select

    End Function

    ''' <summary>
    ''' Encrypts clear data under a ZMK.
    ''' </summary>
    ''' <remarks>
    ''' This method may be used with Thales commands that encrypt key output under a ZMK.
    ''' </remarks>
    Public Shared Function EncryptUnderZMK(ByVal clearZMK As String, ByVal clearData As String, _
                                           ByVal ZMK_KeyScheme As KeySchemeTable.KeyScheme) As String

        Dim result As String = ""

        Select Case ZMK_KeyScheme
            Case KeySchemeTable.KeyScheme.SingleDESKey, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.Unspecified
                result = TripleDES.TripleDESEncrypt(New HexKey(clearZMK), clearData)
            Case KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                result = TripleDES.TripleDESEncryptVariant(New HexKey(clearZMK), clearData)
        End Select

        Select Case ZMK_KeyScheme
            Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                result = KeySchemeTable.GetKeySchemeValue(ZMK_KeyScheme) + result
        End Select

        Return result

    End Function

    ''' <summary>
    ''' Appends a directory separator to a path, if one is needed.
    ''' </summary>
    ''' <param name="path">Path name.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AppendDirectorySeparator(ByVal path As String) As String
        Dim endChar As Char
        If path.IndexOf("\"c) > -1 Then
            endChar = "\"c
        Else
            endChar = "/"c
        End If
        If path.EndsWith(endChar) Then
            Return path
        Else
            Return path + endChar
        End If
    End Function

    ''' <summary>
    ''' Returns the current time, format HH:MM:SS.mmm.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getTimeMMHHSSmmmm() As String
        Return Now.Hour.ToString.PadLeft(2, "0"c) + ":" + Now.Minute.ToString.PadLeft(2, "0"c) + ":" + Now.Second.ToString.PadLeft(2, "0"c) + "." + Now.Millisecond.ToString.PadLeft(3, "0"c)
    End Function

End Class
