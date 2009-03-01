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
    ''' Represents a hexadecimal single, double or triple length key.
    ''' </summary>
    ''' <remarks>
    ''' Objects of this class are used as placeholders for key data. 
    ''' </remarks>
    Public Class HexKey

        ''' <summary>
        ''' Key length enumeration.
        ''' </summary>
        ''' <remarks>
        ''' This is an enumeration that defines the length of a hexadecimal key.
        ''' </remarks>
        Public Enum KeyLength
            ''' <summary>
            ''' Single length keys.
            ''' </summary>
            ''' <remarks>
            ''' Defines a single length hexadecimal key (16 digits).
            ''' </remarks>
            SingleLength = 0

            ''' <summary>
            ''' Double length keys.
            ''' </summary>
            ''' <remarks>
            ''' Defines a double length hexadecimal key (32 digits).
            ''' </remarks>
            DoubleLength = 1

            ''' <summary>
            ''' Triple length keys.
            ''' </summary>
            ''' <remarks>
            ''' Defines a triple length hexadecimal key (48 digits).
            ''' </remarks>
            TripleLength = 2
        End Enum

        Private _partA As String
        Private _partB As String
        Private _partC As String
        Private _keyLength As KeyLength

        ''' <summary>
        ''' First part of the key.
        ''' </summary>
        ''' <remarks>
        ''' Returns or sets the first part of the represented key.
        ''' </remarks>
        Public Property PartA() As String
            Get
                Return _partA
            End Get
            Set(ByVal Value As String)
                _partA = Value
            End Set
        End Property

        ''' <summary>
        ''' Second part of the key.
        ''' </summary>
        ''' <remarks>
        ''' Returns or sets the second part of the represented key.
        ''' </remarks>
        Public Property PartB() As String
            Get
                Return _partB
            End Get
            Set(ByVal Value As String)
                _partB = Value
            End Set
        End Property

        ''' <summary>
        ''' Third part of the key.
        ''' </summary>
        ''' <remarks>
        ''' Returns or sets the third part of the represented key.
        ''' </remarks>
        Public Property PartC() As String
            Get
                Return _partC
            End Get
            Set(ByVal Value As String)
                _partC = Value
            End Set
        End Property

        ''' <summary>
        ''' Key length.
        ''' </summary>
        ''' <remarks>
        ''' Returns or sets the key length.
        ''' </remarks>
        Public Property KeyLen() As KeyLength
            Get
                Return _keyLength
            End Get
            Set(ByVal Value As KeyLength)
                _keyLength = Value
            End Set
        End Property

        ''' <summary>
        ''' Key constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets internal variables according to the length of the 
        ''' supplied key. Use the <see cref="Cryptography.HexKey.KeyLen"/> property
        ''' to determine the length of the key instead of the <see cref="Cryptography.HexKey.PartA"/>,
        '''  <see cref="Cryptography.HexKey.PartB"/> and <see cref="Cryptography.HexKey.PartC"/>
        ''' properties. Internally, the key is always represented as a triple length key according to the
        ''' following rule:
        ''' <P>If the key data is 16 digits long, all key parts are set to this value.</P>
        ''' <P>If the key data is 32 digits long, <see cref="Cryptography.HexKey.PartA"/> and
        ''' <see cref="Cryptography.HexKey.PartC"/> are both set to the first 16 digits, and
        ''' <see cref="Cryptography.HexKey.PartB"/> is set to the last 16 digits.</P>
        ''' <P>If the key data is 48 digits long, all key parts are distributed accordingly.</P>
        ''' </remarks>
        Public Sub New(ByVal key As String)

            If key Is Nothing OrElse key = "" Then
                Throw New Exceptions.XInvalidKey("Invalid key data: " + key)
            End If

            If key.Length = 17 OrElse key.Length = 33 OrElse key.Length = 49 Then
                Dim scheme As ThalesSim.Core.KeySchemeTable.KeyScheme = ThalesSim.Core.KeySchemeTable.GetKeySchemeFromValue(key.Substring(0, 1))
                If scheme <> Core.KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi AndAlso _
                   scheme <> Core.KeySchemeTable.KeyScheme.DoubleLengthKeyVariant AndAlso _
                   scheme <> Core.KeySchemeTable.KeyScheme.SingleDESKey AndAlso _
                   scheme <> Core.KeySchemeTable.KeyScheme.TripleLengthKeyAnsi AndAlso _
                   scheme <> Core.KeySchemeTable.KeyScheme.TripleLengthKeyVariant Then
                    Throw New Exceptions.XInvalidKeyScheme("Invalid key scheme " + key.Substring(0, 1))
                Else
                    key = key.Substring(1)
                End If
            End If

            If Core.Utility.IsHexString(key) = False Then
                Throw New Exceptions.XInvalidKey("Invalid key data: " + key)
            End If

            If key.Length = 16 Then
                _partA = key
                _partB = key
                _partC = key
                _keyLength = KeyLength.SingleLength
            ElseIf key.Length = 32 Then
                _partA = key.Substring(0, 16)
                _partB = key.Substring(16)
                _partC = _partA
                _keyLength = KeyLength.DoubleLength
            ElseIf key.Length = 48 Then
                _partA = key.Substring(0, 16)
                _partB = key.Substring(16, 16)
                _partC = key.Substring(32)
                _keyLength = KeyLength.TripleLength
            Else
                Throw New Exceptions.XInvalidKey("Invalid key length: " + key)
            End If

        End Sub

    End Class

End Namespace
