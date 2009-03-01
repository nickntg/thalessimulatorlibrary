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
    ''' Translates a TMK, TPK or PVK from LMK to TMP, TPK or PVK encryption.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the AE Racal command.
    ''' </remarks>
    <ThalesCommandCode("AE", "AF", "", "Translates a TMK, TPK or PVK from LMK to TMP, TPK or PVK encryption")> _
    Public Class TranslateTMKTPKPVKFromLMKToTMKTPKPVK_AE
        Inherits TranslateFromLMKToKey

        ''' <summary>
        ''' Internal initialization method.
        ''' </summary>
        ''' <remarks>
        ''' This method provides specific implementation of the message determiners,
        ''' LMK pair translation and print string definitions.
        ''' </remarks>
        Public Overrides Sub InitFields()
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(SOURCE_KEY))
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(TARGET_KEY))
            GenerateDelimiterParser()
            SourceLMK = LMKPairs.LMKPair.Pair14_15
            targetlmk = LMKPairs.LMKPair.Pair14_15
            str1 = "TMK-1 (clear): "
            str2 = "TMK-2 (clear): "
            str3 = "TMK-2 (TMK-1): "
            UsesCheckValue = False
        End Sub

    End Class

End Namespace
