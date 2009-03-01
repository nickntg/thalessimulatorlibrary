Imports System
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ThalesSim.Core.Cryptography

<TestClass()> Public Class EncryptionTests

    Private Const ZEROES As String = "0000000000000000"

    Private testContextInstance As TestContext

    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = Value
        End Set
    End Property

    <TestMethod()> Public Sub TestSimpleDES()
        Dim sResult As String = DES.DESEncrypt("0123456789ABCDEF", ZEROES)
        Assert.AreEqual(sResult, "D5D44FF720683D0D")
        Dim sResult2 As String = DES.DESDecrypt("0123456789ABCDEF", sResult)
        Assert.AreEqual(sResult2, ZEROES)
    End Sub

    <TestMethod()> _
    Public Sub TestTripleDES()
        Dim sResult As String = TripleDES.TripleDESEncrypt(New HexKey("0123456789ABCDEFABCDEF0123456789"), ZEROES)
        Assert.AreEqual(sResult, "EE21F1F01A3D7C9A")
        Dim sResult2 As String = TripleDES.TripleDESDecrypt(New HexKey("0123456789ABCDEFABCDEF0123456789"), sResult)
        Assert.AreEqual(sResult2, ZEROES)
    End Sub

    <TestMethod()> _
    Public Sub TestDoubleVariant()
        LMK.LMKStorage.LMKStorageFile = "LMKSTORAGE.TXT"
        LMK.LMKStorage.GenerateTestLMKs()

        Dim sResult As String = TripleDES.TripleDESEncryptVariant(New HexKey(LMK.LMKStorage.LMKVariant(Core.LMKPairs.LMKPair.Pair28_29, 2)), "F1F1F1F1F1F1F1F1C1C1C1C1C1C1C1C1")
        Assert.AreEqual(sResult, "5178C9D3D1052B15BF6AEC458B4A4564")
    End Sub
End Class
