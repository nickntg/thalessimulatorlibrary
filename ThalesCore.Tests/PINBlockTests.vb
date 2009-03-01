Imports System
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core.PIN

<TestClass()> Public Class PINBlockTests

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

    <TestMethod()> _
    Public Sub TestANSIX98Creation()
        Assert.AreEqual(PINBlockFormat.ToPINBlock("1234", "550000025321", PINBlockFormat.PIN_Block_Format.AnsiX98), "041261FFFFFDACDE")
    End Sub

    <TestMethod()> _
    Public Sub TestDieboldCreation()
        Assert.AreEqual(PINBlockFormat.ToPINBlock("1234", "550000025321", PINBlockFormat.PIN_Block_Format.Diebold), "1234FFFFFFFFFFFF")
    End Sub

    <TestMethod()> _
    Public Sub TestANSIX98Decomposition()
        Assert.AreEqual(PINBlockFormat.ToPIN("041261FFFFFDACDE", "550000025321", PINBlockFormat.PIN_Block_Format.AnsiX98), "1234")
    End Sub

    <TestMethod()> _
    Public Sub TestDieboldDecomposition()
        Assert.AreEqual(PINBlockFormat.ToPIN("1234FFFFFFFFFFFF", "550000025321", PINBlockFormat.PIN_Block_Format.Diebold), "1234")
    End Sub

End Class
