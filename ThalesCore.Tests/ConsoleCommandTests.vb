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
        For i As Integer = 0 To s.Length - 1 Step 4
            ret = ret + s.Substring(i, 4) + " "
        Next
        Return ret
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
