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

    Public Class DerivedKey
        Public Shared Function calculateIPEK(ByVal ksn As KeySerialNumber, ByVal bdk As String) As String
            Dim ipek As String
            Dim xorbdk As String
            Dim iKsn As String = Utility.ANDHexStringsOffset(ksn.paddedKsn, "E00000", 14).Substring(0, 16)


            ipek = TripleDES.TripleDESEncrypt(New HexKey(bdk), iKsn)
            xorbdk = Utility.XORHexStringsFull(bdk.ToString, "C0C0C0C000000000C0C0C0C000000000")
            ipek = ipek + TripleDES.TripleDESEncrypt(New HexKey(xorbdk), iKsn)
            Return ipek
        End Function

        Public Shared Function calculateDerivedKey(ByVal ksn As KeySerialNumber, ByVal bdk As String) As String
            Dim _1FFFFF As String = "1FFFFF"
            Dim _100000 As String = "100000"
            Dim _E00000 As String = "E00000"
            Dim curkey As String
            Dim reg8str As String = ""
            Dim reg3 As String
            Dim temp As String
            Dim shiftr As String
            Dim r8astr As String
            Dim r8bstr As String

            Dim shiftrstr As String = ""

            bdk = Utility.RemoveKeyType(bdk)

            '1) Copy IKEY into CURKEY.
            curkey = DUKPT.DerivedKey.calculateIPEK(ksn, bdk)

            '2) Copy KSNR into R8.
            reg8str = ksn.unpaddedKsn

            '3) Clear the 21 right-most bits of R8.
            reg8str = Utility.ANDHexStringsOffset(reg8str, _E00000, 10)
            'reg8str = Utility.ANDHexStringsOffset(reg8str, _E00000, reg8str.Length - 6) '10

            '4) Copy the 21 right-most bits of KSNR into R3.
            reg3 = Utility.ANDHexStrings(ksn.transactionCounter, _1FFFFF)

            '5) Set the left-most bit of SR, clearing the other 20 bits.
            shiftr = _100000

            Do While Convert.ToInt32(shiftr) <> 0
                temp = Utility.ANDHexStrings(shiftr, reg3)
                If Convert.ToInt32(temp) <> 0 Then
                    reg8str = Utility.ORHexStringsFull(reg8str, shiftr, 10)
                    r8astr = Utility.XORHexStringsFull(reg8str, curkey.Substring(16, 16))
                    r8astr = DES.DESEncrypt(curkey.Substring(0, 16), r8astr)
                    r8astr = Utility.XORHexStringsFull(r8astr, curkey.Substring(16, 16))
                    curkey = Utility.XORHexStringsFull(curkey.ToString, "C0C0C0C000000000C0C0C0C000000000")
                    r8bstr = Utility.XORHexStringsFull(curkey.Substring(16, 16), reg8str)
                    r8bstr = DES.DESEncrypt(curkey.Substring(0, 16), r8bstr)
                    r8bstr = Utility.XORHexStringsFull(r8bstr, curkey.Substring(16, 16))
                    curkey = r8bstr + r8astr
                End If
                shiftr = Utility.SHRHexString(shiftr)
            Loop
            curkey = Utility.XORHexStringsFull(curkey, "00000000000000FF00000000000000FF")
            Return curkey
        End Function


    End Class

End Namespace

