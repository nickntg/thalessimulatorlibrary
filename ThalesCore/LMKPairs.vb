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

''' <summary>
''' Class to declare LMK pair constants.
''' </summary>
''' <remarks>
''' This class includes several LMK constants.
''' </remarks>
Public Class LMKPairs

    ''' <summary>
    ''' LMK pair 00-01.
    ''' </summary>
    ''' <remarks>
    ''' Contains the two smart card ""keys"" (Passwords if the HSM is configured for Password mode) required for setting the HSM into the Authorized state.
    ''' </remarks>
    Public Const LMK_PAIR_00_01 As String = "00_01"

    ''' <summary>
    ''' LMK pair 02-03.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts the PINs for host storage.
    ''' </remarks>
    Public Const LMK_PAIR_02_03 As String = "02_03"

    ''' <summary>
    ''' LMK pair 04-05.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Zone Master Keys and double-length ZMKs. Encrypts Zone Master Key components under a Variant.
    ''' </remarks>
    Public Const LMK_PAIR_04_05 As String = "04_05"

    ''' <summary>
    ''' LMK pair 06-07.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts the Zone PIN keys for interchange transactions.
    ''' </remarks>
    Public Const LMK_PAIR_06_07 As String = "06_07"

    ''' <summary>
    ''' LMK pair 08-09.
    ''' </summary>
    ''' <remarks>
    ''' Used for random number generation.
    ''' </remarks>
    Public Const LMK_PAIR_08_09 As String = "08_09"

    ''' <summary>
    ''' LMK pair 10-11.
    ''' </summary>
    ''' <remarks>
    ''' Used for encrypting keys in HSM buffer areas.
    ''' </remarks>
    Public Const LMK_PAIR_10_11 As String = "10_11"

    ''' <summary>
    ''' LMK pair 12-13.
    ''' </summary>
    ''' <remarks>
    ''' The initial set of Secret Values created by the user; used for generating all other Master Key pairs.
    ''' </remarks>
    Public Const LMK_PAIR_12_13 As String = "12_13"

    ''' <summary>
    ''' LMK pair 14-15.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Terminal Master Keys, Terminal PIN Keys and PIN Verification Keys. Encrypts Card Verification Keys under a Variant.
    ''' </remarks>
    Public Const LMK_PAIR_14_15 As String = "14_15"

    ''' <summary>
    ''' LMK pair 16-17.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Terminal Authentication Keys.
    ''' </remarks>
    Public Const LMK_PAIR_16_17 As String = "16_17"

    ''' <summary>
    ''' LMK pair 18-19
    ''' </summary>
    ''' <remarks>
    ''' Encrypts reference numbers for solicitation mailers.
    ''' </remarks>
    Public Const LMK_PAIR_18_19 As String = "18_19"

    ''' <summary>
    ''' LMK pair 20-21.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts 'not on us' PIN Verification Keys and Card Verification Keys under a Variant.
    ''' </remarks>
    Public Const LMK_PAIR_20_21 As String = "20_21"

    ''' <summary>
    ''' LMK pair 22-23.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Watchword Keys.
    ''' </remarks>
    Public Const LMK_PAIR_22_23 As String = "22_23"

    ''' <summary>
    ''' LMK pair 24-25.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Zone Transport Keys.
    ''' </remarks>
    Public Const LMK_PAIR_24_25 As String = "24_25"

    ''' <summary>
    ''' LMK pair 26-27.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Zone Authentication Keys.
    ''' </remarks>
    Public Const LMK_PAIR_26_27 As String = "26_27"

    ''' <summary>
    ''' LMK pair 28-29.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Terminal Derivation Keys.
    ''' </remarks>
    Public Const LMK_PAIR_28_29 As String = "28_29"

    ''' <summary>
    ''' LMK pair 30-31.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Zone Encryption Keys.
    ''' </remarks>
    Public Const LMK_PAIR_30_31 As String = "30_31"

    ''' <summary>
    ''' LMK pair 32-33.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts Terminal Encryption Keys.
    ''' </remarks>
    Public Const LMK_PAIR_32_33 As String = "32_33"

    ''' <summary>
    ''' LMK pair 34-35.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts RSA keys.
    ''' </remarks>
    Public Const LMK_PAIR_34_35 As String = "34_35"

    ''' <summary>
    ''' LMK pair 36-37.
    ''' </summary>
    ''' <remarks>
    ''' Encrypts RSA MAC keys.
    ''' </remarks>
    Public Const LMK_PAIR_36_37 As String = "36_37"

    ''' <summary>
    ''' LMK pair 38-39.
    ''' </summary>
    ''' <remarks>
    ''' LMK pair 38-39.
    ''' </remarks>
    Public Const LMK_PAIR_38_39 As String = "38_39"

    ''' <summary>
    ''' LMK pair description.
    ''' </summary>
    ''' <remarks>
    ''' LMK pair description.
    ''' </remarks>
    Public Shared LMK_PAIR_DESCRIPTION() As String = {"Contains the two smart card ""keys"" (Passwords if the HSM is configured for Password mode) required for setting the HSM into the Authorized state.", _
                                                      "Encrypts the PINs for host storage.", _
                                                      "Encrypts Zone Master Keys and double-length ZMKs. Encrypts Zone Master Key components under a Variant.", _
                                                      "Encrypts the Zone PIN keys for interchange transactions.", _
                                                      "Used for random number generation.", _
                                                      "Used for encrypting keys in HSM buffer areas.", _
                                                      "The initial set of Secret Values created by the user; used for generating all other Master Key pairs.", _
                                                      "Encrypts Terminal Master Keys, Terminal PIN Keys and PIN Verification Keys. Encrypts Card Verification Keys under a Variant.", _
                                                      "Encrypts Terminal Authentication Keys.", _
                                                      "Encrypts reference numbers for solicitation mailers.", _
                                                      "Encrypts 'not on us' PIN Verification Keys and Card Verification Keys under a Variant.", _
                                                      "Encrypts Watchword Keys.", _
                                                      "Encrypts Zone Transport Keys.", _
                                                      "Encrypts Zone Authentication Keys.", _
                                                      "Encrypts Terminal Derivation Keys.", _
                                                      "Encrypts Zone Encryption Keys.", _
                                                      "Encrypts Terminal Encryption Keys.", _
                                                      "Encrypts RSA Keys.", _
                                                      "Encrypts RSA MAC Keys.", _
                                                      "LMK pair 38-39."}

    ''' <summary>
    ''' Enumeration of the LMK pairs.
    ''' </summary>
    ''' <remarks>
    ''' This is an enumeration of the LMK pairs.
    ''' </remarks>
    Public Enum LMKPair
        ''' <summary>
        ''' LMK pair 00-01.
        ''' </summary>
        ''' <remarks>
        ''' Contains the two smart card ""keys"" (Passwords if the HSM is configured for Password mode) required for setting the HSM into the Authorized state.
        ''' </remarks>
        Pair00_01 = 0
        ''' <summary>
        ''' LMK pair 02-03.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts the PINs for host storage.
        ''' </remarks>
        Pair02_03 = 1
        ''' <summary>
        ''' LMK pair 04-05.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Zone Master Keys and double-length ZMKs. Encrypts Zone Master Key components under a Variant.
        ''' </remarks>
        Pair04_05 = 2
        ''' <summary>
        ''' LMK pair 06-07.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts the Zone PIN keys for interchange transactions.
        ''' </remarks>
        Pair06_07 = 3
        ''' <summary>
        ''' LMK pair 08-09.
        ''' </summary>
        ''' <remarks>
        ''' Used for random number generation.
        ''' </remarks>
        Pair08_09 = 4
        ''' <summary>
        ''' LMK pair 10-11.
        ''' </summary>
        ''' <remarks>
        ''' Used for encrypting keys in HSM buffer areas.
        ''' </remarks>
        Pair10_11 = 5
        ''' <summary>
        ''' LMK pair 12-13.
        ''' </summary>
        ''' <remarks>
        ''' The initial set of Secret Values created by the user; used for generating all other Master Key pairs.
        ''' </remarks>
        Pair12_13 = 6
        ''' <summary>
        ''' LMK pair 14-15.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Terminal Master Keys, Terminal PIN Keys and PIN Verification Keys. Encrypts Card Verification Keys under a Variant.
        ''' </remarks>
        Pair14_15 = 7
        ''' <summary>
        ''' LMK pair 16-17.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Terminal Authentication Keys.
        ''' </remarks>
        Pair16_17 = 8
        ''' <summary>
        ''' LMK pair 18-19
        ''' </summary>
        ''' <remarks>
        ''' Encrypts reference numbers for solicitation mailers.
        ''' </remarks>
        Pair18_19 = 9
        ''' <summary>
        ''' LMK pair 20-21.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts 'not on us' PIN Verification Keys and Card Verification Keys under a Variant.
        ''' </remarks>
        Pair20_21 = 10
        ''' <summary>
        ''' LMK pair 22-23.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Watchword Keys.
        ''' </remarks>
        Pair22_23 = 11
        ''' <summary>
        ''' LMK pair 24-25.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Zone Transport Keys.
        ''' </remarks>
        Pair24_25 = 12
        ''' <summary>
        ''' LMK pair 26-27.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Zone Authentication Keys.
        ''' </remarks>
        Pair26_27 = 13
        ''' <summary>
        ''' LMK pair 28-29.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Terminal Derivation Keys.
        ''' </remarks>
        Pair28_29 = 14
        ''' <summary>
        ''' LMK pair 30-31.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Zone Encryption Keys.
        ''' </remarks>
        Pair30_31 = 15
        ''' <summary>
        ''' LMK pair 32-33.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts Terminal Encryption Keys.
        ''' </remarks>
        Pair32_33 = 16
        ''' <summary>
        ''' LMK pair 34-35.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts RSA keys.
        ''' </remarks>
        Pair34_35 = 17
        ''' <summary>
        ''' LMK pair 36-37.
        ''' </summary>
        ''' <remarks>
        ''' Encrypts RSA MAC keys.
        ''' </remarks>
        Pair36_37 = 18
        ''' <summary>
        ''' LMK pair 38-39.
        ''' </summary>
        ''' <remarks>
        ''' LMK pair 38-39.
        ''' </remarks>
        Pair38_39 = 19
    End Enum

    ''' <summary>
    ''' Parses a two-digit LMK type code.
    ''' </summary>
    ''' <remarks>
    ''' This method parses a two-digit LMK type code.
    ''' </remarks>
    Public Shared Sub LMKTypeCodeToLMKPair(ByVal s As String, ByRef LMK As LMKPair, ByRef var As Integer)

        LMK = CType(-1, LMKPair)
        var = 0

        If s Is Nothing OrElse s = "" Then Return

        Select Case s
            Case "00"
                LMK = LMKPair.Pair04_05
            Case "01"
                LMK = LMKPair.Pair06_07
            Case "02"
                LMK = LMKPair.Pair14_15
            Case "03"
                LMK = LMKPair.Pair16_17
            Case "04"
                LMK = LMKPair.Pair18_19
            Case "05"
                LMK = LMKPair.Pair20_21
            Case "06"
                LMK = LMKPair.Pair22_23
            Case "07"
                LMK = LMKPair.Pair24_25
            Case "08"
                LMK = LMKPair.Pair26_27
            Case "09"
                LMK = LMKPair.Pair28_29
            Case "0A"
                LMK = LMKPair.Pair30_31
            Case "0B"
                LMK = LMKPair.Pair32_33
            Case "10"
                LMK = LMKPair.Pair04_05
            Case "42"
                LMK = LMKPair.Pair14_15
        End Select

    End Sub

End Class
