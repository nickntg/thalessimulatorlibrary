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
''' This module containts various constants used throught the simulator.
''' </summary>
''' <remarks></remarks>

Module Constants

    ''' <summary>
    ''' 16 zeroes.
    ''' </summary>
    ''' <remarks>
    ''' 16 zeroes.
    ''' </remarks>
    Public Const ZEROES As String = "0000000000000000"

    ''' <summary>
    ''' Suffix for X9.17 double-length key determiners.
    ''' </summary>
    ''' <remarks>
    ''' Suffix for X9.17 double-length key determiners.
    ''' </remarks>
    Public Const DOUBLE_X917 As String = "_DOUBLE_X917"

    ''' <summary>
    ''' Suffix for X9.17 triple-length key determiners.
    ''' </summary>
    ''' <remarks>
    ''' Suffix for X9.17 triple-length key determiners.
    ''' </remarks>
    Public Const TRIPLE_X917 As String = "_TRIPLE_X917"

    ''' <summary>
    ''' Suffix for variant double-length key determiners.
    ''' </summary>
    ''' <remarks>
    ''' Suffix for variant double-length key determiners.
    ''' </remarks>
    Public Const DOUBLE_VARIANT As String = "_DOUBLE_VARIANT"

    ''' <summary>
    ''' Suffix for variant triple-length key determiners.
    ''' </summary>
    ''' <remarks>
    ''' Suffix for variant triple-length key determiners.
    ''' </remarks>
    Public Const TRIPLE_VARIANT As String = "_TRIPLE_VARIANT"

    ''' <summary>
    ''' Suffix for double-length key determiners without a designation (X9.17).
    ''' </summary>
    ''' <remarks>
    ''' Suffix for double-length key determiners without a designation (X9.17).
    ''' </remarks>
    Public Const PLAIN_DOUBLE As String = "_PLAIN_DOUBLE"

    ''' <summary>
    ''' Suffix for single-length key determiners without a designation (X9.17).
    ''' </summary>
    ''' <remarks>
    ''' Suffix for single-length key determiners without a designation (X9.17).
    ''' </remarks>
    Public Const PLAIN_SINGLE As String = "_PLAIN_SIMPLE"

    ''' <summary>
    ''' Delimiter value.
    ''' </summary>
    ''' <remarks>
    ''' Common delimiter value used in some Racal commands.
    ''' </remarks>
    Public Const DELIMITER_VALUE As String = ";"

    ''' <summary>
    ''' Key value for a delimiter field.
    ''' </summary>
    ''' <remarks>
    ''' Key value for a delimiter field.
    ''' </remarks>
    Public Const DELIMITER As String = "DELIMITER"

    ''' <summary>
    ''' Key value to indicate delimiter presence.
    ''' </summary>
    ''' <remarks>
    ''' This value is used by a determiner if a delimiter is present.
    ''' </remarks>
    Public Const DELIMITER_EXISTS As String = "DEL_PRESENT"

    ''' <summary>
    ''' Key value to indicate delimiter absence.
    ''' </summary>
    ''' <remarks>
    ''' This value is used by a determiner if a delimiter is absent.
    ''' </remarks>
    Public Const DELIMITER_NOT_EXISTS As String = "DEL_ABSENT"

    ''' <summary>
    ''' Key value used for Key Scheme ZMK fields.
    ''' </summary>
    ''' <remarks>
    ''' Key value used for Key Scheme ZMK fields.
    ''' </remarks>
    Public Const KEY_SCHEME_ZMK As String = "KEY_SCHEME_ZMK"

    ''' <summary>
    ''' Key value used for Key Scheme LMK fields.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const KEY_SCHEME_LMK As String = "KEY_SCHEME_LMK"

    ''' <summary>
    ''' Key value used for Key Check Value fields.
    ''' </summary>
    ''' <remarks>
    ''' Key value used for Key Check Value fields.
    ''' </remarks>
    Public Const KEY_CHECK_VALUE As String = "KEY_CHECK_VALUE"

End Module
