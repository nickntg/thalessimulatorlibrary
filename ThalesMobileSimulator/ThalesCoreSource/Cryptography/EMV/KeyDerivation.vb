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

Namespace Cryptography.EMV

    ''' <summary>
    ''' This class implements methods to find a master/session derived keys.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class KeyDerivation

        ''' <summary>
        ''' Finds a master derived key.
        ''' </summary>
        ''' <param name="IMK">Initial master key.</param>
        ''' <param name="PAN">PAN.</param>
        ''' <param name="PANSequenceNo">PAN Sequence Number.</param>
        ''' <param name="DerivationMethod">Derivation method.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDerivedMasterKey(ByVal IMK As Cryptography.HexKey, ByVal PAN As String, ByVal PANSequenceNo As String, ByVal DerivationMethod As EMV.MasterKeyDerivationMethods) As Cryptography.HexKey
            Select Case DerivationMethod
                Case MasterKeyDerivationMethods.EMV_4_2_OptionA
                    Return GetDerivedKey_OptionA(IMK, PAN, PANSequenceNo)
                Case MasterKeyDerivationMethods.EMV_4_2_OptionB
                    Return GetDerivedKey_OptionB(IMK, PAN, PANSequenceNo)
                Case Else
                    Throw New NotSupportedException("Unsupported master key derivation method")
            End Select
        End Function

        ''' <summary>
        ''' Finds a master derived key.
        ''' </summary>
        ''' <param name="IMK">Initial master key.</param>
        ''' <param name="PANAndPANSequenceNo">PAN and PAN Sequence Number.</param>
        ''' <param name="DerivationMethod">Derivation method.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDerivedMasterKey(ByVal IMK As Cryptography.HexKey, ByVal PANAndPANSequenceNo As String, ByVal DerivationMethod As EMV.MasterKeyDerivationMethods) As Cryptography.HexKey
            Select Case DerivationMethod
                Case MasterKeyDerivationMethods.EMV_4_2_OptionA
                    Return GetDerivedKey_OptionA(IMK, PANAndPANSequenceNo)
                Case MasterKeyDerivationMethods.EMV_4_2_OptionB
                    Return GetDerivedKey_OptionB(IMK, PANAndPANSequenceNo)
                Case Else
                    Throw New NotSupportedException("Unsupported master key derivation method")
            End Select
        End Function

        ''' <summary>
        ''' Finds a session derived key.
        ''' </summary>
        ''' <param name="MK">Derived master key.</param>
        ''' <param name="ATC">ATC.</param>
        ''' <param name="UN">UN.</param>
        ''' <param name="DerivationMethod">Derivation method.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSessionKey(ByVal MK As Cryptography.HexKey, ByVal ATC As String, ByVal UN As String, ByVal DerivationMethod As EMV.SessionKeyDerivationMethods) As Cryptography.HexKey
            Select Case DerivationMethod
                Case SessionKeyDerivationMethods.MasterCard_Method
                    Return FindMasterCardSessionKey(MK, ATC, UN)
                Case Else
                    Throw New NotSupportedException("Unsupported session key derivation method")
            End Select
        End Function

        ''' <summary>
        ''' Calculates the derived session key using the MasterCard rules.
        ''' </summary>
        ''' <param name="MK_AC">Derived master key.</param>
        ''' <param name="ATC">Value of the application transaction counter.</param>
        ''' <param name="UN">Value of the unpredictable number.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FindMasterCardSessionKey(ByVal MK_AC As Core.Cryptography.HexKey, ByVal ATC As String, ByVal UN As String) As Core.Cryptography.HexKey
            Return New Core.Cryptography.HexKey(Core.Cryptography.TripleDES.TripleDESEncrypt(MK_AC, ATC + "F000" + UN) + Core.Cryptography.TripleDES.TripleDESEncrypt(MK_AC, ATC + "0F00" + UN))
        End Function

        ''' <summary>
        ''' Calculates the derived key using the initial key, the PAN and the sequence number.
        ''' </summary>
        ''' <param name="IMK">Initial key.</param>
        ''' <param name="PAN">PAN.</param>
        ''' <param name="PANSequenceNo">PAN sequence number.</param>
        ''' <returns></returns>
        ''' <remarks>This implements the key derivation method A.</remarks>
        Public Shared Function GetDerivedKey_OptionA(ByVal IMK As Cryptography.HexKey, ByVal PAN As String, ByVal PANSequenceNo As String) As Cryptography.HexKey
            'Add sequence number to PAN and pad to at least 16 digits. 
            'Then get the rightmost sixteen digits to form an 8-byte block.
            Return GetDerivedKey_OptionA(IMK, PAN + PANSequenceNo)
        End Function

        ''' <summary>
        ''' Calculates the derived key using the initial key, the PAN and the sequence number.
        ''' </summary>
        ''' <param name="IMK">Initial key.</param>
        ''' <param name="PANAndPANSequenceNo">PAN and PAN Sequence Number.</param>
        ''' <returns></returns>
        ''' <remarks>This implements the key derivation method A.</remarks>
        Public Shared Function GetDerivedKey_OptionA(ByVal IMK As Cryptography.HexKey, ByVal PANAndPANSequenceNo As String) As Cryptography.HexKey
            Dim Y As String = PANAndPANSequenceNo
            If Y.Length < 16 Then
                Y = Y.PadLeft(16, "0"c)
            End If
            Y = Y.Substring(Y.Length - 16)

            Return GetDerivedKeyFromPreparedPAN(IMK, Y)
        End Function

        ''' <summary>
        ''' Calculates the derived key using the initial key, the PAN and the sequence number.
        ''' This method is called when the PAN is larger than 16 digits.
        ''' </summary>
        ''' <param name="IMK">Initial key.</param>
        ''' <param name="PAN">PAN.</param>
        ''' <param name="PANSequenceNo">PAN sequence number.</param>
        ''' <returns></returns>
        ''' <remarks>This implements the key derivation method B.</remarks>
        Public Shared Function GetDerivedKey_OptionB(ByVal IMK As Cryptography.HexKey, ByVal PAN As String, ByVal PANSequenceNo As String) As Cryptography.HexKey
            Return GetDerivedKey_OptionB(IMK, PAN + PANSequenceNo)
        End Function

        ''' <summary>
        ''' Calculates the derived key using the initial key, the PAN and the sequence number.
        ''' This method is called when the PAN is larger than 16 digits.
        ''' </summary>
        ''' <param name="IMK">Initial key.</param>
        ''' <param name="PANAndPANSequenceNo">PAN and PAN Sequence Number.</param>
        ''' <returns></returns>
        ''' <remarks>This implements the key derivation method B.</remarks>
        Public Shared Function GetDerivedKey_OptionB(ByVal IMK As Cryptography.HexKey, ByVal PANAndPANSequenceNo As String) As Cryptography.HexKey
            Dim Y As String = PANAndPANSequenceNo

            'Pad to an even length.
            If Y.Length Mod 2 = 1 Then
                Y = Y + "0"c
            End If

            'Hash Y.
            Dim hash As Security.Cryptography.HashAlgorithm = New Security.Cryptography.SHA1Managed
            Dim result() As Byte = hash.ComputeHash(System.Text.ASCIIEncoding.GetEncoding(Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage).GetBytes(Y))

            'Get hex result.
            Dim resultStr As String = ""
            Utility.ByteArrayToHexString(result, resultStr)

            'Keep values A, B, C, D, E and F here.
            Dim undecimalized As String = ""

            'Try to get to the first 16 decimal characters.
            Y = ""
            For i As Integer = 0 To resultStr.Length - 1
                If Char.IsDigit(resultStr.Chars(i)) Then
                    Y = Y + resultStr.Chars(i)
                    If Y.Length = 16 Then Exit For
                Else
                    undecimalized = undecimalized + resultStr.Chars(i)
                End If
            Next

            'If more are needed, do the decimalization and get the rest.
            If Y.Length < 16 Then
                Dim decimalized As String = Utility.Decimalise(undecimalized, "012345")
                Y = Y + decimalized.Substring(0, 16 - Y.Length)
            End If

            Return GetDerivedKeyFromPreparedPAN(IMK, Y)
        End Function

        ''' <summary>
        ''' Calculates the derived key using the initial key given a massaged PAN.
        ''' </summary>
        ''' <param name="IMK">Initial key.</param>
        ''' <param name="Y">Prepared PAN from OptionA or OptionB methods.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetDerivedKeyFromPreparedPAN(ByVal IMK As Cryptography.HexKey, ByVal Y As String) As Cryptography.HexKey
            'Left key is the result of encrypting Y with the IMK.
            Dim ZL As String = Cryptography.TripleDES.TripleDESEncrypt(IMK, Y)
            'Right key is the result of encrypting Y XOR FFs with the IMK.
            Dim ZR As String = Cryptography.TripleDES.TripleDESEncrypt(IMK, Utility.XORHexStringsFull(Y, "FFFFFFFFFFFFFFFF"))

            'Left+Right = the derived key.
            Return New Cryptography.HexKey(Utility.MakeParity(ZL + ZR, Utility.ParityCheck.OddParity))
        End Function

    End Class

End Namespace