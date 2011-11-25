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
    ''' Translates a key from encryption under another key to encryption under the LMK.
    ''' </summary>
    ''' <remarks>
    ''' This class is inherited by commands that perform key to LMK translation of keys.
    ''' </remarks>
    Public MustInherit Class TranslateFromKeyToLMK
        Inherits AHostCommand

        Private _sourceKEY As String
        Private _targetKEY As String
        Private _del As String
        Private _keySchemeLMK As String
        Private _keyCheckValue As String
        Private _sourceKeyScheme As KeySchemeTable.KeyScheme
        Private _targetKeyScheme As KeySchemeTable.KeyScheme
        Private _atallaVariant As String = String.Empty

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
        ''' The LMK pair under which the result is encrypted.
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
        ''' Bad parity flag.
        ''' </summary>
        ''' <remarks>
        ''' Set this to True to indicate that it's allowed to have a parity error on the
        ''' input key that is encrypted under the source key. If that is the case, odd
        ''' parity is enforced on the final resulting key.
        ''' </remarks>
        Protected AllowBadParity As Boolean = False

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
        ''' mode and parity flags, source and target variants and CVK-style check digit flag.
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
                _sourceKEY = kvp.ItemCombination("ZMK Scheme", "ZMK")
                _targetKEY = kvp.ItemCombination("Key Scheme", "Key")
                _del = kvp.ItemOptional("Delimiter")
                _keySchemeLMK = kvp.ItemOptional("Key Scheme LMK")
                _keyCheckValue = kvp.ItemOptional("Key Check Value Type")
                If kvp.ItemOptional("ZMK Scheme") <> "" Then
                    _sourceKeyScheme = KeySchemeTable.GetKeySchemeFromValue(kvp.ItemOptional("ZMK Scheme"))
                Else
                    _sourceKeyScheme = KeySchemeTable.KeyScheme.Unspecified
                End If
                If kvp.ItemOptional("Key Scheme") <> "" Then
                    _targetKeyScheme = KeySchemeTable.GetKeySchemeFromValue(kvp.ItemOptional("Key Scheme"))
                Else
                    _targetKeyScheme = KeySchemeTable.KeyScheme.Unspecified
                End If

                _atallaVariant = kvp.ItemOptional("Atalla Variant 1") + kvp.ItemOptional("Atalla Variant 2")
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

            Dim LMKks As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, LMKks, mr) = False Then Return mr
            Else
                LMKks = _targetKeyScheme
                _keyCheckValue = "0"
            End If

            Dim clearSource As String, clearTarget As String
            Dim cryptSource As New HexKey(_sourceKEY)
            clearSource = Utility.DecryptUnderLMK(cryptSource.ToString, cryptSource.Scheme, SourceLMK, SourceVariant)
            If Utility.IsParityOK(clearSource, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            'This catered only for single-length key situations (see http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=70958).
            'clearTarget = TripleDES.TripleDESDecrypt(New HexKey(clearSource), _targetKEY)

            clearTarget = DecryptUnderZMK(clearSource, Utility.RemoveKeyType(_targetKEY), _targetKeyScheme, _atallaVariant) ' KeySchemeTable.KeyScheme.DoubleLengthKeyVariant)
            clearTarget = Utility.RemoveKeyType(clearTarget)

            Dim finalTarget As String = clearTarget
            If AllowBadParity = True Then
                If Utility.IsParityOK(clearTarget, Utility.ParityCheck.OddParity) = False Then
                    finalTarget = Utility.MakeParity(clearTarget, Utility.ParityCheck.OddParity)
                End If
            Else
                If Utility.IsParityOK(clearTarget, Utility.ParityCheck.OddParity) = False Then
                    mr.AddElement(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR)
                    Return mr
                End If
            End If

            Dim cryptKey As String = Utility.EncryptUnderLMK(finalTarget, LMKks, TargetLMK, TargetVariant)
            Dim checkValue As String

            Log.Logger.MinorInfo(str1 + clearSource)
            Log.Logger.MinorInfo(str2 + clearTarget)
            If AllowBadParity = True Then
                Log.Logger.MinorInfo("Key (clear): " + finalTarget)
            End If
            Log.Logger.MinorInfo(str3 + cryptKey)

            If CVKCheckDigits = False Then
                checkValue = TripleDES.TripleDESEncrypt(New HexKey(finalTarget), ZEROES)
                If _keyCheckValue = "0" Then
                    Log.Logger.MinorInfo("Check value: " + checkValue)
                Else
                    Log.Logger.MinorInfo("Check value: " + checkValue.Substring(0, 6))
                End If
            Else
                If _keyCheckValue = "0" Then
                    Dim chk1 As String = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(finalTarget).Substring(0, 16)), ZEROES).Substring(0, 6)
                    Dim chk2 As String = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(finalTarget).Substring(16)), ZEROES).Substring(0, 6)
                    Log.Logger.MinorInfo("CVK-A check value: " + chk1)
                    Log.Logger.MinorInfo("CVK-B check value: " + chk2)
                    checkValue = chk1 + chk2
                Else
                    checkValue = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(finalTarget)), ZEROES).Substring(0, 6)
                    Log.Logger.MinorInfo("CVK check value: " + checkValue)
                End If
            End If

            If AllowBadParity = True Then
                If finalTarget <> clearTarget Then
                    mr.AddElement(ErrorCodes.ER_01_VERIFICATION_FAILURE)
                Else
                    mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
                End If
            Else
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            End If

            mr.AddElement(cryptKey)

            If CVKCheckDigits = False Then
                If _keyCheckValue = "0" Then
                    mr.AddElement(checkValue)
                Else
                    mr.AddElement(checkValue.Substring(0, 6))
                End If
            Else
                mr.AddElement(checkValue)
            End If

            Return mr

        End Function

    End Class

End Namespace
