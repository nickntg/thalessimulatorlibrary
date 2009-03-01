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

Imports System.IO
Imports System.Security.Cryptography

Namespace Cryptography

    ''' <summary>
    ''' Generic byte-oriented and hex-oriented DES operations.
    ''' </summary>
    ''' <remarks>
    ''' The DES class uses the .Net DESCryptoServiceProvider to implement DES encryption/decryption
    ''' using the ECB mode.
    ''' </remarks>
    Public Class DES

        ''' <summary>
        ''' Encrypts a byte array.
        ''' </summary>
        ''' <remarks>
        ''' The method encrypts a byte array of 16 bytes.
        ''' </remarks>
        Public Shared Sub byteDESEncrypt(ByVal bKey() As Byte, ByVal bData() As Byte, ByRef bResult() As Byte)

            Try
                Dim outStream As MemoryStream
                Dim desProvider As New DESCryptoServiceProvider
                Dim csMyCryptoStream As CryptoStream
                Dim bNullVector() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}

                ReDim bResult(7)

                If DESCryptoServiceProvider.IsWeakKey(bKey) Then
                    Log.Logger.MajorWarning("***DES Encrypt with weak key***")
                    For i As Integer = 0 To 7
                        bResult(i) = bData(i)
                    Next
                    Return
                End If

                outStream = New MemoryStream(bResult)

                desProvider.Mode = CipherMode.ECB
                desProvider.Key = bKey
                desProvider.IV = bNullVector
                desProvider.Padding = PaddingMode.None

                csMyCryptoStream = New CryptoStream(outStream, desProvider.CreateEncryptor(bKey, bNullVector), CryptoStreamMode.Write)
                csMyCryptoStream.Write(bData, 0, 8)
                csMyCryptoStream.Close()
            Catch ex As Exception
                Throw New Exceptions.XEncryptError(ex.Message())
            End Try

        End Sub

        ''' <summary>
        ''' Decrypts a byte array.
        ''' </summary>
        ''' <remarks>
        ''' This method decrypts a byte array of 16 bytes.
        ''' </remarks>
        Public Shared Sub byteDESDecrypt(ByVal bKey() As Byte, ByVal bData() As Byte, ByRef bResult() As Byte)

            Try
                Dim outStream As MemoryStream
                Dim desProvider As New DESCryptoServiceProvider
                Dim csMyCryptoStream As CryptoStream
                Dim bNullVector() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}

                ReDim bResult(7)

                If DESCryptoServiceProvider.IsWeakKey(bKey) Then
                    Log.Logger.MajorWarning("***DES Decrypt with weak key***")
                    For i As Integer = 0 To 7
                        bResult(i) = bData(i)
                    Next
                    Return
                End If

                desProvider.Mode = CipherMode.ECB
                desProvider.Key = bKey
                desProvider.IV = bNullVector
                desProvider.Padding = PaddingMode.None

                outStream = New MemoryStream(bResult)
                csMyCryptoStream = New CryptoStream(outStream, desProvider.CreateDecryptor(bKey, bNullVector), CryptoStreamMode.Write)
                csMyCryptoStream.Write(bData, 0, 8)
                csMyCryptoStream.Close()
            Catch ex As Exception
                Throw New Exceptions.XDecryptError(ex.Message())
            End Try

        End Sub

        ''' <summary>
        ''' Encrypts a hex string.
        ''' </summary>
        ''' <remarks>
        ''' This method encrypts hex data under a hex key and returns the result.
        ''' </remarks>
        Public Shared Function DESEncrypt(ByVal sKey As String, ByVal sData As String) As String

            Dim bKey(7) As Byte, bData(7) As Byte, bOutput(7) As Byte, sResult As String = ""

            Try
                Core.Utility.HexStringToByteArray(sKey, bKey)
                Core.Utility.HexStringToByteArray(sData, bData)
                byteDESEncrypt(bKey, bData, bOutput)
                Core.Utility.ByteArrayToHexString(bOutput, sResult)
                Return sResult
            Catch ex As Exception
                Throw New Exceptions.XEncryptError(ex.Message())
            End Try

        End Function

        'DES-decrypt a 16-hex block using a 16-hex key
        ''' <summary>
        ''' Decrypts a hex string.
        ''' </summary>
        ''' <remarks>
        ''' This method decrypts hex data using a hex key and returns the result.
        ''' </remarks>
        Public Shared Function DESDecrypt(ByVal sKey As String, ByVal sData As String) As String

            Dim bKey(7) As Byte, bData(7) As Byte, bOutput(7) As Byte
            Dim sResult As String = ""

            Try
                Core.Utility.HexStringToByteArray(sKey, bKey)
                Core.Utility.HexStringToByteArray(sData, bData)

                byteDESDecrypt(bKey, bData, bOutput)
                Core.Utility.ByteArrayToHexString(bOutput, sResult)
                Return sResult
            Catch ex As Exception
                Throw New Exceptions.XDecryptError(ex.Message())
            End Try

        End Function

    End Class

End Namespace
