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

Namespace Cryptography.LMK

    ''' <summary>
    ''' Represents the LMK key storage.
    ''' </summary>
    ''' <remarks>
    ''' This class is used to represent the LMK store. 
    ''' </remarks>
    Public Class LMKStorage

        Private Const MAX_LMKS As Integer = 20

        Private Shared _LMKs() As String

        Private Shared _storageFile As String = ""

        ''' <summary>
        ''' The LMK storage file.
        ''' </summary>
        ''' <remarks>
        ''' Gets or sets the LMK storage file.
        ''' </remarks>
        Public Shared Property LMKStorageFile() As String
            Get
                Return _storageFile
            End Get
            Set(ByVal Value As String)
                _storageFile = Value
            End Set
        End Property

        ''' <summary>
        ''' The value of an LMK pair.
        ''' </summary>
        ''' <remarks>
        ''' Gets the corresponding LMK pair value.
        ''' </remarks>
        Public Shared ReadOnly Property LMK(ByVal pair As Core.LMKPairs.LMKPair) As String
            Get
                Return _LMKs(Convert.ToInt32(pair))
            End Get
        End Property

        ''' <summary>
        ''' The value of an LMK pair with an applied variant.
        ''' </summary>
        ''' <remarks>
        ''' Gets the corresponding LMK pair value with the applied variant.
        ''' </remarks>
        Public Shared ReadOnly Property LMKVariant(ByVal pair As Core.LMKPairs.LMKPair, ByVal v As Integer) As String
            Get
                Dim s As String = _LMKs(Convert.ToInt32(pair))
                If v = 0 Then Return s
                Dim var As String = Cryptography.LMK.Variants.VariantNbr(v).PadRight(32, "0"c)
                Return Core.Utility.XORHexStringsFull(s, var)
            End Get
        End Property

        ''' <summary>
        ''' Read LMKs from a storage file.
        ''' </summary>
        ''' <remarks>
        ''' This method reads the keys stored in the storage file and saves them to memory.
        ''' </remarks>
        Public Shared Sub ReadLMKs(ByVal StorageFile As String)

            _storageFile = StorageFile
            Dim i As Integer = 0
            ReDim _LMKs(MAX_LMKS - 1)

            Try
                Using SR As IO.StreamReader = New IO.StreamReader(StorageFile)
                    While SR.Peek > -1
                        Dim s As String = SR.ReadLine()
                        If s <> "" AndAlso s.Trim.StartsWith(";") = False Then
                            If Core.Utility.IsHexString(s) = True Then
                                If s.Length = 32 Then
                                    _LMKs(i) = s
                                    '...and increase the index to read into the next LMK.
                                    i += 1
                                End If
                            End If
                        End If
                    End While
                End Using
            Catch ex As Exception
                ReDim _LMKs(-1)
            End Try

        End Sub

        ''' <summary>
        ''' Generates random LMK keys.
        ''' </summary>
        ''' <remarks>
        ''' This method generates and saves random LMK key pairs.
        ''' </remarks>
        Public Shared Sub GenerateLMKs()

            If _storageFile = "" Then Throw New Exceptions.XInvalidStorageFile("Invalid storage file specified, value=" + _storageFile)
            ReDim _LMKs(MAX_LMKS - 1)

            For i As Integer = 0 To MAX_LMKS - 1
                _LMKs(i) = Core.Utility.RandomKey(True, Core.Utility.ParityCheck.OddParity) + Core.Utility.RandomKey(True, Core.Utility.ParityCheck.OddParity)
            Next

            WriteLMKs()

        End Sub

        ''' <summary>
        ''' Generates the standard LMK test set.
        ''' </summary>
        ''' <remarks>
        ''' This method generates and saves the standard LMK test key set.
        ''' </remarks>
        Public Shared Sub GenerateTestLMKs()

            If _storageFile = "" Then Throw New Exceptions.XInvalidStorageFile("Invalid storage file specified, value=" + _storageFile)
            ReDim _LMKs(MAX_LMKS - 1)

            _LMKs(0) = "01010101010101017902CD1FD36EF8BA"
            _LMKs(1) = "20202020202020203131313131313131"
            _LMKs(2) = "40404040404040405151515151515151"
            _LMKs(3) = "61616161616161617070707070707070"
            _LMKs(4) = "80808080808080809191919191919191"
            _LMKs(5) = "A1A1A1A1A1A1A1A1B0B0B0B0B0B0B0B0"
            _LMKs(6) = "C1C1010101010101D0D0010101010101"
            _LMKs(7) = "E0E0010101010101F1F1010101010101"
            _LMKs(8) = "1C587F1C13924FEF0101010101010101"
            _LMKs(9) = "01010101010101010101010101010101"
            _LMKs(10) = "02020202020202020404040404040404"
            _LMKs(11) = "07070707070707071010101010101010"
            _LMKs(12) = "13131313131313131515151515151515"
            _LMKs(13) = "16161616161616161919191919191919"
            _LMKs(14) = "1A1A1A1A1A1A1A1A1C1C1C1C1C1C1C1C"
            _LMKs(15) = "23232323232323232525252525252525"
            _LMKs(16) = "26262626262626262929292929292929"
            _LMKs(17) = "2A2A2A2A2A2A2A2A2C2C2C2C2C2C2C2C"
            _LMKs(18) = "2F2F2F2F2F2F2F2F3131313131313131"
            _LMKs(19) = "01010101010101010101010101010101"

            WriteLMKs()

        End Sub

        ''' <summary>
        ''' Generates an LMK check value.
        ''' </summary>
        ''' <remarks>
        ''' Note: This does not return the proper LMK check value, just a XOR between
        ''' LMK keys.
        ''' </remarks>
        Public Shared Function GenerateLMKCheckValue() As String

            If _storageFile = "" Then Throw New Exceptions.XInvalidStorageFile("Invalid storage file specified")

            Dim s As String = Core.Utility.XORHexStringsFull(_LMKs(0), _LMKs(1))
            For i As Integer = 2 To _LMKs.GetUpperBound(0)
                s = Core.Utility.XORHexStringsFull(_LMKs(i), s)
            Next

            Return Core.Utility.XORHexStrings(s.Substring(0, 16), s.Substring(16, 16))

        End Function

        ''' <summary>
        ''' Checks the Local Master Keys for parity errors.
        ''' </summary>
        ''' <remarks>
        ''' This method checks the LMK store for parity errors.
        ''' </remarks>
        Public Shared Function CheckLMKStorage() As Boolean
            For i As Integer = 0 To MAX_LMKS - 1
                If Core.Utility.IsParityOK(_LMKs(i), Core.Utility.ParityCheck.OddParity) = False Then
                    Return False
                End If
            Next
            Return True
        End Function

        ''' <summary>
        ''' Returns all LMKs.
        ''' </summary>
        ''' <remarks>
        ''' Returns all Local Master Keys in text form.
        ''' </remarks>
        Public Shared Function DumpLMKs() As String
            Dim s As String = ""
            For i As Integer = 0 To MAX_LMKS - 1
                s += _LMKs(i) + vbCrLf
            Next
            Return s
        End Function

        Private Shared Sub WriteLMKs()

            Using SW As IO.StreamWriter = New IO.StreamWriter(_storageFile)
                SW.WriteLine("; LMK Storage file")
                For i As Integer = 0 To MAX_LMKS - 1
                    SW.WriteLine(_LMKs(i))
                Next
            End Using

        End Sub

    End Class

End Namespace
