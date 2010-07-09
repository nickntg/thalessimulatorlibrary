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

Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Configuration
Imports System.Reflection

Namespace TCP

    ''' <summary>
    ''' TPC/IP utility class.
    ''' </summary>
    ''' <remarks>
    ''' The worker client is used to send and receive data from the host application.
    ''' It abstracts TCP/IP operations and includes logic to handle a 2-byte software header
    ''' that holds the length of the message data (excluding the length of the header itself).
    ''' </remarks>
    Public Class WorkerClient

        Private MyClient As TcpClient
        Private Const PacketSize As Integer = 4096
        Private ReceiveData(PacketSize) As Byte
        Private m_ClientNum As Integer
        Private m_len As Integer = -1
        Private recBytes(65536) As Byte
        Private recBytesOffset As Integer = 0
        Private connected As Boolean = False

        ''' <summary>
        ''' Raised when the remote party disconnects.
        ''' </summary>
        ''' <remarks>
        ''' This event is raised when the remote party disconnects the session.
        ''' </remarks>
        Public Event Disconnected(ByVal sender As WorkerClient)

        ''' <summary>
        ''' Raised when data arrives from the remote party.
        ''' </summary>
        ''' <remarks>
        ''' This event is raised when a message is received from the remote party.
        ''' </remarks>
        Public Event MessageArrived(ByVal sender As WorkerClient, ByRef b() As Byte, ByVal len As Integer)

        ''' <summary>
        ''' Returns the connection status.
        ''' </summary>
        ''' <remarks>
        ''' Returns the connection status.
        ''' </remarks>
        Public ReadOnly Property IsConnected() As Boolean
            Get
                Return connected
            End Get
        End Property

        ''' <summary>
        ''' Class constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor accepts a connected <see cref="TcpClient"/> object.
        ''' </remarks>
        Public Sub New(ByVal MyClient As TcpClient)
            Me.MyClient = MyClient
        End Sub

        ''' <summary>
        ''' Object initialization.
        ''' </summary>
        ''' <remarks>
        ''' Call this method after instantiating the WorkerClient object to begin waiting
        ''' for remote party messages.
        ''' </remarks>
        Public Sub InitOps()
            connected = True
            MyClient.GetStream.BeginRead(ReceiveData, 0, PacketSize, _
                                 AddressOf StreamReceive, Nothing)
        End Sub

        ''' <summary>
        ''' Object termination.
        ''' </summary>
        ''' <remarks>
        ''' Call this method to terminate the connection to the remote party and cleanup.
        ''' </remarks>
        Public Sub TermClient()
            Try
                connected = False
                MyClient.GetStream.Close()
                MyClient.Close()
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' Returns the remote party IP address.
        ''' </summary>
        ''' <remarks>
        ''' Returns the remote party IP address.
        ''' </remarks>
        Public Function ClientIP() As String
            Return MyClient.Client.RemoteEndPoint.ToString()
        End Function

        'Property ClientNum() As Integer
        '    Get
        '        ClientNum = m_ClientNum
        '    End Get
        '    Set(ByVal Value As Integer)
        '        m_ClientNum = Value
        '    End Set
        'End Property

        ''' <summary>
        ''' For internal use.
        ''' </summary>
        ''' <remarks>
        ''' Do not call this method directly.
        ''' </remarks>
        Public Sub StreamReceive(ByVal ar As IAsyncResult)
            Dim ByteCount As Integer

            Try
                SyncLock MyClient.GetStream
                    ByteCount = MyClient.GetStream.EndRead(ar)
                End SyncLock

                If ByteCount < 1 Then
                    connected = False
                    RaiseEvent Disconnected(Me)
                    Exit Sub
                End If

                MessageAssembler(ReceiveData, 0, ByteCount)

                SyncLock MyClient.GetStream
                    MyClient.GetStream.BeginRead(ReceiveData, 0, PacketSize, _
                                                 AddressOf StreamReceive, Nothing)

                End SyncLock

            Catch ex As Exception
                connected = False
                RaiseEvent Disconnected(Me)
            End Try

        End Sub

        Private Sub MessageAssembler(ByVal Bytes() As Byte, _
                                     ByVal offset As Integer, _
                                     ByVal count As Integer)

            Static len As Integer = -1
            Static recBytesOffset As Integer = 0
            Static recBytes() As Byte = {}

            Dim ByteCount As Integer = 0

            'Unpack what we have.
            While ByteCount <> count

                'Get the software header.
                If len = -1 Then
                    If count - ByteCount < 2 Then Exit Sub
                    len = Bytes(ByteCount) * 256 + Bytes(ByteCount + 1)
                    ByteCount += 2
                    ReDim recBytes(len - 1)
                End If

                'If empty packet, fire the event now
                If len = 0 Then
                    RaiseEvent MessageArrived(Me, recBytes, 0)
                    ReDim recBytes(-1)
                    len = -1
                    recBytesOffset = 0
                Else
                    For i As Integer = ByteCount To count - 1
                        recBytes(recBytesOffset) = Bytes(i)
                        recBytesOffset += 1
                        ByteCount += 1
                        'If we have a logical packet, fire our event.
                        If recBytesOffset = len Then
                            RaiseEvent MessageArrived(Me, recBytes, recBytesOffset)
                            ReDim recBytes(-1)
                            len = -1
                            recBytesOffset = 0
                            Exit For
                        End If
                    Next
                End If

            End While

        End Sub

        ''' <summary>
        ''' Sends a message.
        ''' </summary>
        ''' <remarks>
        ''' This method sends a string message to the remote party.
        ''' </remarks>
        Public Sub send(ByVal sendData As String)

            Dim Buffer() As Byte

            'Fixed bug where this was behaving completely wrong.
            Buffer = Utility.GetBytesFromString("  " + sendData)
            Buffer(0) = Convert.ToByte(sendData.Length \ 256)
            Buffer(1) = Convert.ToByte(sendData.Length Mod 256)

            SyncLock MyClient.GetStream
                MyClient.GetStream.BeginWrite(Buffer, 0, Buffer.Length, _
                                              Nothing, Nothing)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Sends a message.
        ''' </summary>
        ''' <remarks>
        ''' This method sends a byte array message to the remote party.
        ''' </remarks>
        Public Sub send(ByVal buffer() As Byte)

            Dim b() As Byte
            ReDim b(buffer.GetLength(0) + 2 - 1)
            b(0) = Convert.ToByte(buffer.GetLength(0) \ 256)
            b(1) = Convert.ToByte(buffer.GetLength(0) Mod 256)
            Array.Copy(buffer, 0, b, 2, buffer.GetLength(0))

            SyncLock MyClient.GetStream
                MyClient.GetStream.BeginWrite(b, 0, b.Length, _
                                              Nothing, Nothing)
                ReDim b(-1)
            End SyncLock
        End Sub

    End Class

End Namespace
