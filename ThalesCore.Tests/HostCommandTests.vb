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
Imports ThalesSim.Core.HostCommands
Imports ThalesSim.Core.HostCommands.BuildIn
Imports ThalesSim.Core.Message

<TestClass()> Public Class HostCommandTests

    Private WithEvents o As ThalesSim.Core.ThalesMain

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

    Private Function TestTran(ByVal input As String, ByVal HC As AHostCommand) As String
        Dim retMsg As MessageResponse
        Dim msg As New Message(input)

        Dim trailingChars As String = ""
        If ExpectTrailers Then
            trailingChars = msg.GetTrailers()
        End If

        HC.AcceptMessage(msg)

        If HC.XMLParseResult <> ErrorCodes.ER_00_NO_ERROR Then
            retMsg = New MessageResponse
            retMsg.AddElement(HC.XMLParseResult)
        Else
            retMsg = HC.ConstructResponse()
        End If

        retMsg.AddElement(trailingChars)

        HC.Terminate()
        HC = Nothing
        Return retMsg.MessageData()
    End Function

    <TestMethod()> _
    Public Sub TestGenerateZPK()
        AuthorizedStateOn()
        SwitchToDoubleLengthZMKs()
        Dim ZMK As String = TestTran("0000U", New GenerateKey_A0).Substring(2, 33)
        Assert.AreEqual("00", TestTran(ZMK, New GenerateZPK_IA()).Substring(0, 2))
    End Sub

    <TestMethod()> _
    Public Sub TestCancelAuthState()
        Assert.AreEqual(TestTran("", New CancelAuthState_RA), "00")
    End Sub

    <TestMethod()> _
    Public Sub TestSetHSMDelay()
        Assert.AreEqual("00", TestTran("001", New SetHSMDelay_LG))
    End Sub

    <TestMethod()> _
    Public Sub TestHSMStatus()
        Assert.AreEqual("00", TestTran("00", New HSMStatus_NO).Substring(0, 2))
    End Sub

    <TestMethod()> _
    Public Sub TestExportKey()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("0035ED0C0EA7F7D0FA0035BB", TestTran("0024ED06495741C280C0406FBB23A5214DFZ", New ExportKey_A8))
        SwitchToDoubleLengthZMKs()

        Assert.AreEqual("0016224FDAA779AFB31FFD3C", TestTran("002U1457FF6ADF6250C66C368416B4C9D3837BB126F2BE631486Z", New ExportKey_A8))
        Assert.AreEqual("00U2C62A23D001B97412950CD8BE66C7639070753", TestTran("002U1457FF6ADF6250C66C368416B4C9D383U8463435FC4B4DAA0C49025272C29B12CU", New ExportKey_A8))
    End Sub

    <TestMethod()> _
    Public Sub TestFormKeyFromEncryptedComponents()
        AuthorizedStateOn()
        Assert.AreEqual("00FE018240022A76DCA192FE", TestTran("3002Z3B723AF4CF00A7A6954111D254A90D17EAAF49979FA95742", New FormKeyFromEncryptedComponents_A4))
        Assert.AreEqual("00XC0BC1DFFC449A402DAB71250CA5869CC8CE396", TestTran("3000XX2EC8A0412B5D0E86E3C1E5ABFA19B3F5XFF43378ED5D85B1BC465BF000335FBF1XA235EDF4C58A2CB0C84641D07319CF21", New FormKeyFromEncryptedComponents_A4))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestFormZMKFrom3EncryptedComponents()
        AuthorizedStateOn()
        Assert.AreEqual("00XC0BC1DFFC449A402DAB71250CA5869CC8CE39643DA9A9B99", TestTran("A235EDF4C58A2CB0C84641D07319CF21FF43378ED5D85B1BC465BF000335FBF12EC8A0412B5D0E86E3C1E5ABFA19B3F5", New FormZMKFromThreeComponents_GG))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateCheckValueBU()
        Assert.AreEqual("000035BBE340A4B763", TestTran("0200406FBB23A5214DF", New GenerateCheckValue_BU))
        Assert.AreEqual("004BD5E2482582C2C4", TestTran("000ACE9B8A0BE50C09B", New GenerateCheckValue_BU))
        Assert.AreEqual("00B70AD25C94548822", TestTran("001U93CB819F8FEE4F78BF9C4CDD84750DB1", New GenerateCheckValue_BU))
        Assert.AreEqual("006F1E3F74F826B7EB", TestTran("011U1EF828AA8F6B80EB83E19FBC373F3A85", New GenerateCheckValue_BU))
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateCheckValueKA()
        Assert.AreEqual("000035BBE340A4B763", TestTran("0406FBB23A5214DF02", New GenerateCheckValue_KA))
        Assert.AreEqual("000035BB", TestTran("0406FBB23A5214DF02;ZZ1", New GenerateCheckValue_KA))
        Assert.AreEqual("006F1E3F74F826B7EB", TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8501", New GenerateCheckValue_KA))
        Assert.AreEqual("006F1E3F", TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8501;ZZ1", New GenerateCheckValue_KA))
    End Sub

    <TestMethod()> _
    Public Sub TestImportKey()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("000406FBB23A5214DF0035BB", TestTran("0024ED06495741C280C35ED0C0EA7F7D0FAZ", New ImportKey_A6))
        SwitchToDoubleLengthZMKs()
        'Contributed by wpak, fixes issue described at http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=217215.
        Assert.AreEqual("00U0E07CDC0161A0DE3B5AA44DF227EC9DEABDEBC", TestTran("001U71979DEB8587E2734F1E99D5DCAEF9ACU482C4E722BB0CF1845E1E5BD16310119U", New ImportKey_A6))
        Assert.AreEqual("00BAB32D775A38E4AB73936E", TestTran("001U1457FF6ADF6250C66C368416B4C9D3832B930A07119F93A8Z", New ImportKey_A6))
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateVISAPVV()
        Assert.AreEqual("003969", TestTran("X367930344805B1FAD6146EF4ED7502B3012345500000253211", New GenerateVISAPVV_DG))
        Assert.AreEqual("003969", TestTran("367930344805B1FAD6146EF4ED7502B3012345500000253211", New GenerateVISAPVV_DG))
        Assert.AreEqual("000550", TestTran("U183DF6EA5EDFF7D5C91C8F2BA6451884012345500000253211", New GenerateVISAPVV_DG))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyTerminalPINUsingVISAAlgorithm()
        Assert.AreEqual("00", TestTran("0406FBB23A5214DFX367930344805B1FAD6146EF4ED7502B3AE7A708F877571A90155000002532113969", New VerifyTerminalPINWithVISAAlgorithm_DC))
        Assert.AreEqual("00", TestTran("0406FBB23A5214DF367930344805B1FAD6146EF4ED7502B3AE7A708F877571A90155000002532113969", New VerifyTerminalPINWithVISAAlgorithm_DC))
        Assert.AreEqual("01", TestTran("0406FBB23A5214DF367930344805B1FAD6146EF4ED7502B3AE7A708F877571A90155000002532113962", New VerifyTerminalPINWithVISAAlgorithm_DC))
        Assert.AreEqual("00", TestTran("U8463435FC4B4DAA0C49025272C29B12CX367930344805B1FAD6146EF4ED7502B3028DCC093FB0471F0355000002532113969", New VerifyTerminalPINWithVISAAlgorithm_DC))
        Assert.AreEqual("01", TestTran("U8463435FC4B4DAA0C49025272C29B12CX367930344805B1FAD6146EF4ED7502B3028DCC093FB0471F0355000002532113962", New VerifyTerminalPINWithVISAAlgorithm_DC))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyInterchangePINUsingVISAAlgorithm()
        Assert.AreEqual("00", TestTran("BAB32D775A38E4ABX367930344805B1FAD6146EF4ED7502B3F7808F2CBEC631680355000002532113969", New VerifyInterchangePINWithVISAAlgorithm_EC))
        Assert.AreEqual("01", TestTran("BAB32D775A38E4ABX367930344805B1FAD6146EF4ED7502B3F7808F2CBEC631680355000002532113962", New VerifyInterchangePINWithVISAAlgorithm_EC))
        Assert.AreEqual("00", TestTran("U1EF828AA8F6B80EB83E19FBC373F3A85X367930344805B1FAD6146EF4ED7502B391DDDA0A7C12CFAA0155000002532113969", New VerifyInterchangePINWithVISAAlgorithm_EC))
        Assert.AreEqual("01", TestTran("U1EF828AA8F6B80EB83E19FBC373F3A85X367930344805B1FAD6146EF4ED7502B391DDDA0A7C12CFAA0155000002532113962", New VerifyInterchangePINWithVISAAlgorithm_EC))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromVISAToThales()
        AuthorizedStateOn()
        Assert.AreEqual("00001234", TestTran("55000002532101234", New TranslatePINFromVISAToThales_BQ))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromOneZPKToAnother()
        Assert.AreEqual("000482206CCD872229C203", TestTran("BAB32D775A38E4ABU1EF828AA8F6B80EB83E19FBC373F3A8512F7808F2CBEC631680303550000025321", New TranslatePINFromZPKToZPK_CC))
        Assert.AreEqual("000491DDDA0A7C12CFAA01", TestTran("BAB32D775A38E4ABU1EF828AA8F6B80EB83E19FBC373F3A8512F7808F2CBEC631680301550000025321", New TranslatePINFromZPKToZPK_CC))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromTPKToLMK()
        Assert.AreEqual("0001234", TestTran("U8463435FC4B4DAA0C49025272C29B12C028DCC093FB0471F03550000025321", New TranslatePINFromTPKToLMK_JC))
        Assert.AreEqual("0001234", TestTran("U8463435FC4B4DAA0C49025272C29B12C6428EB94035AF53B01550000025321", New TranslatePINFromTPKToLMK_JC))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromLMKToZPK()
        Assert.AreEqual("00E98FFDA17099AF55", TestTran("BAB32D775A38E4AB0155000002532101234", New TranslatePINFromLMKToZPK_JG))
        Assert.AreEqual("00F7808F2CBEC63168", TestTran("BAB32D775A38E4AB0355000002532101234", New TranslatePINFromLMKToZPK_JG))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromZPKToLMK()
        Assert.AreEqual("0001234", TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8591DDDA0A7C12CFAA01550000025321", New TranslatePINFromZPKToLMK_JE))
        Assert.AreEqual("0001234", TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8582206CCD872229C203550000025321", New TranslatePINFromZPKToLMK_JE))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromTPKToZMK()
        Assert.AreEqual("0004F7808F2CBEC6316803", TestTran("0406FBB23A5214DFBAB32D775A38E4AB12DC80B186C30902B00303550000025321", New TranslatePINFromTPKToZPK_CA))
        Assert.AreEqual("0004E98FFDA17099AF5501", TestTran("0406FBB23A5214DFBAB32D775A38E4AB12DC80B186C30902B00301550000025321", New TranslatePINFromTPKToZPK_CA))
        Assert.AreEqual("0004E98FFDA17099AF5501", TestTran("0406FBB23A5214DFBAB32D775A38E4AB12AE7A708F877571A90101550000025321", New TranslatePINFromTPKToZPK_CA))
        Assert.AreEqual("000491DDDA0A7C12CFAA01", TestTran("U8463435FC4B4DAA0C49025272C29B12CU1EF828AA8F6B80EB83E19FBC373F3A85126428EB94035AF53B0101550000025321", New TranslatePINFromTPKToZPK_CA))
        Assert.AreEqual("000482206CCD872229C203", TestTran("U8463435FC4B4DAA0C49025272C29B12CU1EF828AA8F6B80EB83E19FBC373F3A85126428EB94035AF53B0103550000025321", New TranslatePINFromTPKToZPK_CA))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateKeyScheme()
        AuthorizedStateOn()
        Assert.AreEqual("00XDA05B7A979CBD9A1DA05B7A979CBD9A1", TestTran("000U42BBE7D9A0A55D0EAA54C982B4D06B70X", New TranslateKeyScheme_B0))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestFormZMKFromTwoToNineComponents()
        AuthorizedStateOn()
        Assert.AreEqual("00C0BC1DFFC449A402DAB71250CA5869CC8CE39643DA9A9B99", TestTran("32EC8A0412B5D0E86E3C1E5ABFA19B3F5FF43378ED5D85B1BC465BF000335FBF1A235EDF4C58A2CB0C84641D07319CF21", New FormZMKFromTwoToNineComponents_GY))
        Assert.AreEqual("00U369835189A058604EB7F84EAE10C7D048CE396", TestTran("32EC8A0412B5D0E86E3C1E5ABFA19B3F5FF43378ED5D85B1BC465BF000335FBF1A235EDF4C58A2CB0C84641D07319CF21;0U1", New FormZMKFromTwoToNineComponents_GY))
        Assert.AreEqual("00XC0BC1DFFC449A402DAB71250CA5869CC8CE39643DA9A9B99", TestTran("32EC8A0412B5D0E86E3C1E5ABFA19B3F5FF43378ED5D85B1BC465BF000335FBF1A235EDF4C58A2CB0C84641D07319CF21;0X0", New FormZMKFromTwoToNineComponents_GY))

        'Unit test for the legacy mode.
        LegacyModeOn()
        Dim ZMKComp1 As String = TestTran("Component1|U00", New GenerateAndPrintZMKComponent_OC).Substring(2, 33)
        System.Threading.Thread.Sleep(50)
        Dim ZMKComp2 As String = TestTran("Component1|U00", New GenerateAndPrintZMKComponent_OC).Substring(2, 33)
        Assert.AreEqual("00", TestTran("2" + ZMKComp1 + ZMKComp2, New FormZMKFromTwoToNineComponents_GY).Substring(0, 2))
        'Get out of legacy mode and try again to get an invalid input data error.
        LegacyModeOff()
        Assert.AreEqual("15", TestTran("2" + ZMKComp1 + ZMKComp2, New FormZMKFromTwoToNineComponents_GY).Substring(0, 2))

        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZMKFromZMKToLMK()
        AuthorizedStateOn()
        Assert.AreEqual("00UF673F2E0149686A7365E73B881152B9713F44F34A77D8263", TestTran("2EC8A0412B5D0E86E3C1E5ABFA19B3F579E1B5D8DC672AF00137070260B5FA8E;XU0", New TranslateZMKFromZMKToLMK_BY))
        Assert.AreEqual("00FF43378ED5D85B1BC465BF000335FBF113F44F34A77D8263", TestTran("2EC8A0412B5D0E86E3C1E5ABFA19B3F579E1B5D8DC672AF00137070260B5FA8E", New TranslateZMKFromZMKToLMK_BY))
        Assert.AreEqual("01FF43378ED5D85B1BC465BF000335FBF113F44F34A77D8263", TestTran("2EC8A0412B5D0E86E3C1E5ABFA19B3F579E1B5D8DC672AF0B2E2357CA57E5705", New TranslateZMKFromZMKToLMK_BY))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZPKFromZMKToLMK()
        Assert.AreEqual("00BAB32D775A38E4AB73936E441E46819D", TestTran("U1457FF6ADF6250C66C368416B4C9D3832B930A07119F93A8", New TranslateZPKFromZMKToLMK_FA))
        Assert.AreEqual("00BAB32D775A38E4AB73936E", TestTran("U1457FF6ADF6250C66C368416B4C9D3832B930A07119F93A8;XZ1", New TranslateZPKFromZMKToLMK_FA))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZPKFromLMKToZMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("002B930A07119F93A873936E441E46819D", TestTran("U1457FF6ADF6250C66C368416B4C9D383BAB32D775A38E4AB", New TranslateZPKFromLMKToZMK_GC))
        Assert.AreEqual("002B930A07119F93A873936E", TestTran("U1457FF6ADF6250C66C368416B4C9D383BAB32D775A38E4AB;ZZ1", New TranslateZPKFromLMKToZMK_GC))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTMKFromLMKToTMK()
        Assert.AreEqual("00A341A0CC5F71B229", TestTran("7BB126F2BE6314860406FBB23A5214DF", New TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE))
        'This test case seems incorrect. Variant is specified as the key, so it should be also used in the output.
        'Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12CU28DDAEC83617D2F6E2302928B28A54D0", New TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE), "00XE098159ADAD90802CF32ED579CE48557")
        Assert.AreEqual("00UB44202060B6C5C0B2F372149A68FA0B5", TestTran("U8463435FC4B4DAA0C49025272C29B12CU28DDAEC83617D2F6E2302928B28A54D0", New TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE))
        Assert.AreEqual("00508949F68B4060A4", TestTran("U8463435FC4B4DAA0C49025272C29B12C0406FBB23A5214DF", New TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTMKFromZMKToLMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("000406FBB23A5214DF0035BBE340A4B763", TestTran("4ED06495741C280C35ED0C0EA7F7D0FA", New TranslateTMPTPKPVKFromZMKToLMK_FC))
        Assert.AreEqual("000406FBB23A5214DF0035BB", TestTran("4ED06495741C280C35ED0C0EA7F7D0FA;ZZ1", New TranslateTMPTPKPVKFromZMKToLMK_FC))
        SwitchToDoubleLengthZMKs()
        Assert.AreEqual("000406FBB23A5214DF0035BBE340A4B763", TestTran("U1457FF6ADF6250C66C368416B4C9D38355FF9012A3854818", New TranslateTMPTPKPVKFromZMKToLMK_FC))
        Assert.AreEqual("00U8463435FC4B4DAA0C49025272C29B12C070753", TestTran("U1457FF6ADF6250C66C368416B4C9D383XF7D53991678347EFB3026882F724E6EE;XU1", New TranslateTMPTPKPVKFromZMKToLMK_FC))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTMKFromLMKToZMK()
        AuthorizedStateOn()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("0035ED0C0EA7F7D0FA0035BBE340A4B763", TestTran("4ED06495741C280C0406FBB23A5214DF", New TranslateTMKTPKPVKFromLMKToZMK_FE))
        Assert.AreEqual("0035ED0C0EA7F7D0FA0035BB", TestTran("4ED06495741C280C0406FBB23A5214DF;ZZ1", New TranslateTMKTPKPVKFromLMKToZMK_FE))
        SwitchToDoubleLengthZMKs()
        Assert.AreEqual("00XF7D53991678347EFB3026882F724E6EE070753", TestTran("U1457FF6ADF6250C66C368416B4C9D383U8463435FC4B4DAA0C49025272C29B12C;XX1", New TranslateTMKTPKPVKFromLMKToZMK_FE))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTAKFromZMKToLMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("00AD2EE63F23D8F733EDFE6926B3B9D27C", TestTran("4ED06495741C280CE6CF9DB3EC5D766F", New TranslateTAKFromZMKToLMK_MI))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTAKFromLMKToZMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("00E6CF9DB3EC5D766FEDFE6926B3B9D27C", TestTran("4ED06495741C280CAD2EE63F23D8F733", New TranslateTAKFromLMKToZMK_MG))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTAKFromLMKToTMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("00D3A9103B524C49ACEDFE6926B3B9D27C", TestTran("0406FBB23A5214DFAD2EE63F23D8F733", New TranslateTAKFromLMKToTMK_AG))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateCVV()
        Assert.AreEqual("00561", TestTran("U9B4934384B19946B040CD702B4D581454123456789012345;8701101", New GenerateVISACVV_CW))
        Assert.AreEqual("00561", TestTran("0A61E674E88C6A7EEABC38C2B2BB492F4123456789012345;8701101", New GenerateVISACVV_CW))
        Assert.AreEqual("00649", TestTran("0A61E674E88C6A7EEABC38C2B2BB492F4999988887777;9105111", New GenerateVISACVV_CW))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyCVV()
        Assert.AreEqual("00", TestTran("U9B4934384B19946B040CD702B4D581455614123456789012345;8701101", New VerifyVISACVV_CY))
        Assert.AreEqual("01", TestTran("U9B4934384B19946B040CD702B4D581451114123456789012345;8701101", New VerifyVISACVV_CY))
        Assert.AreEqual("00", TestTran("0A61E674E88C6A7EEABC38C2B2BB492F5614123456789012345;8701101", New VerifyVISACVV_CY))
        Assert.AreEqual("00", TestTran("0A61E674E88C6A7EEABC38C2B2BB492F6494999988887777;9105111", New VerifyVISACVV_CY))
        Assert.AreEqual("01", TestTran("0A61E674E88C6A7EEABC38C2B2BB492F1114999988887777;9105111", New VerifyVISACVV_CY))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateCVKFromZMKToLMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("000A61E674E88C6A7EEABC38C2B2BB492FD5D44FA68CDC", TestTran("4ED06495741C280CAB88EE604522372FDAA27A67A8CDADFA;000", New TranslateCVKFromZMKToLMK_AW))
        Assert.AreEqual("000A61E674E88C6A7EEABC38C2B2BB492F08D7B4", TestTran("4ED06495741C280CAB88EE604522372FDAA27A67A8CDADFA;001", New TranslateCVKFromZMKToLMK_AW))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TranslateCVKFromLMKToZMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("00AB88EE604522372FDAA27A67A8CDADFAD5D44FA68CDC", TestTran("4ED06495741C280C0A61E674E88C6A7EEABC38C2B2BB492F", New TranslateCVKFromLMKToZMK_AU))
        Assert.AreEqual("00AB88EE604522372FDAA27A67A8CDADFA08D7B4", TestTran("4ED06495741C280C0A61E674E88C6A7EEABC38C2B2BB492F;ZU1", New TranslateCVKFromLMKToZMK_AU))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZEKZAKFromZMKToLMK()
        SwitchToSingleLengthZMKs()
        Assert.AreEqual("00U913248D2781448EB99849CFFE39768AD468318", TestTran("04ED06495741C280CX2C0E3EE7A5EFFA00C3CD721FE6E66B82;ZU1", New TranslateZEKORZAKFromZMKToLMK_FK))
        Assert.AreEqual("00U9564EC9D49DE4E59BE82FE34B5CB17864BF61EA8437E3EDA", TestTran("14ED06495741C280CXFF85E926874B18C660410136CE46BD48;ZU0", New TranslateZEKORZAKFromZMKToLMK_FK))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZEKZAKFromLMKToZMK()
        SwitchToSingleLengthZMKs()
        'Both tests seem incorrect since variant is specified in the key.
        'Assert.AreEqual(TestTran("04ED06495741C280CUDBF5EFC3DBA454A827022B569E0E086B", New TranslateZEKORZAKFromLMKToZMK_FM), "00XFF85E926874B18C660410136CE46BD484BF61EA8437E3EDA")
        'Assert.AreEqual(TestTran("14ED06495741C280CUE84C6E8F364BB594D2F59F6E8A6BBBF5", New TranslateZEKORZAKFromLMKToZMK_FM), "00X2C0E3EE7A5EFFA00C3CD721FE6E66B8246831833F9855C52")
        Assert.AreEqual("00U3A67A68D3A26D46334B93F4D06D482974BF61EA8437E3EDA", TestTran("04ED06495741C280CUDBF5EFC3DBA454A827022B569E0E086B", New TranslateZEKORZAKFromLMKToZMK_FM))
        Assert.AreEqual("00U693CDDD75EE651B6102E400E70E98F9C46831833F9855C52", TestTran("14ED06495741C280CUE84C6E8F364BB594D2F59F6E8A6BBBF5", New TranslateZEKORZAKFromLMKToZMK_FM))
        SwitchToDoubleLengthZMKs()
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateMAC()
        Assert.AreEqual("00170C2BDB", TestTran("AD2EE63F23D8F733givemeSOMEMACing", New GenerateMAC_MA))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyMAC()
        Assert.AreEqual("01", TestTran("AD2EE63F23D8F733170C2BDBgivemeSOMEMACinG", New VerifyMAC_MC))
        Assert.AreEqual("00", TestTran("AD2EE63F23D8F733170C2BDBgivemeSOMEMACing", New VerifyMAC_MC))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyTranslateMAC()
        Assert.AreEqual("00D7DA46FC", TestTran("AD2EE63F23D8F73395BB142B1A349EC7170C2BDBgivemeSOMEMACing", New VerifyAndTranslateMAC_ME))
        Assert.AreEqual("01", TestTran("AD2EE63F23D8F73395BB142B1A349EC7170C2BDBgivemeSOMEMACinG", New VerifyAndTranslateMAC_ME))
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateLargeMAC()
        Assert.AreEqual("007F805A8874D3B604", TestTran("1UE84C6E8F364BB594D2F59F6E8A6BBBF5010givemeSOMEMACing", New GenerateMACForLargeMessage_MQ))
        Assert.AreEqual("00832239FEDDD43CE1", TestTran("2UE84C6E8F364BB594D2F59F6E8A6BBBF57F805A8874D3B604014givemeSOMEMOREMACing", New GenerateMACForLargeMessage_MQ))
        Assert.AreEqual("00D2BF9C1E86E5BB14", TestTran("3UE84C6E8F364BB594D2F59F6E8A6BBBF5832239FEDDD43CE1014givemeSOMEMOREMACing", New GenerateMACForLargeMessage_MQ))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromDukptoZPK()
        Assert.AreEqual("00047D26134E85E7DADF01", TestTran("U8E3D3E2FD5919657F05A1AA90D32A014U9D961F6AF1D24B38AB37B320022FF57D605FFFF9876543210E000011B9C1845EB993A7A01401234567890", New TranslatePINFromDUKPTToZPK_CI))
        Assert.AreEqual("00047D26134E85E7DADF01", TestTran("U8E3D3E2FD5919657F05A1AA90D32A014U9D961F6AF1D24B38AB37B320022FF57D605FFFF9876543210E00010D5D9638559EF53D601401234567890", New TranslatePINFromDUKPTToZPK_CI))
        Assert.AreEqual("00047D26134E85E7DADF01", TestTran("U8E3D3E2FD5919657F05A1AA90D32A014U9D961F6AF1D24B38AB37B320022FF57D605FFFF9876543210EFFC00DEFC6F09F8927B7101401234567890", New TranslatePINFromDUKPTToZPK_CI))
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromDukptToZPK3DES()
        Dim BDK As String = "0123456789ABCDEFFEDCBA9876543210"
        Dim PAN As String = "4012345678909"
        Dim PIN As String = "1234"
        Dim ZPK As String = "C7152C20BC2C830EA7EC9E8334ABE3FE"
        Dim KSN As String = "FFFF9876543210E00008"
        Dim KSNDescriptor As String = "605"

        Dim clearPB As String = Core.PIN.PINBlockFormat.ToPINBlock(PIN, PAN.Substring(0, 12), Core.PIN.PINBlockFormat.PIN_Block_Format.AnsiX98)
        Dim derivedKey As String = Cryptography.DUKPT.DerivedKey.calculateDerivedKey(New Cryptography.DUKPT.KeySerialNumber(KSN, KSNDescriptor), BDK)

        Dim cryptBDK As String = Utility.EncryptUnderLMK(BDK, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, LMKPairs.LMKPair.Pair28_29, "0")
        Dim cryptZPK As String = Utility.EncryptUnderLMK(ZPK, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, LMKPairs.LMKPair.Pair06_07, "0")

        Dim sourceEncryptedPB As String = Cryptography.TripleDES.TripleDESEncrypt(New Cryptography.HexKey(derivedKey), clearPB)
        Dim targetEncryptedPB As String = Cryptography.TripleDES.TripleDESEncrypt(New Cryptography.HexKey(ZPK), clearPB)
        Assert.AreEqual("0004" + targetEncryptedPB + "01", TestTran(cryptBDK + cryptZPK + KSNDescriptor + KSN + sourceEncryptedPB + "0101" + PAN.Substring(0, 12), New TranslatePINFromDUKPTToZPK3DES_G0()))

        'Hamdi case
        Assert.AreEqual("00", TestTran("U8E3D3E2FD5919657F05A1AA90D32A014U12E294DA05CE8761CC557F8D4786D21F7089A0003000002DD20000BF497689DC4224B200101000000000000", New TranslatePINFromDUKPTToZPK3DES_G0()).Substring(0, 2))
    End Sub

    <TestMethod()> _
    Public Sub TestValidateDukptPinWithIBMAlgorithm()
        Assert.AreEqual("00", TestTran("U8E3D3E2FD5919657F05A1AA90D32A01456C1DC7F3A899043605FFFF9876543210E000011B9C1845EB993A7A0440123456789001234567890123450004012345N99252FFFFFFFF", New VerifyDukptPINWithIBMAlgorithm_CK))
    End Sub

    <TestMethod()> _
    Public Sub TestValidateDukptPinWithVisaAlgorithm()
        Assert.AreEqual("00", TestTran("U8E3D3E2FD5919657F05A1AA90D32A014U9487FAD9CF6AF6E918BF06F71FED1415605FFFF9876543210E000011B9C1845EB993A7A04401234567808266", New VerifyDukptPINWithVISAAlgorithm_CM))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyAndGeneratePVV()
        'Successfully change PIN from 1234 to 4321 for ZPK and TPK.
        Assert.AreEqual("009860", TestTran("001U1EF828AA8F6B80EB83E19FBC373F3A85X367930344805B1FAD6146EF4ED7502B391DDDA0A7C12CFAA0155000002532113969262D1CC277EF0BB4", New VerifyAndGenerateVISAPVV_CU))
        Assert.AreEqual("009860", TestTran("002U8463435FC4B4DAA0C49025272C29B12CX367930344805B1FAD6146EF4ED7502B3028DCC093FB0471F0355000002532113969DC90438F79A7A075", New VerifyAndGenerateVISAPVV_CU))
        'Fail both above tests (wrong PVV).
        Assert.AreEqual("01", TestTran("001U1EF828AA8F6B80EB83E19FBC373F3A85X367930344805B1FAD6146EF4ED7502B391DDDA0A7C12CFAA0155000002532113961262D1CC277EF0BB4", New VerifyAndGenerateVISAPVV_CU))
        Assert.AreEqual("01", TestTran("002U8463435FC4B4DAA0C49025272C29B12CX367930344805B1FAD6146EF4ED7502B3028DCC093FB0471F0355000002532113961DC90438F79A7A075", New VerifyAndGenerateVISAPVV_CU))
    End Sub

    'Contributed by robt, http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=70958
    <TestMethod()> _
    Public Sub TestGenerateZEKAndCheckTranslation()
        AuthorizedStateOn()
        Dim ZMK As String = TestTran("0000U", New GenerateKey_A0).Substring(2, 33)
        Dim ZEKResult As String = TestTran("0" + ZMK + ";UU1", New GenerateZEKorZAK_FI)
        Dim ZEKUnderZMK As String = ZEKResult.Substring(2, 33)
        Dim ZEKUnderLMK As String = TestTran("0" + ZMK + ZEKUnderZMK + ";UU1", New TranslateZEKORZAKFromZMKToLMK_FK)
        Assert.AreEqual(ZEKResult.Substring(35, 33), ZEKUnderLMK.Substring(2, 33))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyDynamicCVV()
        ''PM12U2E774AEF908AFBA19D44D4A29AD7BD61A5338830099990279;010385338830099990279D14052216323014001072F001122330011200000
        'Dim resss As String = TestTran("12U2E774AEF908AFBA19D44D4A29AD7BD61A5338830099990279;01019" + CreateBytesWithData("5338830099990279D14052216323014001072F") + "001122330011200000", New VerifyDynamicCVV_PM())
        Assert.AreEqual("00", TestTran("12U1E6F5623CAEF7F791373A1F01A506A28A5413123556784804;00019" + CreateBytesWithData("5413123556784804D09061019005997722553F") + "0000077200005XX255", New VerifyDynamicCVV_PM))
        Assert.AreEqual("00", TestTran("12U1E6F5623CAEF7F791373A1F01A506A28A5413123556784803;00019" + CreateBytesWithData("5413123556784803D09061019005997723333F") + "0000077200005XX333", New VerifyDynamicCVV_PM))
        Assert.AreEqual("00", TestTran("12U1E6F5623CAEF7F791373A1F01A506A28A5413123556784801;00019" + CreateBytesWithData("5413123556784801D09061019005997722253F") + "0000077200028XX225", New VerifyDynamicCVV_PM))

        'In authorized mode, we want to return the dynamic CVV when the CVV verification fails.
        AuthorizedStateOn()
        Assert.AreEqual("01225", TestTran("12U1E6F5623CAEF7F791373A1F01A506A28A5413123556784801;00019" + CreateBytesWithData("5413123556784801D09061019005997722253F") + "0000077200028XX000", New VerifyDynamicCVV_PM))

        'When not in authorized mode, we want to just say that it failed.
        AuthorizedStateOff()
        Assert.AreEqual("01", TestTran("12U1E6F5623CAEF7F791373A1F01A506A28A5413123556784801;00019" + CreateBytesWithData("5413123556784801D09061019005997722253F") + "0000077200028XX000", New VerifyDynamicCVV_PM))
    End Sub

    'Get a track-II with a string representation and return a string with a byte representation.
    Private Function CreateBytesWithData(ByVal trackData As String) As String
        Dim b((trackData.Length \ 2) - 1) As Byte
        Utility.HexStringToByteArray(trackData, b)
        Return Utility.GetStringFromBytes(b)
    End Function

    <TestMethod()> _
    Public Sub TestHashDataBlock()
        'Test hashes from Wikipedia.
        TestHash("01", "The quick brown fox jumps over the lazy dog", "2fd4e1c67a2d28fced849ee1bb76e7391b93eb12".ToUpper)
        TestHash("02", "The quick brown fox jumps over the lazy dog", "9e107d9d372bb6826bd81d3542a419d6".ToUpper)
        TestHash("06", "The quick brown fox jumps over the lazy dog", "d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592".ToUpper)
        TestHash("07", "The quick brown fox jumps over the lazy dog", "ca737f1014a48f4c0b6dd43cb177b0afd9e5169367544c494011e3317dbf9a509cb1e5dc1e85a941bbee3d7f2afbc9b1".ToUpper)
        TestHash("08", "The quick brown fox jumps over the lazy dog", "07e547d9586f6a73f73fbac0435ed76951218fb7d0c8d788a309d785436bbb642e93a252a954f23912547d1e8a3b5ed6e1bfd7097821233fa0538f3db854fee6".ToUpper)
    End Sub

    'Test a hash
    Private Sub TestHash(ByVal hashID As String, ByVal data As String, ByVal expectedResult As String)
        Dim res As String = TestTran(hashID + data.Length.ToString.PadLeft(5, "0"c) + data, New HashDataBlock_GM)
        If res.Substring(0, 2) <> "00" Then
            Assert.Fail("Hash failed")
        End If

        Dim resBytes() As Byte = bytesFromString(res.Substring(2))
        Dim hexResult As String = ""
        Utility.ByteArrayToHexString(resBytes, hexResult)

        If hexResult <> expectedResult Then
            Assert.Fail("Hash failed for hash id " + hashID)
        End If
    End Sub

    'ASC-convert a string to a byte-array.
    Private Function bytesFromString(ByVal s As String) As Byte()
        Return Text.ASCIIEncoding.GetEncoding(Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage).GetBytes(s)
    End Function

    Private Function ASCIIBytesFromString(ByVal s As String) As String
        Dim ret As String = ""
        For i As Integer = 0 To s.Length - 1 Step 2
            ret = ret + Text.ASCIIEncoding.Default.GetChars(New Byte() {Convert.ToByte(s.Substring(i, 2))})
        Next
        Return ret
    End Function

    <TestMethod()> _
    Public Sub TestEchoCommand()
        'Correct - No Data
        Assert.AreEqual("00", TestTran("0000", New EchoTest_B2))
        'Correct - With Data
        Assert.AreEqual("000123456789ABCDEF", TestTran("00100123456789ABCDEF", New EchoTest_B2))
        'Length bigger than data
        Assert.AreEqual("80", TestTran("00100123456789", New EchoTest_B2))
        'Length shorter than data
        Assert.AreEqual("15", TestTran("00090123456789ABCDEF", New EchoTest_B2))
    End Sub

    <TestMethod()> _
    Public Sub TestCommandChainingBasic()
        'Correct case (without headers)
        Assert.AreEqual("00020004BF010004CZ00", TestTran("0020070BEUB13875901A7ECEDF1DBD06A5C0AED0FBBF0514F7D6A62A4401020000054301785040061CYB62C0A61BEAB9D15A31A02F7CCBCC8291713500200000543012;0413526", New CommandChaining_NK))
        'Correct case (with headers)
        Assert.AreEqual("00020008AAAABF010008BBBBCZ00", TestTran("1020074AAAABEUB13875901A7ECEDF1DBD06A5C0AED0FBBF0514F7D6A62A4401020000054301785040065BBBBCYB62C0A61BEAB9D15A31A02F7CCBCC8291713500200000543012;0413526", New CommandChaining_NK))
        'Invalid number of commands field
        Assert.AreEqual("52", TestTran("000", New CommandChaining_NK))
        'Incorrect sub command count
        Assert.AreEqual("15", TestTran("0010070BEUB13875901A7ECEDF1DBD06A5C0AED0FBBF0514F7D6A62A4401020000054301785040061CYB62C0A61BEAB9D15A31A02F7CCBCC8291713500200000543012;0413526", New CommandChaining_NK))
        'Incorrect sub command length
        Assert.AreEqual("15", TestTran("0020071BEUB13875901A7ECEDF1DBD06A5C0AED0FBBF0514F7D6A62A4401020000054301785040061CYB62C0A61BEAB9D15A31A02F7CCBCC8291713500200000543012;0413526", New CommandChaining_NK))
        'With Has headers setted but commands have no headers
        'Assert.AreEqual("00020008BEUB14670008CYB62D67", TestTran("1020070BEUB13875901A7ECEDF1DBD06A5C0AED0FBBF0514F7D6A62A4401020000054301785040061CYB62C0A61BEAB9D15A31A02F7CCBCC8291713500200000543012;0413526", New CommandChaining_NK))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyTerminalPINUsingComparisonMethod()
        Dim PAN As String = "5044070000253211"
        Dim PINResult As String = TestTran(PAN.Substring(PAN.Length - 12 - 1, 12), New GenerateRandomPIN_JA)
        Assert.AreEqual(PINResult.Substring(0, 2), "00")

        'Randomizer based on clock, so sleep a bit to get a different PIN.
        System.Threading.Thread.Sleep(25)

        Dim PINResult2 As String = TestTran(PAN.Substring(PAN.Length - 12 - 1, 12), New GenerateRandomPIN_JA)
        Assert.AreEqual(PINResult.Substring(0, 2), "00")

        Dim PIN As String = PINResult.Substring(2)
        Dim PIN2 As String = PINResult2.Substring(2)
        Dim PINBlock As String = Core.PIN.PINBlockFormat.ToPINBlock(PIN, PAN, Core.PIN.PINBlockFormat.PIN_Block_Format.Diebold)
        Dim ClearTPK As String = "99000ECE70A9FEA82AF9EA4B3B5B0495", cryptTPK As String = "UBB7A05C1CC51C1B7242626495B8A93D4"
        Dim cryptPINBlock As String = Core.Cryptography.TripleDES.TripleDESEncrypt(New Core.Cryptography.HexKey(ClearTPK), PINBlock)

        'Verification OK.
        Assert.AreEqual("00", TestTran(cryptTPK + cryptPINBlock + Core.PIN.PINBlockFormat.FromPINBlockFormat(Core.PIN.PINBlockFormat.PIN_Block_Format.Diebold) + PAN.Substring(PAN.Length - 12 - 1, 12) + PIN, New VerifyTerminalPinUsingComparisonMethod_BC))
        'Invalid PIN block.
        Assert.AreEqual("23", TestTran(cryptTPK + cryptPINBlock + "99" + PAN.Substring(PAN.Length - 12 - 1, 12) + PIN, New VerifyTerminalPinUsingComparisonMethod_BC))
        'Verification failure.
        Assert.AreEqual("01", TestTran(cryptTPK + cryptPINBlock + Core.PIN.PINBlockFormat.FromPINBlockFormat(Core.PIN.PINBlockFormat.PIN_Block_Format.Diebold) + PAN.Substring(PAN.Length - 12 - 1, 12) + PIN2, New VerifyTerminalPinUsingComparisonMethod_BC))
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyInterchangePINUsingComparisonMethod()
        Dim PAN As String = "5044070000253211"
        Dim PINResult As String = TestTran(PAN.Substring(PAN.Length - 12 - 1, 12), New GenerateRandomPIN_JA)
        Assert.AreEqual(PINResult.Substring(0, 2), "00")

        'Randomizer based on clock, so sleep a bit to get a different PIN.
        System.Threading.Thread.Sleep(25)

        Dim PINResult2 As String = TestTran(PAN.Substring(PAN.Length - 12 - 1, 12), New GenerateRandomPIN_JA)
        Assert.AreEqual(PINResult.Substring(0, 2), "00")

        Dim PIN As String = PINResult.Substring(2)
        Dim PIN2 As String = PINResult2.Substring(2)
        Dim PINBlock As String = Core.PIN.PINBlockFormat.ToPINBlock(PIN, PAN, Core.PIN.PINBlockFormat.PIN_Block_Format.Diebold)
        Dim ClearZPK As String = "99000ECE70A9FEA82AF9EA4B3B5B0495", cryptZPK As String = "U402F396F7ABEDC14976EB65959AA99B2"
        Dim cryptPINBlock As String = Core.Cryptography.TripleDES.TripleDESEncrypt(New Core.Cryptography.HexKey(ClearZPK), PINBlock)

        'Verification OK.
        Assert.AreEqual("00", TestTran(cryptZPK + cryptPINBlock + Core.PIN.PINBlockFormat.FromPINBlockFormat(Core.PIN.PINBlockFormat.PIN_Block_Format.Diebold) + PAN.Substring(PAN.Length - 12 - 1, 12) + PIN, New VerifyInterchangePinUsingComparisonMethod_BE))
        'Verification failure.
        Assert.AreEqual("01", TestTran(cryptZPK + cryptPINBlock + Core.PIN.PINBlockFormat.FromPINBlockFormat(Core.PIN.PINBlockFormat.PIN_Block_Format.Diebold) + PAN.Substring(PAN.Length - 12 - 1, 12) + PIN2, New VerifyInterchangePinUsingComparisonMethod_BE))
    End Sub

    <TestMethod()> _
    Public Sub TranslateBDKFromZMKToLMK()
        Assert.AreEqual("00U8E3D3E2FD5919657F05A1AA90D32A01408D7B4", TestTran("U1457FF6ADF6250C66C368416B4C9D3836A2C67C227784BC5D8508B6BED82ECB8;0U1", New TranslateBDKFromZMKToLMK_DW))
    End Sub

    <TestMethod()> _
    Public Sub TranslateBDKFromLMKToZMK()
        Assert.AreEqual("00X6A2C67C227784BC5D8508B6BED82ECB808D7B4", TestTran("U1457FF6ADF6250C66C368416B4C9D383U8E3D3E2FD5919657F05A1AA90D32A014;X01", New TranslateBDKFromLMKToZMK_DY))
    End Sub

    <TestMethod()> _
    Public Sub EncryptClearPIN()
        Dim clearPIN As String = "1234"
        Dim clearTPK As String = "D3DCC7EA9BCB755D254620B376B3D007"
        Dim cryptTPK As String = "U8463435FC4B4DAA0C49025272C29B12C"
        Dim cryptPVK As String = "UA8B1520E201412938388191885FFA50A"
        Dim PAN As String = "5044070000253211"

        'Test for clear PIN padded with Fs (http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=239725).
        Assert.AreEqual("00" + "01234", TestTran("1234F" + PAN.Substring(3, 12), New EncryptClearPIN_BA))

        'Generate the "encrypted" PIN.
        Assert.AreEqual("000" + clearPIN, TestTran("0" + clearPIN + PAN.Substring(3, 12), New EncryptClearPIN_BA))

        'Generate a PVV for the "encrypted PIN"
        Dim res As String = TestTran(cryptPVK + "0" + clearPIN + PAN.Substring(3, 12) + "1" + ";", New GenerateVISAPVV_DG)
        If res.Substring(0, 2) <> ErrorCodes.ER_00_NO_ERROR Then
            Assert.Fail("Error duging PVV generation.")
        End If

        'Get the PVV.
        Dim PVV As String = res.Substring(2)

        'Create a simple PIN block using the clear PIN.
        Dim PB As String = Cryptography.TripleDES.TripleDESEncrypt(New Cryptography.HexKey(clearTPK), clearPIN + New String("F"c, 12))

        'Throw a Visa PVV verification against all this.
        Assert.AreEqual("00", TestTran(cryptTPK + cryptPVK + PB + "03" + PAN.Substring(3, 12) + "1" + PVV, New VerifyTerminalPINWithVISAAlgorithm_DC))
    End Sub

    <TestMethod()> _
    Public Sub GenerateMAC()
        Assert.AreEqual("0000085F1448EE", TestTran("01032003U19D25F349FD03CC3556BB05F65283CA10000000000000000004072C29C2371CC9BDB65B779B8E8D37B29ECC154AA56A8799FAE2F498F76ED92F2", New GenerateMAC_M6))
    End Sub

    <TestMethod()> _
    Public Sub VerifyMAC()
        Assert.AreEqual("00", TestTran("01032003U19D25F349FD03CC3556BB05F65283CA10000000000000000004072C29C2371CC9BDB65B779B8E8D37B29ECC154AA56A8799FAE2F498F76ED92F25F1448EE", New VerifyMAC_M8))
    End Sub

    <TestMethod()> _
    Public Sub VerifyTruncatedApplicationCryptogram()
        Dim tranData As String = "000000000000000000000000000080000000000000000000005344657800000007800001240000"
        Dim tranDataLength As String = ""
        Utility.ByteArrayToHexString(New Byte() {Convert.ToByte(tranData.Length / 2)}, tranDataLength)
        Assert.AreEqual("00", TestTran("00001UA67981CE67F36A4BD2DE46BA17D6F59608" + Utility.toBCD("6414000000036701") + ASCIIBytesFromString("0007") + ASCIIBytesFromString("53446578") + tranDataLength + ASCIIBytesFromString(tranData) + ";" + CreateBytesWithData("1D8F14549EDED2D6") + CreateBytesWithData("0000000000000000") + CreateBytesWithData("00000000"), New VerifyTruncatedApplicationCryptogram_K2))
        AuthorizedStateOff()
        Assert.AreEqual("01", TestTran("00001UA67981CE67F36A4BD2DE46BA17D6F59608" + Utility.toBCD("6414000000036701") + ASCIIBytesFromString("0007") + ASCIIBytesFromString("53446578") + tranDataLength + ASCIIBytesFromString(tranData) + ";" + CreateBytesWithData("1D8F14549EDED2D7") + CreateBytesWithData("0000000000000000") + CreateBytesWithData("00000000"), New VerifyTruncatedApplicationCryptogram_K2))
        AuthorizedStateOn()
        Assert.AreEqual("01" + CreateBytesWithData("1D8F14549EDED2D6"), TestTran("00001UA67981CE67F36A4BD2DE46BA17D6F59608" + Utility.toBCD("6414000000036701") + ASCIIBytesFromString("0007") + ASCIIBytesFromString("53446578") + tranDataLength + ASCIIBytesFromString(tranData) + ";" + CreateBytesWithData("1D8F14549EDED2D7") + CreateBytesWithData("0000000000000000") + CreateBytesWithData("00000000"), New VerifyTruncatedApplicationCryptogram_K2))
        AuthorizedStateOff()
    End Sub

    <TestMethod()> _
    Public Sub GenerateAndVerifyIBMPin()
        Dim cryptPVK As String = "UA8B1520E201412938388191885FFA50A"
        Dim cryptZPK As String = "U402F396F7ABEDC14976EB65959AA99B2"
        Dim acct As String = "832937216759"
        Dim decTable As String = "FFFFFFFFFFFFFFFF"
        Dim pinValData As String = "4458329372N3"
        Dim offset As String = "0000FFFFFFFF"

        'Derive the PIN encrypted under LMK.
        Dim result As String = TestTran(cryptPVK + offset + "04" + acct + decTable + pinValData, New DerivePINUsingTheIBMMethod_EE())
        If Not result.StartsWith("00") Then
            Assert.Fail("Error deriving IBM PIN")
        End If

        'Create a PIN block under the ZPK.
        result = TestTran(cryptZPK + "01" + acct + result.Substring(2), New TranslatePINFromLMKToZPK_JG())
        If Not result.StartsWith("00") Then
            Assert.Fail("Error translating PIN from LMK to ZPK encryption")
        End If

        'Verify the PIN.
        Assert.AreEqual("00", TestTran(cryptZPK + cryptPVK + "12" + result.Substring(2) + "01" + "04" + acct + decTable + pinValData + offset, New VerifyInterchangePINWithIBMAlgorithm_EA()).Substring(0, 2))
    End Sub

    <TestMethod()> _
    Public Sub DecryptEncryptedPIN()
        Assert.AreEqual("001234F", TestTran("12345678901201234", New DecryptEncryptedPIN_NG()))
    End Sub

    <TestMethod()> _
    Public Sub TranslateFromOldToNewStorage()
        Assert.AreEqual("00UDBA4FF8A2DC7386947EC21165DE0F728", TestTran("021U9431CB85DB136EA608B44528E50C68E5", New TranslateKeysFromOldLMKToNewLMK_BW()))
        Assert.AreEqual("00X0FDC679935C438AAC1214C40F5B59FA2", TestTran("021U9431CB85DB136EA608B44528E50C68E5;002;0X0", New TranslateKeysFromOldLMKToNewLMK_BW()))
    End Sub

    'Dump major events to the console window.
    Private Sub o_MajorLogEvent(ByVal sender As Core.ThalesMain, ByVal s As String) Handles o.MajorLogEvent
        Console.WriteLine(s)
    End Sub

    'Dump minor events to the console window.
    Private Sub o_MinorLogEvent(ByVal sender As Core.ThalesMain, ByVal s As String) Handles o.MinorLogEvent
        Console.WriteLine(s)
    End Sub

    'Switches the simulator to using single-length zone master keys.
    Private Sub SwitchToSingleLengthZMKs()
        Resources.UpdateResource(Resources.DOUBLE_LENGTH_ZMKS, False)
        ClearMessageFieldStoreStore()
    End Sub

    'Switches the simulator to using double-length zone master keys.
    Private Sub SwitchToDoubleLengthZMKs()
        Resources.UpdateResource(Resources.DOUBLE_LENGTH_ZMKS, True)
        ClearMessageFieldStoreStore()
    End Sub

    'Put the simulator in the authorized state.
    Private Sub AuthorizedStateOn()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
    End Sub

    'Put the simulator out of the authorized state.
    Private Sub AuthorizedStateOff()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    'Put the simulator in legacy mode.
    Private Sub LegacyModeOn()
        Resources.UpdateResource(Resources.LEGACY_MODE, True)
    End Sub

    'Put the simulator out of legacy mode.
    Private Sub LegacyModeOff()
        Resources.UpdateResource(Resources.LEGACY_MODE, False)
    End Sub

    'Put the simulator to expect trailers.
    Private Sub ExpectTrailersOn()
        Resources.UpdateResource(Resources.EXPECT_TRAILERS, True)
    End Sub

    'Put the simulator to not expect trailers.
    Private Sub ExpectTrailersOff()
        Resources.UpdateResource(Resources.EXPECT_TRAILERS, False)
    End Sub

    'Determine whether we're expecting trailers or not.
    Private Function ExpectTrailers() As Boolean
        Return CType(Resources.GetResource(Resources.EXPECT_TRAILERS), Boolean)
    End Function

    Private Sub ClearMessageFieldStoreStore()
        'We're clearing the message fields store because there are definitions
        'that are based on the value of internal variables (such as the allow double ZMKs).
        'Normally these values do not change when the simulator is running but they
        'do change when we run the tests.
        Core.Message.XML.MessageFieldsStore.Clear()
    End Sub

End Class
