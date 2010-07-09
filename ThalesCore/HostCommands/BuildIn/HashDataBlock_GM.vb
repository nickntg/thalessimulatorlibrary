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

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Hashes a block of data.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the GM command.
    ''' Currently the command does NOT support ISO 10118-2 and SHA-224 hash methods.
    ''' </remarks>
    <ThalesCommandCode("GM", "GN", "", "Hashes a block of data.")> _
    Public Class HashDataBlock_GM
        Inherits AHostCommand

        Const SHA1 As String = "01"
        Const MD5 As String = "02"
        Const ISO_10118_2 As String = "03"
        Const SHA_224 As String = "05"
        Const SHA_256 As String = "06"
        Const SHA_384 As String = "07"
        Const SHA_512 As String = "08"

        Private _hashID As String
        Private _bytes() As Byte

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the GM message parsing fields.
        ''' </remarks>
        Public Sub New()
            ReadXMLDefinitions()
        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            XML.MessageParser.Parse(msg, XMLMessageFields, kvp, XMLParseResult)
            If XMLParseResult = ErrorCodes.ER_00_NO_ERROR Then
                _hashID = kvp.Item("Hash Identifier")
                Dim dataLen As String = kvp.Item("Data Length")
                _bytes = Utility.GetBytesFromString(kvp.Item("Message Data"))
                If Convert.ToInt32(dataLen) <> _bytes.GetLength(0) Then
                    XMLParseResult = ErrorCodes.ER_80_DATA_LENGTH_ERROR
                End If
            End If
        End Sub

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As Message.MessageResponse

            Dim mr As New MessageResponse

            'Currently not implemented for ISO 10118-2 and SHA-224.
            If _hashID = ISO_10118_2 OrElse _hashID = SHA_224 Then
                mr.AddElement(ErrorCodes.ER_41_INTERNAL_HARDWARE_SOFTWARE_ERROR)
                Return mr
            End If

            Dim hash As Security.Cryptography.HashAlgorithm = Nothing
            Select Case _hashID
                Case SHA1
                    hash = New Security.Cryptography.SHA1Managed
                Case MD5
                    hash = New Security.Cryptography.MD5CryptoServiceProvider
                Case SHA_256
                    hash = New Security.Cryptography.SHA256Managed
                Case SHA_384
                    hash = New Security.Cryptography.SHA384Managed
                Case SHA_512
                    hash = New Security.Cryptography.SHA512Managed
            End Select

            Dim result() As Byte = hash.ComputeHash(_bytes)
            Dim resultStr As String = Utility.GetStringFromBytes(result)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(resultStr)

            Return mr
        End Function

    End Class

End Namespace
