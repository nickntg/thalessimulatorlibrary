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
    ''' Translates a TAK from encryption under a ZMK to encryption under LMK.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the MI Racal command.
    ''' </remarks>
    <ThalesCommandCode("MI", "MJ", "", "Translates a TAK from encryption under a ZMK to encryption under LMK")> _
    Public Class TranslateTAKFromZMKToLMK_MI
        Inherits TranslateFromKeyToLMK

        ''' <summary>
        ''' Initialization method.
        ''' </summary>
        ''' <remarks>
        ''' Command-specific implementation of message parsers, source/target LMK pairs, 
        ''' print strings, authorized and parity flags.
        ''' </remarks>
        Public Overrides Sub InitFields()
            MFPC.AddMessageFieldParser(GenerateLongZMKKeyParser(SOURCE_KEY, 60))
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(TARGET_KEY))
            GenerateDelimiterParser()
            SourceLMK = LMKPairs.LMKPair.Pair04_05
            targetlmk = LMKPairs.LMKPair.Pair16_17
            str1 = "ZMK (clear): "
            str2 = "TAK (clear): "
            str3 = "TAK (LMK): "
        End Sub
    End Class

End Namespace
