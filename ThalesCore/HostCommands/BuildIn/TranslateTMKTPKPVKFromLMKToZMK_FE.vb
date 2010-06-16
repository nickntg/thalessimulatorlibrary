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
    ''' Translates a TMK, TPK or PVK from encryption under the LMK to encryption under a ZMK.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the FE Racal command.
    ''' </remarks>
    <ThalesCommandCode("FE", "FF", "", "Translates a TMK, TPK or PVK from encryption under the LMK to encryption under a ZMK")> _
    Public Class TranslateTMKTPKPVKFromLMKToZMK_FE
        Inherits TranslateFromLMKToKey

        ''' <summary>
        ''' Internal initialization method.
        ''' </summary>
        ''' <remarks>
        ''' This method provides specific implementation of the message determiners,
        ''' LMK pair translation and print string definitions.
        ''' </remarks>
        Public Overrides Sub InitFields()
            SourceLMK = LMKPairs.LMKPair.Pair04_05
            TargetLMK = LMKPairs.LMKPair.Pair14_15
            str1 = "ZMK (clear): "
            str2 = "TMK (clear): "
            str3 = "TMK (TMK): "
            NeedsAuthorizedMode = True
        End Sub

    End Class

End Namespace
