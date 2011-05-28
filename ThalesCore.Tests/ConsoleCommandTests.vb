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

Imports System
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ThalesSim.Core
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core.ConsoleCommands

<TestClass()> Public Class ConsoleCommandTests
    Private WithEvents o As ThalesSim.Core.ThalesMain

    Private testContextInstance As TestContext

    Private Const ZEROES As String = "0000000000000000"

    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = value
        End Set
    End Property

    <TestInitialize()> Public Sub InitTests()
        o = New ThalesSim.Core.ThalesMain
        o.StartUpWithoutTCP("..\..\..\ThalesCore\ThalesParameters.xml")
    End Sub

    <TestCleanup()> Public Sub EndTests()
        o.ShutDown()
        o = Nothing
    End Sub

    Private Function TestCommand(ByVal input() As String, ByVal CC As AConsoleCommand) As String
        CC.InitializeStack()

        Dim ret As String = ""

        If CC.IsNoinputCommand() Then
            ret = CC.ProcessMessage()
            CC = Nothing
        Else
            Dim i As Integer = 0
            While (i < input.GetLength(0)) AndAlso (Not CC.CommandFinished)
                CC.GetClientMessage()
                ret = CC.AcceptMessage(input(i))
                i += 1
            End While
        End If

        Return ret
    End Function

    <TestMethod()> _
    Public Sub TestEnterAuthorizedState()
        TestCommand(New String() {}, New EnterAuthorizedState_A())
        Assert.IsTrue(IsInAuthorizedState)
    End Sub

    <TestMethod()> _
    Public Sub TestExitAuthorizedState()
        TestCommand(New String() {}, New CancelAuthorizedState_C())
        Assert.IsFalse(IsInAuthorizedState)
    End Sub

    <TestMethod()> _
    Public Sub TestDoubleLengthDesCalculator()
        Dim k As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Assert.AreEqual("Encrypted: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES)) + vbCrLf + "Decrypted: " + Breakup(TripleDES.TripleDESDecrypt(k, ZEROES)), _
                        TestCommand(New String() {k.ToString(), ZEROES}, New DoubleLengthDESCalculator_Dollar()))
    End Sub

    <TestMethod()> _
    Public Sub TestSingleLengthDesCalculator()
        Dim k As HexKey = GetRandomKey(HexKey.KeyLength.SingleLength)
        Assert.AreEqual("Encrypted: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES)) + vbCrLf + "Decrypted: " + Breakup(TripleDES.TripleDESDecrypt(k, ZEROES)), _
                        TestCommand(New String() {k.ToString(), ZEROES}, New SingleLengthDESCalculator_N()))
    End Sub

    <TestMethod()> _
    Public Sub TestTripleLengthDesCalculator()
        Dim k As HexKey = GetRandomKey(HexKey.KeyLength.TripleLength)
        Assert.AreEqual("Encrypted: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES)) + vbCrLf + "Decrypted: " + Breakup(TripleDES.TripleDESDecrypt(k, ZEROES)), _
                        TestCommand(New String() {k.ToString(), "S", ZEROES}, New TripleLengthDESCalculator_T()))
    End Sub

    <TestMethod()> _
    Public Sub TestEncryptClearComponent()
        AuthorizedStateOn()
        Dim k As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Assert.AreEqual("Encrypted Component: " + Breakup(Utility.EncryptUnderLMK(k.ToString(), KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, LMKPairs.LMKPair.Pair04_05, "0")) + vbCrLf + _
                        "Key check value: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES).Substring(0, 6)), _
                        TestCommand(New String() {"000", "U", k.ToString()}, New EncryptClearComponent_EC()))

        Assert.AreEqual("Encrypted Component: " + Breakup(Utility.EncryptUnderLMK(k.ToString(), KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, LMKPairs.LMKPair.Pair06_07, "0")) + vbCrLf + _
                        "Key check value: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES).Substring(0, 6)), _
                        TestCommand(New String() {"001", "X", k.ToString()}, New EncryptClearComponent_EC()))

        k = GetRandomKey(HexKey.KeyLength.TripleLength)
        Assert.AreEqual("Encrypted Component: " + Breakup(Utility.EncryptUnderLMK(k.ToString(), KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, LMKPairs.LMKPair.Pair26_27, "0")) + vbCrLf + _
                        "Key check value: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES).Substring(0, 6)), _
                        TestCommand(New String() {"008", "Y", k.ToString()}, New EncryptClearComponent_EC()))
    End Sub

    <TestMethod()> _
    Public Sub TestExportKey()
        AuthorizedStateOn()
        Dim k As HexKey = Nothing, ZMK As HexKey = Nothing, cryptZMK As String = "", cryptKey As String = "", cryptUnderZMK As String = ""
        GenerateTestKeyAndZMKKey(k, LMKPairs.LMKPair.Pair06_07, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, ZMK, cryptZMK, cryptKey, cryptUnderZMK)
        Assert.AreEqual("Key encrypted under ZMK: " + Breakup(cryptUnderZMK) + vbCrLf + _
                        "Key Check Value: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES).Substring(0, 6)), _
                        TestCommand(New String() {"001", "U", cryptZMK, cryptKey}, New ExportKey_KE()))
    End Sub

    <TestMethod()> _
    Public Sub TestFormKeyFromComponents()
        AuthorizedStateOn()
        Dim cmp1 As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Dim cmp2 As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Dim cmp3 As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Dim zmk As HexKey = New HexKey(Utility.XORHexStringsFull(Utility.XORHexStringsFull(cmp1.ToString, cmp2.ToString), cmp3.ToString))
        Dim cryptZMK As String = Utility.EncryptUnderLMK(zmk.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, LMKPairs.LMKPair.Pair04_05, "0")
        Assert.AreEqual("Encrypted key: " + Breakup(cryptZMK) + vbCrLf + _
                        "Key check value: " + Breakup(TripleDES.TripleDESEncrypt(zmk, ZEROES).Substring(0, 6)), _
                        TestCommand(New String() {"2", "000", "U", "X", "3", cmp1.ToString, cmp2.ToString, cmp3.ToString}, New FormKeyFromComponents_FK()))
    End Sub

    <TestMethod()> _
    Public Sub TestZMKFromEncryptedComponents()
        AuthorizedStateOn()
        Dim cmp1 As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Dim cmp2 As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Dim cmp3 As HexKey = GetRandomKey(HexKey.KeyLength.DoubleLength)
        Dim zmk As HexKey = New HexKey(Utility.XORHexStringsFull(Utility.XORHexStringsFull(cmp1.ToString, cmp2.ToString), cmp3.ToString))
        Dim cryptZMK As String = Utility.EncryptUnderLMK(zmk.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, LMKPairs.LMKPair.Pair04_05, "0")
        Assert.AreEqual("Encrypted key: " + Breakup(cryptZMK) + vbCrLf + _
                        "Key check value: " + Breakup(TripleDES.TripleDESEncrypt(zmk, ZEROES).Substring(0, 6)), _
                        TestCommand(New String() {"3", Utility.RemoveKeyType(Utility.EncryptUnderLMK(cmp1.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, LMKPairs.LMKPair.Pair04_05, "0")), _
                                                       Utility.RemoveKeyType(Utility.EncryptUnderLMK(cmp2.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, LMKPairs.LMKPair.Pair04_05, "0")), _
                                                       Utility.RemoveKeyType(Utility.EncryptUnderLMK(cmp3.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, LMKPairs.LMKPair.Pair04_05, "0"))}, New FormZMKFromEncryptedComponents_D()))
    End Sub

    <TestMethod()> _
    Public Sub TestImportKey()
        AuthorizedStateOn()
        Dim k As HexKey = Nothing, ZMK As HexKey = Nothing, cryptZMK As String = "", cryptKey As String = "", cryptUnderZMK As String = ""
        GenerateTestKeyAndZMKKey(k, LMKPairs.LMKPair.Pair06_07, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, ZMK, cryptZMK, cryptKey, cryptUnderZMK)
        Assert.AreEqual("Key under LMK: " + Breakup(cryptKey) + vbCrLf + _
                        "Key Check Value: " + Breakup(TripleDES.TripleDESEncrypt(k, ZEROES).Substring(0, 6)), _
                        TestCommand(New String() {"001", "X", cryptZMK, cryptUnderZMK}, New ImportKey_IK()))
    End Sub

    'Generate a randoom key, a random zmk and encrypt key under zmk and both under lmk.
    Private Sub GenerateTestKeyAndZMKKey(ByRef k As HexKey, ByVal kLMK As LMKPairs.LMKPair, ByVal kScheme As KeySchemeTable.KeyScheme, ByRef ZMK As HexKey, ByRef cryptZMK As String, ByRef cryptKey As String, ByRef cryptUnderZMK As String)
        k = GetRandomKey(HexKey.KeyLength.DoubleLength)
        ZMK = GetRandomKey(HexKey.KeyLength.DoubleLength)
        cryptZMK = Utility.EncryptUnderLMK(ZMK.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, LMKPairs.LMKPair.Pair04_05, "0")
        cryptKey = Utility.EncryptUnderLMK(k.ToString, kScheme, kLMK, "0")
        cryptUnderZMK = Utility.EncryptUnderZMK(ZMK.ToString, k.ToString, kScheme)
    End Sub

    'Generates a random key of specified length.
    Private Function GetRandomKey(ByVal l As HexKey.KeyLength) As HexKey
        Select Case l
            Case HexKey.KeyLength.SingleLength
                Return New HexKey(Utility.RandomKey(True, Utility.ParityCheck.OddParity))
            Case HexKey.KeyLength.DoubleLength
                Return New HexKey(Utility.MakeParity(Utility.RandomKey(True, Utility.ParityCheck.OddParity) + Utility.RandomKey(True, Utility.ParityCheck.OddParity), Utility.ParityCheck.OddParity))
            Case Else
                Return New HexKey(Utility.MakeParity(Utility.RandomKey(True, Utility.ParityCheck.OddParity) + Utility.RandomKey(True, Utility.ParityCheck.OddParity) + Utility.RandomKey(True, Utility.ParityCheck.OddParity), Utility.ParityCheck.OddParity))
        End Select
    End Function

    'Enters a space to a string every four characters.
    Private Function Breakup(ByVal s As String) As String
        Dim ret As String = ""
        Dim key As String = Utility.RemoveKeyType(s)
        For i As Integer = 0 To key.Length - 1 Step 4
            If i + 4 <= key.Length Then
                ret = ret + key.Substring(i, 4) + " "
            Else
                ret = ret + key.Substring(i)
            End If
        Next
        If key <> s Then
            Return s.Substring(0, 1) + " " + ret
        Else
            Return ret
        End If
    End Function

    'Put the simulator in the authorized state.
    Private Sub AuthorizedStateOn()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
    End Sub

    'Put the simulator out of the authorized state.
    Private Sub AuthorizedStateOff()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    'Returns True if the simulator is in the authorized state.
    Private Function IsInAuthorizedState() As Boolean
        Return Convert.ToBoolean(Resources.GetResource(Resources.AUTHORIZED_STATE))
    End Function
End Class
