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

Imports System.Security.Cryptography

Public Class RSAFunctions

    Public Shared Function EncryptData(ByVal key As RSAKey, ByVal dataToEncrypt() As Byte) As Byte()
        Return key.Key.Encrypt(dataToEncrypt, False)
    End Function

    Public Shared Function DecryptData(ByVal key As RSAKey, ByVal dataToDecrypt() As Byte) As Byte()
        Return key.Key.Decrypt(dataToDecrypt, False)
    End Function

    Public Shared Function GetStandardRSAKey() As RSAKey
        Dim key As String = "<RSAKeyValue><Modulus>qnxiIix6PkwLIu0T+TjVQDesiTbnCUk1KuEIVc/Xkwns4jaaEZlAlqMO+K7NQp054yARshMirTZwy/Vm36MqmVSUHLrnXYBs2eKuA5ahfWJjRlLYo8gx1OekLVwnxFH8DRSREB+tZAvLI8bDUxp+Bs/QTDQRUW7bprrYc8Dobys=</Modulus><Exponent>AQAB</Exponent><P>5Ak7rW0FZ/xxnhysBCsAlMQeqHRc2ifuaXr/d8dxGfX1Bl6qiKhxRocSk0ea0nUi9Q/AmVeioXjkh47apAaOMw==</P><Q>v2R4H5fkob3TVOdnnybOH4cAc898btMMesZyiNWjIN+zFKtlW2leTJxwTenEu4tx8CXMc0dB7S7NRXeh9dqzKQ==</Q><DP>gZyF4QLwabRg19+wHgZbJDN8wX6yMAU2S5nvjqM7s+fKxz4Ta+1hxRaNBk1SwIB3yBaWABKBi4ntSud1enmHZQ==</DP><DQ>j1izc44zKkTfjH7IqHXK+egGGbc0Tlj5xtbtH7lKtat2GCwK/P0dKKoenuxQcdsOGjxlNY4QPZHJIpdokZgciQ==</DQ><InverseQ>YKwDZwkWDOx7kYOOflzW7xg9M864RVYVRbxBimLm4KGnLXL08ObmudFHy6vDhOgQjsPPikaBA+bP31+Dam8Y9w==</InverseQ><D>AVWh6wSQHGa5+5cBfgTs3mjRJ+3PFyqEiQZj61AKLvN38DlfMV4SvFsgZd/waWITrux0VwGBVvvDHEWafYuobLGNTGr4R9EZCsNpgCBWclaYh84Mkh02z7QRFMhdqI011rqBTQYqbJoZLdA+OTrTTI36kh577dA/d95Ys2eRzdE=</D></RSAKeyValue>"
        Return New RSAKey(key)
    End Function

    Public Shared Function GetRSAEncryptedPINBlock(ByVal key As RSAKey, ByVal PIN As String, ByVal account As String) As Byte()
        Dim clearPB As String = Core.PIN.PINBlockFormat.ToPINBlock(PIN, account, Core.PIN.PINBlockFormat.PIN_Block_Format.AnsiX98)
        Return EncryptData(key, Utility.GetBytesFromString(clearPB))
    End Function

    Public Shared Function GetRSADecryptedPINBlock(ByVal key As RSAKey, ByVal encryptedPINBlock() As Byte) As String
        Return Utility.GetStringFromBytes(DecryptData(key, encryptedPINBlock))
    End Function

End Class
