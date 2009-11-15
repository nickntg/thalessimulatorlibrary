Imports System
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ThalesSim.Core
Imports ThalesSim.Core.HostCommands
Imports ThalesSim.Core.HostCommands.BuildIn
Imports ThalesSim.Core.Message

<TestClass()> Public Class HostCommandTests

    Private o As ThalesSim.Core.ThalesMain

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

        HC.AcceptMessage(msg)
        retMsg = HC.ConstructResponse()
        HC.Terminate()
        HC = Nothing
        Return retMsg.MessageData()
    End Function

    <TestMethod()> _
    Public Sub TestCancelAuthState()
        Assert.AreEqual(TestTran("", New CancelAuthState_RA), "00")
    End Sub

    <TestMethod()> _
    Public Sub TestSetHSMDelay()
        Assert.AreEqual(TestTran("001", New SetHSMDelay_LG), "00")
    End Sub

    <TestMethod()> _
    Public Sub TestHSMStatus()
        Assert.AreEqual(TestTran("00", New HSMStatus_NO).Substring(0, 2), "00")
    End Sub

    <TestMethod()> _
    Public Sub TestExportKey()
        Assert.AreEqual(TestTran("0024ED06495741C280C0406FBB23A5214DFZ", New ExportKey_A8), "0035ED0C0EA7F7D0FA0035BB")
        Assert.AreEqual(TestTran("002U1457FF6ADF6250C66C368416B4C9D3837BB126F2BE631486Z", New ExportKey_A8), "0016224FDAA779AFB31FFD3C")
        Assert.AreEqual(TestTran("002U1457FF6ADF6250C66C368416B4C9D383U8463435FC4B4DAA0C49025272C29B12CU", New ExportKey_A8), "00U2C62A23D001B97412950CD8BE66C7639070753")
    End Sub

    <TestMethod()> _
    Public Sub TestFormKeyFromEncryptedComponents()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
        Assert.AreEqual(TestTran("3002Z3B723AF4CF00A7A6954111D254A90D17EAAF49979FA95742", New FormKeyFromEncryptedComponents_A4), "00FE018240022A76DCA192FE")
        Assert.AreEqual(TestTran("3000XX2EC8A0412B5D0E86E3C1E5ABFA19B3F5XFF43378ED5D85B1BC465BF000335FBF1XA235EDF4C58A2CB0C84641D07319CF21", New FormKeyFromEncryptedComponents_A4), "00XC0BC1DFFC449A402DAB71250CA5869CC8CE396")
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    <TestMethod()> _
    Public Sub TestFormZMKFrom3EncryptedComponents()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
        Assert.AreEqual(TestTran("A235EDF4C58A2CB0C84641D07319CF21FF43378ED5D85B1BC465BF000335FBF12EC8A0412B5D0E86E3C1E5ABFA19B3F5", New FormZMKFromThreeComponents_GG), "00XC0BC1DFFC449A402DAB71250CA5869CC8CE39643DA9A9B99")
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateCheckValueBU()
        Assert.AreEqual(TestTran("0200406FBB23A5214DF", New GenerateCheckValue_BU), "000035BBE340A4B763")
        Assert.AreEqual(TestTran("000ACE9B8A0BE50C09B", New GenerateCheckValue_BU), "004BD5E2482582C2C4")
        Assert.AreEqual(TestTran("001U93CB819F8FEE4F78BF9C4CDD84750DB1", New GenerateCheckValue_BU), "00B70AD25C94548822")
        Assert.AreEqual(TestTran("011U1EF828AA8F6B80EB83E19FBC373F3A85", New GenerateCheckValue_BU), "006F1E3F74F826B7EB")
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateCheckValueKA()
        Assert.AreEqual(TestTran("0406FBB23A5214DF02", New GenerateCheckValue_KA), "000035BBE340A4B763")
        Assert.AreEqual(TestTran("0406FBB23A5214DF02;ZZ1", New GenerateCheckValue_KA), "000035BB")
        Assert.AreEqual(TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8501", New GenerateCheckValue_KA), "006F1E3F74F826B7EB")
        Assert.AreEqual(TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8501;ZZ1", New GenerateCheckValue_KA), "006F1E3F")
    End Sub

    <TestMethod()> _
    Public Sub TestImportKey()
        Assert.AreEqual(TestTran("0024ED06495741C280C35ED0C0EA7F7D0FAZ", New ImportKey_A6), "000406FBB23A5214DF0035BB")
        Assert.AreEqual(TestTran("001U1457FF6ADF6250C66C368416B4C9D3832B930A07119F93A8Z", New ImportKey_A6), "00BAB32D775A38E4AB73936E")
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateVISAPVV()
        Assert.AreEqual(TestTran("X367930344805B1FAD6146EF4ED7502B3012345500000253211", New GenerateVISAPVV_DG), "003969")
        Assert.AreEqual(TestTran("367930344805B1FAD6146EF4ED7502B3012345500000253211", New GenerateVISAPVV_DG), "003969")
        Assert.AreEqual(TestTran("U183DF6EA5EDFF7D5C91C8F2BA6451884012345500000253211", New GenerateVISAPVV_DG), "000550")
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyTerminalPINUsingVISAAlgorithm()
        Assert.AreEqual(TestTran("0406FBB23A5214DFX367930344805B1FAD6146EF4ED7502B3AE7A708F877571A90155000002532113969", New VerifyTerminalPINWithVISAAlgorithm_DC), "00")
        Assert.AreEqual(TestTran("0406FBB23A5214DF367930344805B1FAD6146EF4ED7502B3AE7A708F877571A90155000002532113969", New VerifyTerminalPINWithVISAAlgorithm_DC), "00")
        Assert.AreEqual(TestTran("0406FBB23A5214DF367930344805B1FAD6146EF4ED7502B3AE7A708F877571A90155000002532113962", New VerifyTerminalPINWithVISAAlgorithm_DC), "01")
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12CX367930344805B1FAD6146EF4ED7502B3028DCC093FB0471F0355000002532113969", New VerifyTerminalPINWithVISAAlgorithm_DC), "00")
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12CX367930344805B1FAD6146EF4ED7502B3028DCC093FB0471F0355000002532113962", New VerifyTerminalPINWithVISAAlgorithm_DC), "01")
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyInterchangePINUsingVISAAlgorithm()
        Assert.AreEqual(TestTran("BAB32D775A38E4ABX367930344805B1FAD6146EF4ED7502B3F7808F2CBEC631680355000002532113969", New VerifyInterchangePINWithVISAAlgorithm_EC), "00")
        Assert.AreEqual(TestTran("BAB32D775A38E4ABX367930344805B1FAD6146EF4ED7502B3F7808F2CBEC631680355000002532113962", New VerifyInterchangePINWithVISAAlgorithm_EC), "01")
        Assert.AreEqual(TestTran("U1EF828AA8F6B80EB83E19FBC373F3A85X367930344805B1FAD6146EF4ED7502B391DDDA0A7C12CFAA0155000002532113969", New VerifyInterchangePINWithVISAAlgorithm_EC), "00")
        Assert.AreEqual(TestTran("U1EF828AA8F6B80EB83E19FBC373F3A85X367930344805B1FAD6146EF4ED7502B391DDDA0A7C12CFAA0155000002532113962", New VerifyInterchangePINWithVISAAlgorithm_EC), "01")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromVISAToThales()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
        Assert.AreEqual(TestTran("55000002532101234", New TranslatePINFromVISAToThales_BQ), "00001234")
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromOneZPKToAnother()
        Assert.AreEqual(TestTran("BAB32D775A38E4ABU1EF828AA8F6B80EB83E19FBC373F3A8512F7808F2CBEC631680303550000025321", New TranslatePINFromZPKToZPK_CC), "000482206CCD872229C203")
        Assert.AreEqual(TestTran("BAB32D775A38E4ABU1EF828AA8F6B80EB83E19FBC373F3A8512F7808F2CBEC631680301550000025321", New TranslatePINFromZPKToZPK_CC), "000491DDDA0A7C12CFAA01")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromTPKToLMK()
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12C028DCC093FB0471F03550000025321", New TranslatePINFromTPKToLMK_JC), "0001234")
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12C6428EB94035AF53B01550000025321", New TranslatePINFromTPKToLMK_JC), "0001234")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromLMKToZPK()
        Assert.AreEqual(TestTran("BAB32D775A38E4AB0155000002532101234", New TranslatePINFromLMKToZPK_JG), "00E98FFDA17099AF55")
        Assert.AreEqual(TestTran("BAB32D775A38E4AB0355000002532101234", New TranslatePINFromLMKToZPK_JG), "00F7808F2CBEC63168")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromZPKToLMK()
        Assert.AreEqual(TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8591DDDA0A7C12CFAA01550000025321", New TranslatePINFromZPKToLMK_JE), "0001234")
        Assert.AreEqual(TestTran("U1EF828AA8F6B80EB83E19FBC373F3A8582206CCD872229C203550000025321", New TranslatePINFromZPKToLMK_JE), "0001234")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslatePINFromTPKToZMK()
        Assert.AreEqual(TestTran("0406FBB23A5214DFBAB32D775A38E4AB12DC80B186C30902B00303550000025321", New TranslatePINFromTPKToZPK_CA), "0004F7808F2CBEC6316803")
        Assert.AreEqual(TestTran("0406FBB23A5214DFBAB32D775A38E4AB12DC80B186C30902B00301550000025321", New TranslatePINFromTPKToZPK_CA), "0004E98FFDA17099AF5501")
        Assert.AreEqual(TestTran("0406FBB23A5214DFBAB32D775A38E4AB12AE7A708F877571A90101550000025321", New TranslatePINFromTPKToZPK_CA), "0004E98FFDA17099AF5501")
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12CU1EF828AA8F6B80EB83E19FBC373F3A85126428EB94035AF53B0101550000025321", New TranslatePINFromTPKToZPK_CA), "000491DDDA0A7C12CFAA01")
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12CU1EF828AA8F6B80EB83E19FBC373F3A85126428EB94035AF53B0103550000025321", New TranslatePINFromTPKToZPK_CA), "000482206CCD872229C203")
    End Sub

    <TestMethod()> _
    Public Sub TestFormZMKFromTwoToNineComponents()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
        Assert.AreEqual(TestTran("32EC8A0412B5D0E86E3C1E5ABFA19B3F5FF43378ED5D85B1BC465BF000335FBF1A235EDF4C58A2CB0C84641D07319CF21", New FormZMKFromTwoToNineComponents_GY), "00C0BC1DFFC449A402DAB71250CA5869CC8CE39643DA9A9B99")
        Assert.AreEqual(TestTran("32EC8A0412B5D0E86E3C1E5ABFA19B3F5FF43378ED5D85B1BC465BF000335FBF1A235EDF4C58A2CB0C84641D07319CF21;0U1", New FormZMKFromTwoToNineComponents_GY), "00U369835189A058604EB7F84EAE10C7D048CE396")
        Assert.AreEqual(TestTran("32EC8A0412B5D0E86E3C1E5ABFA19B3F5FF43378ED5D85B1BC465BF000335FBF1A235EDF4C58A2CB0C84641D07319CF21;0X0", New FormZMKFromTwoToNineComponents_GY), "00XC0BC1DFFC449A402DAB71250CA5869CC8CE39643DA9A9B99")
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZMKFromZMKToLMK()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
        Assert.AreEqual(TestTran("2EC8A0412B5D0E86E3C1E5ABFA19B3F579E1B5D8DC672AF00137070260B5FA8E;XU0", New TranslateZMKFromZMKToLMK_BY), "00UF673F2E0149686A7365E73B881152B9713F44F34A77D8263")
        Assert.AreEqual(TestTran("2EC8A0412B5D0E86E3C1E5ABFA19B3F579E1B5D8DC672AF00137070260B5FA8E", New TranslateZMKFromZMKToLMK_BY), "00FF43378ED5D85B1BC465BF000335FBF113F44F34A77D8263")
        Assert.AreEqual(TestTran("2EC8A0412B5D0E86E3C1E5ABFA19B3F579E1B5D8DC672AF0B2E2357CA57E5705", New TranslateZMKFromZMKToLMK_BY), "01FF43378ED5D85B1BC465BF000335FBF113F44F34A77D8263")
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZPKFromZMKToLMK()
        Assert.AreEqual(TestTran("U1457FF6ADF6250C66C368416B4C9D3832B930A07119F93A8", New TranslateZPKFromZMKToLMK_FA), "00BAB32D775A38E4AB73936E441E46819D")
        Assert.AreEqual(TestTran("U1457FF6ADF6250C66C368416B4C9D3832B930A07119F93A8;XZ1", New TranslateZPKFromZMKToLMK_FA), "00BAB32D775A38E4AB73936E")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZPKFromLMKToZMK()
        Assert.AreEqual(TestTran("U1457FF6ADF6250C66C368416B4C9D383BAB32D775A38E4AB", New TranslateZPKFromLMKToZMK_GC), "002B930A07119F93A873936E441E46819D")
        Assert.AreEqual(TestTran("U1457FF6ADF6250C66C368416B4C9D383BAB32D775A38E4AB;ZZ1", New TranslateZPKFromLMKToZMK_GC), "002B930A07119F93A873936E")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTMKFromLMKToTMK()
        Assert.AreEqual(TestTran("7BB126F2BE6314860406FBB23A5214DF", New TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE), "00A341A0CC5F71B229")
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12CU28DDAEC83617D2F6E2302928B28A54D0", New TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE), "00XE098159ADAD90802CF32ED579CE48557")
        Assert.AreEqual(TestTran("U8463435FC4B4DAA0C49025272C29B12C0406FBB23A5214DF", New TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE), "00508949F68B4060A4")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTMKFromZMKToLMK()
        Assert.AreEqual(TestTran("4ED06495741C280C35ED0C0EA7F7D0FA", New TranslateTMPTPKPVKFromZMKToLMK_FC), "000406FBB23A5214DF0035BBE340A4B763")
        Assert.AreEqual(TestTran("4ED06495741C280C35ED0C0EA7F7D0FA;ZZ1", New TranslateTMPTPKPVKFromZMKToLMK_FC), "000406FBB23A5214DF0035BB")
        Assert.AreEqual(TestTran("U1457FF6ADF6250C66C368416B4C9D38355FF9012A3854818", New TranslateTMPTPKPVKFromZMKToLMK_FC), "000406FBB23A5214DF0035BBE340A4B763")
        Assert.AreEqual(TestTran("U1457FF6ADF6250C66C368416B4C9D383XF7D53991678347EFB3026882F724E6EE;XU1", New TranslateTMPTPKPVKFromZMKToLMK_FC), "00U8463435FC4B4DAA0C49025272C29B12C070753")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTMKFromLMKToZMK()
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, True)
        Assert.AreEqual(TestTran("4ED06495741C280C0406FBB23A5214DF", New TranslateTMKTPKPVKFromLMKToZMK_FE), "0035ED0C0EA7F7D0FA0035BBE340A4B763")
        Assert.AreEqual(TestTran("4ED06495741C280C0406FBB23A5214DF;ZZ1", New TranslateTMKTPKPVKFromLMKToZMK_FE), "0035ED0C0EA7F7D0FA0035BB")
        Assert.AreEqual(TestTran("U1457FF6ADF6250C66C368416B4C9D383U8463435FC4B4DAA0C49025272C29B12C;XX1", New TranslateTMKTPKPVKFromLMKToZMK_FE), "00XF7D53991678347EFB3026882F724E6EE070753")
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, False)
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTAKFromZMKToLMK()
        Assert.AreEqual(TestTran("4ED06495741C280CE6CF9DB3EC5D766F", New TranslateTAKFromZMKToLMK_MI), "00AD2EE63F23D8F733EDFE6926B3B9D27C")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTAKFromLMKToZMK()
        Assert.AreEqual(TestTran("4ED06495741C280CAD2EE63F23D8F733", New TranslateTAKFromLMKToZMK_MG), "00E6CF9DB3EC5D766FEDFE6926B3B9D27C")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateTAKFromLMKToTMK()
        Assert.AreEqual(TestTran("0406FBB23A5214DFAD2EE63F23D8F733", New TranslateTAKFromLMKToTMK_AG), "00D3A9103B524C49ACEDFE6926B3B9D27C")
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateCVV()
        Assert.AreEqual(TestTran("U9B4934384B19946B040CD702B4D581454123456789012345;8701101", New GenerateVISACVV_CW), "00561")
        Assert.AreEqual(TestTran("0A61E674E88C6A7EEABC38C2B2BB492F4123456789012345;8701101", New GenerateVISACVV_CW), "00561")
        Assert.AreEqual(TestTran("0A61E674E88C6A7EEABC38C2B2BB492F4999988887777;9105111", New GenerateVISACVV_CW), "00649")
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyCVV()
        Assert.AreEqual(TestTran("U9B4934384B19946B040CD702B4D581455614123456789012345;8701101", New VerifyVISACVV_CY), "00")
        Assert.AreEqual(TestTran("U9B4934384B19946B040CD702B4D581451114123456789012345;8701101", New VerifyVISACVV_CY), "01")
        Assert.AreEqual(TestTran("0A61E674E88C6A7EEABC38C2B2BB492F5614123456789012345;8701101", New VerifyVISACVV_CY), "00")
        Assert.AreEqual(TestTran("0A61E674E88C6A7EEABC38C2B2BB492F6494999988887777;9105111", New VerifyVISACVV_CY), "00")
        Assert.AreEqual(TestTran("0A61E674E88C6A7EEABC38C2B2BB492F1114999988887777;9105111", New VerifyVISACVV_CY), "01")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateCVKFromZMKToLMK()
        Assert.AreEqual(TestTran("4ED06495741C280CAB88EE604522372FDAA27A67A8CDADFA;000", New TranslateCVKFromZMKToLMK_AW), "000A61E674E88C6A7EEABC38C2B2BB492FD5D44FA68CDC")
        Assert.AreEqual(TestTran("4ED06495741C280CAB88EE604522372FDAA27A67A8CDADFA;001", New TranslateCVKFromZMKToLMK_AW), "000A61E674E88C6A7EEABC38C2B2BB492F08D7B4")
    End Sub

    <TestMethod()> _
    Public Sub TranslateCVKFromLMKToZMK()
        Assert.AreEqual(TestTran("4ED06495741C280C0A61E674E88C6A7EEABC38C2B2BB492F", New TranslateCVKFromLMKToZMK_AU), "00AB88EE604522372FDAA27A67A8CDADFAD5D44FA68CDC")
        Assert.AreEqual(TestTran("4ED06495741C280C0A61E674E88C6A7EEABC38C2B2BB492F;ZU1", New TranslateCVKFromLMKToZMK_AU), "00AB88EE604522372FDAA27A67A8CDADFA08D7B4")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZEKZAKFromZMKToLMK()
        Assert.AreEqual(TestTran("04ED06495741C280CX2C0E3EE7A5EFFA00C3CD721FE6E66B82;ZU1", New TranslateZEKORZAKFromZMKToLMK_FK), "00U913248D2781448EB99849CFFE39768AD468318")
        Assert.AreEqual(TestTran("14ED06495741C280CXFF85E926874B18C660410136CE46BD48;ZU0", New TranslateZEKorZAKFromZMKToLMK_FK), "00U9564EC9D49DE4E59BE82FE34B5CB17864BF61EA8437E3EDA")
    End Sub

    <TestMethod()> _
    Public Sub TestTranslateZEKZAKFromLMKToZMK()
        Assert.AreEqual(TestTran("04ED06495741C280CUDBF5EFC3DBA454A827022B569E0E086B", New TranslateZEKORZAKFromLMKToZMK_FM), "00XFF85E926874B18C660410136CE46BD484BF61EA8437E3EDA")
        Assert.AreEqual(TestTran("14ED06495741C280CUE84C6E8F364BB594D2F59F6E8A6BBBF5", New TranslateZEKOrZAKFromLMKToZMK_FM), "00X2C0E3EE7A5EFFA00C3CD721FE6E66B8246831833F9855C52")
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateMAC()
        Assert.AreEqual(TestTran("AD2EE63F23D8F733givemeSOMEMACing", New GenerateMAC_MA), "00170C2BDB")
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyMAC()
        Assert.AreEqual(TestTran("AD2EE63F23D8F733170C2BDBgivemeSOMEMACinG", New VerifyMAC_MC), "01")
        Assert.AreEqual(TestTran("AD2EE63F23D8F733170C2BDBgivemeSOMEMACing", New VerifyMAC_MC), "00")
    End Sub

    <TestMethod()> _
    Public Sub TestVerifyTranslateMAC()
        Assert.AreEqual(TestTran("AD2EE63F23D8F73395BB142B1A349EC7170C2BDBgivemeSOMEMACing", New VerifyAndTranslateMAC_ME), "00D7DA46FC")
        Assert.AreEqual(TestTran("AD2EE63F23D8F73395BB142B1A349EC7170C2BDBgivemeSOMEMACinG", New VerifyAndTranslateMAC_ME), "01")
    End Sub

    <TestMethod()> _
    Public Sub TestGenerateLargeMAC()
        Assert.AreEqual(TestTran("1UE84C6E8F364BB594D2F59F6E8A6BBBF5010givemeSOMEMACing", New GenerateMACForLargeMessage_MQ), "007F805A8874D3B604")
        Assert.AreEqual(TestTran("2UE84C6E8F364BB594D2F59F6E8A6BBBF57F805A8874D3B604014givemeSOMEMOREMACing", New GenerateMACForLargeMessage_MQ), "00832239FEDDD43CE1")
        Assert.AreEqual(TestTran("3UE84C6E8F364BB594D2F59F6E8A6BBBF5832239FEDDD43CE1014givemeSOMEMOREMACing", New GenerateMACForLargeMessage_MQ), "00D2BF9C1E86E5BB14")
    End Sub

    'Contributed by robt, http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=70958
    <TestMethod()> _
    Public Sub TestGenerateZEKAndCheckTranslation()
        Dim ZMK As String = TestTran("0000U", New GenerateKey_A0).Substring(2, 33)
        Dim ZEKResult As String = TestTran("0" + ZMK + ";UU1", New GenerateZEKorZAK_FI)
        Dim ZEKUnderZMK As String = ZEKResult.Substring(2, 33)
        Dim ZEKUnderLMK As String = TestTran("0" + ZMK + ZEKUnderZMK + ";UU1", New TranslateZEKORZAKFromZMKToLMK_FK)
        Assert.AreEqual(ZEKResult.Substring(35, 33), ZEKUnderLMK.Substring(2, 33))
    End Sub

End Class
