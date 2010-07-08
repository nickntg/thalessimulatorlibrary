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

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Translates a ZEK or ZAK from encryption under a ZMK to encryption under LMK.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the FK Racal command.
    ''' </remarks>
    <ThalesCommandCode("FK", "FL", "", "Translates a ZEK or ZAK from encryption under a ZMK to encryption under LMK")> _
    Public Class TranslateZEKORZAKFromZMKToLMK_FK
        Inherits TranslateFromKeyToLMK

        Private _cmdFlag As String

        ''' <summary>
        ''' Initialization method.
        ''' </summary>
        ''' <remarks>
        ''' Command-specific implementation of message parsers, source/target LMK pairs, 
        ''' print strings, authorized and parity flags.
        ''' </remarks>
        Public Overrides Sub InitFields()
            SourceLMK = LMKPairs.LMKPair.Pair04_05
            str1 = "ZMK (clear): "
            str2 = "ZEK/ZAK (clear): "
            str3 = "ZEK/ZAK (LMK): "
        End Sub

        ''' <summary>
        ''' Accepts a message to parse.
        ''' </summary>
        ''' <remarks>
        ''' The AcceptMessage method is overriden to provide parsing of the ZEK/ZAK flag.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            MyBase.AcceptMessage(msg)
            _cmdFlag = kvp.Item("Flag")
        End Sub

        ''' <summary>
        ''' Constructs the command response.
        ''' </summary>
        ''' <remarks>
        ''' The ConstructResponse method is overriden to differentiate the <b>TargetLMK</b>
        ''' variable, depending upon the value of the ZEK/ZAK command flag.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As Message.MessageResponse

            If _cmdFlag = "0" Then
                TargetLMK = LMKPairs.LMKPair.Pair30_31
            Else
                TargetLMK = LMKPairs.LMKPair.Pair26_27
            End If

            Return MyBase.ConstructResponse()

        End Function

    End Class

End Namespace
