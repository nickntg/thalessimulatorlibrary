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
' Contributed by rjw - May 2010

Namespace Cryptography.DUKPT
    Public Class KeySerialNumber
        Private _baseKeyId As String
        Private _trsmId As String
        Private _transactionCounter As String
        Private _paddedKsn As String
        Private _unpaddedKsn As String

        Public Property baseKeyId() As String
            Get
                Return _baseKeyId
            End Get
            Set(ByVal value As String)
                _baseKeyId = value
            End Set
        End Property

        Public Property trsmId() As String
            Get
                Return _trsmId
            End Get
            Set(ByVal value As String)
                _trsmId = value
            End Set
        End Property

        Public Property transactionCounter() As String
            Get
                Return _transactionCounter
            End Get
            Set(ByVal value As String)
                _transactionCounter = value
            End Set
        End Property

        Public Property paddedKsn() As String
            Get
                Return _paddedKsn
            End Get
            Set(ByVal value As String)
                _paddedKsn = value
            End Set
        End Property

        Public Property unpaddedKsn() As String
            Get
                Return _unpaddedKsn
            End Get
            Set(ByVal value As String)
                _unpaddedKsn = value
            End Set
        End Property


        Public Sub New(ByVal KSN As String, ByVal KSNDescriptor As String)
            If KSN Is Nothing OrElse KSN = "" OrElse KSN.Length < 16 Then
                Throw New Exceptions.XInvalidData("Invalid KSN data: " + KSN)
            End If
            If KSNDescriptor Is Nothing OrElse KSNDescriptor = "" OrElse KSNDescriptor.Length <> 3 Then
                Throw New Exceptions.XInvalidData("Invalid KSNDescriptor data: " + KSNDescriptor)
            End If

            paddedKsn = KSN
            If KSN.StartsWith("FFFF") Then
                KSN = KSN.Remove(0, 4)
            End If
            unpaddedKsn = KSN

            Dim p As Integer = 0

            baseKeyId = KSN.Substring(p, Convert.ToInt32(KSNDescriptor.Substring(0, 1)))
            p += Convert.ToInt32(KSNDescriptor.Substring(0, 1))

            trsmId = KSN.Substring(p, Convert.ToInt32(KSNDescriptor.Substring(2, 1)))
            p += Convert.ToInt32(KSNDescriptor.Substring(2, 1))

            transactionCounter = Convert.ToString(Convert.ToInt32(KSN(p - 1), 16) And &H1, 16) + KSN.Substring(p, KSN.Length - p)
        End Sub

    End Class
End Namespace
