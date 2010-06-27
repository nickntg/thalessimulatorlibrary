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

Imports ThalesSim.Core
Imports ThalesSim.Core.Cryptography

''' <summary>
''' This class implements total storage for a key. It has the capability to:
''' 
''' * Load a clear key.
''' * Load a key encrypted under an LMK.
''' 
''' Once the key is loaded and an appropriate LMK is defined, the class can
''' provide the clear and encrypted key values. Encrypted key values are
''' provided in ANSI and variant format. Finally, the class also provides
''' the key check values.
''' </summary>
''' <remarks></remarks>
Public Class CryptoKey

    Protected m_key As HexKey
    Protected m_keyType As KeyType = KeyType.Undefined

    ''' <summary>
    ''' Get/set the key type.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property KeyType() As KeyType
        Get
            Return m_keyType
        End Get
        Set(ByVal value As KeyType)
            m_keyType = value
        End Set
    End Property

    ''' <summary>
    ''' Creates an instance of this class using a clear key.
    ''' </summary>
    ''' <param name="hexKey">Clear key value.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal hexKey As String)
        If Utility.RemoveKeyType(hexKey) <> hexKey Then
            Throw New Exception("Not clear key")
        End If

        m_key = New HexKey(hexKey)
    End Sub

    ''' <summary>
    ''' Creates an instance of this class using an encrypted key.
    ''' </summary>
    ''' <param name="cryptHexKey">Encrypted key value.</param>
    ''' <param name="keyType">Key type.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal cryptHexKey As String, ByVal keyType As KeyType)
        m_keyType = keyType
        If keyType = ThalesKeyManager.KeyType.Undefined Then
            m_key = New HexKey(cryptHexKey)
        Else
            m_key = New HexKey(Utility.RemoveKeyType(DecryptUnderLMK(cryptHexKey)))
        End If
    End Sub

    ''' <summary>
    ''' Returns the clear key value as a hexadecimal key.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetClearValue() As String
        Return m_key.ToString
    End Function

    ''' <summary>
    ''' Returns the clear key.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetClearKey() As HexKey
        Return m_key
    End Function

    ''' <summary>
    ''' Returns the encrypted key value (ANSI format, without a starting character).
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetEncryptedValue() As String
        If m_keyType = ThalesKeyManager.KeyType.Undefined Then
            Return ""
        Else
            Return Utility.EncryptUnderLMK(m_key.ToString, KeySchemeTable.KeyScheme.Unspecified, GetLMKPair(m_keyType), GetVariant(m_keyType))
        End If
    End Function

    ''' <summary>
    ''' Returns the encrypted key value (ANSI format).
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetANSIValue() As String
        If m_keyType = ThalesKeyManager.KeyType.Undefined Then
            Return ""
        Else
            Select Case m_key.KeyLen
                Case HexKey.KeyLength.DoubleLength
                    Return Utility.EncryptUnderLMK(m_key.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, GetLMKPair(m_keyType), GetVariant(m_keyType))
                Case HexKey.KeyLength.SingleLength
                    Return GetEncryptedValue()
                Case Else
                    Return Utility.EncryptUnderLMK(m_key.ToString, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, GetLMKPair(m_keyType), GetVariant(m_keyType))
            End Select
        End If
    End Function

    ''' <summary>
    ''' Returns the encrypted key value (variant format).
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVariantValue() As String
        If m_keyType = ThalesKeyManager.KeyType.Undefined Then
            Return ""
        Else
            Select Case m_key.KeyLen
                Case HexKey.KeyLength.DoubleLength
                    Return Utility.EncryptUnderLMK(m_key.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, GetLMKPair(m_keyType), GetVariant(m_keyType))
                Case HexKey.KeyLength.SingleLength
                    Return GetEncryptedValue()
                Case Else
                    Return Utility.EncryptUnderLMK(m_key.ToString, KeySchemeTable.KeyScheme.TripleLengthKeyVariant, GetLMKPair(m_keyType), GetVariant(m_keyType))
            End Select
        End If
    End Function

    ''' <summary>
    ''' Calculates and returns the key check value.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCheckValue() As String
        Return TripleDES.TripleDESEncrypt(m_key, "0000000000000000")
    End Function

    ''' <summary>
    ''' Returns true/false depending on whether the key has odd parity or not.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsOddParity() As Boolean
        Return Utility.IsParityOK(m_key.ToString, Utility.ParityCheck.OddParity)
    End Function

    ''' <summary>
    ''' Decrypts the key under the appropriate LMK and variant.
    ''' </summary>
    ''' <param name="cryptHexKey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DecryptUnderLMK(ByVal cryptHexKey As String) As String
        Dim k As New HexKey(cryptHexKey)
        Return Utility.DecryptUnderLMK(k.ToString, k.Scheme, GetLMKPair(m_keyType), GetVariant(m_keyType))
    End Function

    ''' <summary>
    ''' Returns the appropriate LMK, given a key type.
    ''' </summary>
    ''' <param name="keyType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetLMKPair(ByVal keyType As KeyType) As LMKPairs.LMKPair
        Select Case keyType
            Case ThalesKeyManager.KeyType.BDK
                Return LMKPairs.LMKPair.Pair28_29
            Case ThalesKeyManager.KeyType.MK_AC
                Return LMKPairs.LMKPair.Pair28_29
            Case ThalesKeyManager.KeyType.MK_CVC3
                Return LMKPairs.LMKPair.Pair28_29
            Case ThalesKeyManager.KeyType.MK_DAC
                Return LMKPairs.LMKPair.Pair28_29
            Case ThalesKeyManager.KeyType.MK_SMC
                Return LMKPairs.LMKPair.Pair28_29
            Case ThalesKeyManager.KeyType.PVK
                Return LMKPairs.LMKPair.Pair14_15
            Case ThalesKeyManager.KeyType.TAK
                Return LMKPairs.LMKPair.Pair16_17
            Case ThalesKeyManager.KeyType.TPK
                Return LMKPairs.LMKPair.Pair14_15
            Case ThalesKeyManager.KeyType.TMK
                Return LMKPairs.LMKPair.Pair14_15
            Case ThalesKeyManager.KeyType.ZAK
                Return LMKPairs.LMKPair.Pair26_27
            Case ThalesKeyManager.KeyType.ZEK
                Return LMKPairs.LMKPair.Pair30_31
            Case ThalesKeyManager.KeyType.ZMK
                Return LMKPairs.LMKPair.Pair04_05
            Case ThalesKeyManager.KeyType.ZPK
                Return LMKPairs.LMKPair.Pair06_07
            Case ThalesKeyManager.KeyType.CVK
                Return LMKPairs.LMKPair.Pair14_15
            Case ThalesKeyManager.KeyType.Undefined
                Throw New Exception("No LMK pair for undefined key type")
        End Select
    End Function

    ''' <summary>
    ''' Returns the appropriate variant, given a key type.
    ''' </summary>
    ''' <param name="keyType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetVariant(ByVal keyType As KeyType) As String
        Select Case keyType
            Case ThalesKeyManager.KeyType.MK_AC
                Return "1"
            Case ThalesKeyManager.KeyType.MK_CVC3
                Return "7"
            Case ThalesKeyManager.KeyType.MK_DAC
                Return "4"
            Case ThalesKeyManager.KeyType.MK_SMC
                Return "3"
            Case ThalesKeyManager.KeyType.CVK
                Return "4"
            Case ThalesKeyManager.KeyType.Undefined
                Throw New Exception("No variant for undefined key type")
            Case Else
                Return "0"
        End Select
    End Function

End Class
