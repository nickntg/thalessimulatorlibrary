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

Imports ThalesSim.Core
Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography

Namespace HostCommands

    ''' <summary>
    ''' Translates a key from encryption under the LMK to another key.
    ''' </summary>
    ''' <remarks>
    ''' This class is inherited by commands that implement key to LMK translation of keys.
    ''' </remarks>
    Public MustInherit Class TranslateFromLMKToKey
        Inherits AHostCommand

        ''' <summary>
        ''' Source key string.
        ''' </summary>
        ''' <remarks>
        ''' Source key string.
        ''' </remarks>
        Protected Const SOURCE_KEY As String = "SOURCE_KEY"

        ''' <summary>
        ''' Target key string.
        ''' </summary>
        ''' <remarks>
        ''' Target key string.
        ''' </remarks>
        Protected Const TARGET_KEY As String = "TARGET_KEY"

        Private _sourceKEY As String
        Private _targetKEY As String
        Private _del As String
        Private _keySchemeZMK As String
        Private _keySchemeLMK As String
        Private _keyCheckValue As String

        ''' <summary>
        ''' Source LMK pair.
        ''' </summary>
        ''' <remarks>
        ''' The LMK pair under which the source key is encrypted.
        ''' </remarks>
        Protected SourceLMK As LMKPairs.LMKPair

        ''' <summary>
        ''' Target LMK pair.
        ''' </summary>
        ''' <remarks>
        ''' The LMK pair under which the target key is encrypted.
        ''' </remarks>
        Protected TargetLMK As LMKPairs.LMKPair

        ''' <summary>
        ''' First print string.
        ''' </summary>
        ''' <remarks>
        ''' Print string used to denote the clear source key.
        ''' </remarks>
        Protected str1 As String

        ''' <summary>
        ''' Second print string.
        ''' </summary>
        ''' <remarks>
        ''' Print string used to denote the clear target key.
        ''' </remarks>
        Protected str2 As String

        ''' <summary>
        ''' Third print string.
        ''' </summary>
        ''' <remarks>
        ''' Print string used to denote the encrypted target key under the source key.
        ''' </remarks>
        Protected str3 As String

        ''' <summary>
        ''' Authorized mode flag.
        ''' </summary>
        ''' <remarks>
        ''' Set this to True to indicate that the commands needs the Authorized mode.
        ''' </remarks>
        Protected NeedsAuthorizedMode As Boolean = False

        ''' <summary>
        ''' Check value flag.
        ''' </summary>
        ''' <remarks>
        ''' Set this to False to indicate that the command does not return a check value.
        ''' </remarks>
        Protected UsesCheckValue As Boolean = True

        ''' <summary>
        ''' Source variant.
        ''' </summary>
        ''' <remarks>
        ''' Set this to the value of the variant under which the source key is encrypted
        '''  (default is 0 for most commands).
        ''' </remarks>
        Protected SourceVariant As String = "0"

        ''' <summary>
        ''' Target variant.
        ''' </summary>
        ''' <remarks>
        ''' Set this to the value of the variant under which the target key is to be encrypted
        '''  (default is 0 for most commands).
        ''' </remarks>
        Protected TargetVariant As String = "0"

        ''' <summary>
        ''' CVK check digits flag.
        ''' </summary>
        ''' <remarks>
        ''' Set this to True to indicate that the key translate command should produce
        ''' CVK-style check digits.
        ''' </remarks>
        Protected CVKCheckDigits As Boolean = False

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            InitFields()
        End Sub

        ''' <summary>
        ''' Initialization method.
        ''' </summary>
        ''' <remarks>
        ''' This method must be overriden to provide specific implementation of the 
        ''' message determiners, LMK pair translation, print string definitions, authorized
        ''' mode and check value flags.
        ''' </remarks>
        Public MustOverride Sub InitFields()

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            MFPC.ParseMessage(msg)
            _sourceKEY = MFPC.GetMessageFieldByName(SOURCE_KEY).FieldValue
            _targetKEY = MFPC.GetMessageFieldByName(TARGET_KEY).FieldValue
            _del = MFPC.GetMessageFieldByName(DELIMITER).FieldValue
            _keySchemeZMK = MFPC.GetMessageFieldByName(KEY_SCHEME_ZMK).FieldValue
            _keySchemeLMK = MFPC.GetMessageFieldByName(KEY_SCHEME_LMK).FieldValue
            _keyCheckValue = MFPC.GetMessageFieldByName(KEY_CHECK_VALUE).FieldValue
        End Sub

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As Message.MessageResponse
            Dim mr As New MessageResponse

            If NeedsAuthorizedMode = True Then
                If CType(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE), Boolean) = False Then
                    Log.Logger.MajorInfo("Can't run command while not in the AUTHORIZED state")
                    mr.AddElement(ErrorCodes._17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                    Return mr
                End If
            End If

            Dim LMKks As KeySchemeTable.KeyScheme, KeyKs As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, LMKks, mr) = False Then Return mr
                If ValidateKeySchemeCode(_keySchemeZMK, KeyKs, mr) = False Then Return mr
            Else
                Select Case MFPC.GetMessageFieldByName(TARGET_KEY).DeterminerName
                    Case TARGET_KEY + DOUBLE_VARIANT, TARGET_KEY + DOUBLE_X917
                        LMKks = KeySchemeTable.KeyScheme.DoubleLengthKeyVariant
                        KeyKs = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                    Case TARGET_KEY + TRIPLE_VARIANT, TARGET_KEY + TRIPLE_X917
                        LMKks = KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                        KeyKs = KeySchemeTable.KeyScheme.TripleLengthKeyAnsi
                    Case TARGET_KEY + PLAIN_DOUBLE
                        LMKks = KeySchemeTable.KeyScheme.Unspecified
                        KeyKs = KeySchemeTable.KeyScheme.Unspecified
                    Case Else
                        LMKks = KeySchemeTable.KeyScheme.SingleDESKey
                        KeyKs = KeySchemeTable.KeyScheme.SingleDESKey
                End Select
                'If MFPC.GetMessageFieldByName(SOURCE_KEY).DeterminerName = SOURCE_KEY + Me.PLAIN_DOUBLE Then
                '    LMKks = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                '    KeyKs = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                'Else
                '    LMKks = KeySchemeTable.KeyScheme.SingleDESKey
                '    KeyKs = KeySchemeTable.KeyScheme.SingleDESKey
                'End If
                _keyCheckValue = "0"
            End If

            Dim clearSource As String, clearTarget As String

            clearSource = Utility.DecryptUnderLMK(_sourceKEY, SOURCE_KEY, MFPC.GetMessageFieldByName(SOURCE_KEY).DeterminerName, SourceLMK, SourceVariant)
            If Utility.IsParityOK(clearSource, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            clearTarget = Utility.DecryptUnderLMK(_targetKEY, TARGET_KEY, MFPC.GetMessageFieldByName(TARGET_KEY).DeterminerName, TargetLMK, TargetVariant)
            If Utility.IsParityOK(clearTarget, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim cryptKey As String = Utility.EncryptUnderZMK(clearSource, Utility.RemoveKeyType(clearTarget), KeyKs)
            Dim checkValue As String = ""

            Log.Logger.MinorInfo(str1 + clearSource)
            Log.Logger.MinorInfo(str2 + clearTarget)
            Log.Logger.MinorInfo(str3 + cryptKey)

            If UsesCheckValue = True Then
                If CVKCheckDigits = False Then
                    checkValue = TripleDES.TripleDESEncrypt(New HexKey(clearTarget), ZEROES)
                    If _keyCheckValue = "0" Then
                        Log.Logger.MinorInfo("Check value: " + checkValue)
                    Else
                        Log.Logger.MinorInfo("Check value: " + checkValue.Substring(0, 6))
                    End If
                Else
                    If _keyCheckValue = "0" Then
                        Dim chk1 As String = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(clearTarget).Substring(0, 16)), ZEROES).Substring(0, 6)
                        Dim chk2 As String = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(clearTarget).Substring(16)), ZEROES).Substring(0, 6)
                        Log.Logger.MinorInfo("CVK-A check value: " + chk1)
                        Log.Logger.MinorInfo("CVK-B check value: " + chk2)
                        checkValue = chk1 + chk2
                    Else
                        checkValue = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(clearTarget)), ZEROES).Substring(0, 6)
                        Log.Logger.MinorInfo("CVK check value: " + checkValue)
                    End If
                End If
            End If

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(cryptKey)

            If UsesCheckValue = True Then
                If CVKCheckDigits = False Then
                    If _keyCheckValue = "0" Then
                        mr.AddElement(checkValue)
                    Else
                        mr.AddElement(checkValue.Substring(0, 6))
                    End If
                Else
                    mr.AddElement(checkValue)
                End If
            End If

            Return mr

        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> as no printer I/O is related with this command.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            Return Nothing
        End Function

    End Class

End Namespace
