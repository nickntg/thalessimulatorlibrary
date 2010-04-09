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
