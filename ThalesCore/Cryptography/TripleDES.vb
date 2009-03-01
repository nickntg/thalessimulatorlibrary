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

Namespace Cryptography

    ''' <summary>
    ''' Utility class to perform Triple DES operations.
    ''' </summary>
    ''' <remarks>
    ''' This class can be used to perform 3D operations using <see cref="HexKey"/> hexadecimal keys.
    ''' </remarks>
    Public Class TripleDES

        ''' <summary>
        ''' Performs an encryption operation.
        ''' </summary>
        ''' <remarks>
        ''' Performs an encryption operation.
        ''' </remarks>
        Public Shared Function TripleDESEncrypt(ByVal key As HexKey, ByVal data As String) As String

            If data Is Nothing AndAlso data.Length <> 16 AndAlso data.Length <> 32 AndAlso data.Length <> 48 Then Throw New Exceptions.XInvalidData("Invalid data for 3DEncrypt with variant")

            Dim result As String
            If data.Length = 16 Then
                result = TripleDESEncryptSingleLength(key, data)
            ElseIf data.Length = 32 Then
                result = TripleDESEncryptSingleLength(key, data.Substring(0, 16)) + _
                         TripleDESEncryptSingleLength(key, data.Substring(16, 16))
            Else
                result = TripleDESEncryptSingleLength(key, data.Substring(0, 16)) + _
                         TripleDESEncryptSingleLength(key, data.Substring(16, 16)) + _
                         TripleDESEncryptSingleLength(key, data.Substring(32, 16))
            End If
            Return result
        End Function

        Private Shared Function TripleDESEncryptSingleLength(ByVal key As HexKey, ByVal data As String) As String
            Dim result As String = ""
            result = DES.DESEncrypt(key.PartA, data)
            result = DES.DESDecrypt(key.PartB, result)
            result = DES.DESEncrypt(key.PartC, result)
            Return result
        End Function

        ''' <summary>
        ''' Performs a decrypt operation.
        ''' </summary>
        ''' <remarks>
        ''' Performs a decryption operation.
        ''' </remarks>
        Public Shared Function TripleDESDecrypt(ByVal key As HexKey, ByVal data As String) As String
            If data Is Nothing AndAlso data.Length <> 16 AndAlso data.Length <> 32 AndAlso data.Length <> 48 Then Throw New Exceptions.XInvalidData("Invalid data for 3DEncrypt with variant")

            Dim result As String
            If data.Length = 16 Then
                result = TripleDESDecryptSingleLength(key, data)
            ElseIf data.Length = 32 Then
                result = TripleDESDecryptSingleLength(key, data.Substring(0, 16)) + _
                         TripleDESDecryptSingleLength(key, data.Substring(16, 16))
            Else
                result = TripleDESDecryptSingleLength(key, data.Substring(0, 16)) + _
                         TripleDESDecryptSingleLength(key, data.Substring(16, 16)) + _
                         TripleDESDecryptSingleLength(key, data.Substring(32, 16))
            End If
            Return result
        End Function

        Private Shared Function TripleDESDecryptSingleLength(ByVal key As HexKey, ByVal data As String) As String
            Dim result As String
            result = DES.DESDecrypt(key.PartC, data)
            result = DES.DESEncrypt(key.PartB, result)
            result = DES.DESDecrypt(key.PartA, result)
            Return result
        End Function

        ''' <summary>
        ''' Performs an encrypt operation with an applied variant.
        ''' </summary>
        ''' <remarks>
        ''' Performs an encrypt operation with an applied variant.
        ''' </remarks>
        Public Shared Function TripleDESEncryptVariant(ByVal key As HexKey, ByVal data As String) As String

            If data Is Nothing AndAlso data.Length <> 32 AndAlso data.Length <> 48 Then Throw New Exceptions.XInvalidData("Invalid data for 3DEncrypt with variant")

            Dim result1 As String, result2 As String, result3 As String = "", orgKeyPartB As String
            If data.Length = 32 Then
                orgKeyPartB = key.PartB
                key.PartB = Core.Utility.XORHexStrings(key.PartB, Cryptography.LMK.Variants.DoubleLengthVariant(1).PadRight(16, "0"c))
                result1 = TripleDESEncrypt(key, data.Substring(0, 16))
                key.PartB = Core.Utility.XORHexStringsFull(orgKeyPartB, Cryptography.LMK.Variants.DoubleLengthVariant(2).PadRight(16, "0"c))
                result2 = TripleDESEncrypt(key, data.Substring(16, 16))
            Else
                orgKeyPartB = key.PartB
                key.PartB = Core.Utility.XORHexStringsFull(key.PartB, Cryptography.LMK.Variants.TripleLengthVariant(1).PadRight(16, "0"c))
                result1 = TripleDESEncrypt(key, data.Substring(0, 16))
                key.PartB = Core.Utility.XORHexStringsFull(orgKeyPartB, Cryptography.LMK.Variants.TripleLengthVariant(2).PadRight(16, "0"c))
                result2 = TripleDESEncrypt(key, data.Substring(16, 16))
                key.PartB = Core.Utility.XORHexStringsFull(orgKeyPartB, Cryptography.LMK.Variants.TripleLengthVariant(3).PadRight(16, "0"c))
                result3 = TripleDESEncrypt(key, data.Substring(32, 16))
            End If
            Return result1 + result2 + result3
        End Function

        ''' <summary>
        ''' Performs a decrypt operation with an applied variant.
        ''' </summary>
        ''' <remarks>
        ''' Performs a decrypt operation with an applied variant.
        ''' </remarks>
        Public Shared Function TripleDESDecryptVariant(ByVal key As HexKey, ByVal data As String) As String

            If data Is Nothing AndAlso data.Length <> 32 AndAlso data.Length <> 48 Then Throw New Exceptions.XInvalidData("Invalid data for 3DDecrypt with variant")

            Dim result1 As String = "", result2 As String = "", result3 As String = "", orgKeyPartB As String
            If data.Length = 32 Then
                orgKeyPartB = key.PartB
                key.PartB = Core.Utility.XORHexStrings(key.PartB, Cryptography.LMK.Variants.DoubleLengthVariant(1).PadRight(16, "0"c))
                result1 = TripleDESDecrypt(key, data.Substring(0, 16))
                key.PartB = Core.Utility.XORHexStringsFull(orgKeyPartB, Cryptography.LMK.Variants.DoubleLengthVariant(2).PadRight(16, "0"c))
                result2 = TripleDESDecrypt(key, data.Substring(16, 16))
            ElseIf data.Length = 48 Then
                orgKeyPartB = key.PartB
                key.PartB = Core.Utility.XORHexStringsFull(key.PartB, Cryptography.LMK.Variants.TripleLengthVariant(1).PadRight(16, "0"c))
                result1 = TripleDESDecrypt(key, data.Substring(0, 16))
                key.PartB = Core.Utility.XORHexStringsFull(orgKeyPartB, Cryptography.LMK.Variants.TripleLengthVariant(2).PadRight(16, "0"c))
                result2 = TripleDESDecrypt(key, data.Substring(16, 16))
                key.PartB = Core.Utility.XORHexStringsFull(orgKeyPartB, Cryptography.LMK.Variants.TripleLengthVariant(3).PadRight(16, "0"c))
                result3 = TripleDESDecrypt(key, data.Substring(32, 16))
            End If

            Return result1 + result2 + result3

        End Function

    End Class

End Namespace
