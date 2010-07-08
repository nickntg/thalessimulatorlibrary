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

        Private _sourceKEY As String
        Private _targetKEY As String
        Private _del As String
        Private _keySchemeZMK As String
        Private _keyCheckValue As String
        Private _sourceKeyScheme As KeySchemeTable.KeyScheme
        Private _targetKeyScheme As KeySchemeTable.KeyScheme

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
            ReadXMLDefinitions()
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
            XML.MessageParser.Parse(msg, XMLMessageFields, kvp, XMLParseResult)
            If XMLParseResult = ErrorCodes.ER_00_NO_ERROR Then
                _sourceKEY = kvp.ItemCombination("Source Key Scheme", "Source Key")
                _targetKEY = kvp.ItemCombination("Target Key Scheme", "Target Key")
                _del = kvp.ItemOptional("Delimiter")
                _keySchemeZMK = kvp.ItemOptional("Key Scheme ZMK")
                _keyCheckValue = kvp.ItemOptional("Key Check Value Type")
                If kvp.ItemOptional("Source Key Scheme") <> "" Then
                    _sourceKeyScheme = KeySchemeTable.GetKeySchemeFromValue(kvp.ItemOptional("Source Key Scheme"))
                Else
                    _sourceKeyScheme = KeySchemeTable.KeyScheme.Unspecified
                End If
                If kvp.ItemOptional("Target Key Scheme") <> "" Then
                    _targetKeyScheme = KeySchemeTable.GetKeySchemeFromValue(kvp.ItemOptional("Target Key Scheme"))
                Else
                    _targetKeyScheme = KeySchemeTable.KeyScheme.Unspecified
                End If
            End If
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
                If Not IsInAuthorizedState() Then
                    Log.Logger.MajorInfo("Can't run command while not in the AUTHORIZED state")
                    mr.AddElement(ErrorCodes.ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                    Return mr
                End If
            End If

            Dim KeyKs As KeySchemeTable.KeyScheme
            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeZMK, KeyKs, mr) = False Then Return mr
            Else
                KeyKs = _targetKeyScheme
                _keyCheckValue = "0"
            End If

            Dim clearSource As String, clearTarget As String
            Dim cryptSource As New HexKey(_sourceKEY), cryptTarget As New HexKey(_targetKEY)

            clearSource = Utility.DecryptUnderLMK(cryptSource.ToString, cryptSource.Scheme, SourceLMK, SourceVariant)
            If Utility.IsParityOK(clearSource, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            clearTarget = Utility.DecryptUnderLMK(cryptTarget.ToString, cryptTarget.Scheme, TargetLMK, TargetVariant)
            If Utility.IsParityOK(clearTarget, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR)
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

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
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

    End Class

End Namespace
