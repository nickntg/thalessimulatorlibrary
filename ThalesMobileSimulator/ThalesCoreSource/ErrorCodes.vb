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
''' This class abstracts the Racal returned error codes.
''' </summary>
''' <remarks>Most Racal error codes are declared as constants in this class.</remarks>
Public Class ErrorCodes

    ''' <summary>
    ''' Racal error code 00.
    ''' </summary>
    ''' <remarks>No error.</remarks>
    Public Const ER_00_NO_ERROR As String = "00"

    ''' <summary>
    ''' Racal error code 01.
    ''' </summary>
    ''' <remarks>
    ''' Verification failure.
    ''' </remarks>
    Public Const ER_01_VERIFICATION_FAILURE As String = "01"

    ''' <summary>
    ''' Racal error code 02.
    ''' </summary>
    ''' <remarks>
    ''' Inappropriate key length for algorithm.
    ''' </remarks>
    Public Const ER_02_INAPPROPRIATE_KEY_LENGTH_FOR_ALGORITHM As String = "02"

    ''' <summary>
    ''' Racal error code 03.
    ''' </summary>
    ''' <remarks>
    ''' Invalid number of components.
    ''' </remarks>
    Public Const ER_03_INVALID_NUMBER_OF_COMPONENTS As String = "03"

    ''' <summary>
    ''' Racal error code 04.
    ''' </summary>
    ''' <remarks>
    ''' Invalid key type code.
    ''' </remarks>
    Public Const ER_04_INVALID_KEY_TYPE_CODE As String = "04"

    ''' <summary>
    ''' Racal error code 05.
    ''' </summary>
    ''' <remarks>
    ''' Invalid key length flag.
    ''' </remarks>
    Public Const ER_05_INVALID_KEY_LENGTH_FLAG As String = "05"

    ''' <summary>
    ''' Racal error code 05.
    ''' </summary>
    ''' <remarks>
    ''' Invalid hash identifier.
    ''' </remarks>
    Public Const ER_05_INVALID_HASH_IDENTIFIER As String = "05"

    ''' <summary>
    ''' Racal error code 10.
    ''' </summary>
    ''' <remarks>
    ''' Source key parity error.
    ''' </remarks>
    Public Const ER_10_SOURCE_KEY_PARITY_ERROR As String = "10"

    ''' <summary>
    ''' Racal error code 11.
    ''' </summary>
    ''' <remarks>
    ''' Destination key parity error.
    ''' </remarks>
    Public Const ER_11_DESTINATION_KEY_PARITY_ERROR As String = "11"

    ''' <summary>
    ''' Racal error code 12.
    ''' </summary>
    ''' <remarks>
    ''' Contents of user storage not available.
    ''' </remarks>
    Public Const ER_12_CONTENTS_OF_USER_STORAGE_NOT_AVAILABLE As String = "12"

    ''' <summary>
    ''' Racal error code 13.
    ''' </summary>
    ''' <remarks>
    ''' Master key parity error.
    ''' </remarks>
    Public Const ER_13_MASTER_KEY_PARITY_ERROR As String = "13"

    ''' <summary>
    ''' Racal error code 14.
    ''' </summary>
    ''' <remarks>
    ''' PIN encrypted under LMK pair 02-03 is invalid.
    ''' </remarks>
    Public Const ER_14_PIN_ENCRYPTED_UNDER_LMK_PAIR_02_03_IS_INVALID As String = "14"

    ''' <summary>
    ''' Racal error code 15.
    ''' </summary>
    ''' <remarks>
    ''' Invalid input data.
    ''' </remarks>
    Public Const ER_15_INVALID_INPUT_DATA As String = "15"

    ''' <summary>
    ''' Racal error code 16.
    ''' </summary>
    ''' <remarks>
    ''' Console or printer not ready/not connected.
    ''' </remarks>
    Public Const ER_16_CONSOLE_OR_PRINTER_NOT_READY_NOT_CONNECTED As String = "16"

    ''' <summary>
    ''' Racal error code 17.
    ''' </summary>
    ''' <remarks>
    ''' HSM is not in the authorized state.
    ''' </remarks>
    Public Const ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE As String = "17"

    ''' <summary>
    ''' Racal error code 18.
    ''' </summary>
    ''' <remarks>
    ''' Document definition not loaded.
    ''' </remarks>
    Public Const ER_18_DOCUMENT_DEFINITION_NOT_LOADED As String = "18"

    ''' <summary>
    ''' Racal error code 19.
    ''' </summary>
    ''' <remarks>
    ''' Specified Diebold table is invalid.
    ''' </remarks>
    Public Const ER_19_SPECIFIED_DIEBOLD_TABLE_IS_INVALID As String = "19"

    ''' <summary>
    ''' Racal error code 20.
    ''' </summary>
    ''' <remarks>
    ''' PIN block does not contain valid values.
    ''' </remarks>
    Public Const ER_20_PIN_BLOCK_DOES_NOT_CONTAIN_VALID_VALUES As String = "20"

    ''' <summary>
    ''' Racal error code 21.
    ''' </summary>
    ''' <remarks>
    ''' Invalid index value.
    ''' </remarks>
    Public Const ER_21_INVALID_INDEX_VALUE As String = "21"

    ''' <summary>
    ''' Racal error code 22.
    ''' </summary>
    ''' <remarks>
    ''' Invalid account number.
    ''' </remarks>
    Public Const ER_22_INVALID_ACCOUNT_NUMBER As String = "22"

    ''' <summary>
    ''' Racal error code 23.
    ''' </summary>
    ''' <remarks>
    ''' Invalid PIN block format code.
    ''' </remarks>
    Public Const ER_23_INVALID_PIN_BLOCK_FORMAT_CODE As String = "23"

    ''' <summary>
    ''' Racal error code 24.
    ''' </summary>
    ''' <remarks>
    ''' PIN is fewer than 4 or more than 12 digits long.
    ''' </remarks>
    Public Const ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG As String = "24"

    ''' <summary>
    ''' Racal error code 25.
    ''' </summary>
    ''' <remarks>
    ''' Decimalization table error.
    ''' </remarks>
    Public Const ER_25_DECIMALIZATION_TABLE_ERROR As String = "25"

    ''' <summary>
    ''' Racal error code 26.
    ''' </summary>
    ''' <remarks>
    ''' Invalid key scheme.
    ''' </remarks>
    Public Const ER_26_INVALID_KEY_SCHEME As String = "26"

    ''' <summary>
    ''' Racal error code 27.
    ''' </summary>
    ''' <remarks>
    ''' Incompatible key length.
    ''' </remarks>
    Public Const ER_27_INCOMPATIBLE_KEY_LENGTH As String = "27"

    ''' <summary>
    ''' Racal error code 28.
    ''' </summary>
    ''' <remarks>
    ''' Invalid key type.
    ''' </remarks>
    Public Const ER_28_INVALID_KEY_TYPE As String = "28"

    ''' <summary>
    ''' Racal error code 29.
    ''' </summary>
    ''' <remarks>
    ''' Function not permitted.
    ''' </remarks>
    Public Const ER_29_FUNCTION_NOT_PERMITTED As String = "29"

    ''' <summary>
    ''' Racal error code 30.
    ''' </summary>
    ''' <remarks>
    ''' Invalid reference number.
    ''' </remarks>
    Public Const ER_30_INVALID_REFERENCE_NUMBER As String = "30"

    ''' <summary>
    ''' Racal error code 31.
    ''' </summary>
    ''' <remarks>
    ''' Insuficcient solicitation entries for batch.
    ''' </remarks>
    Public Const ER_31_INSUFICCIENT_SOLICITATION_ENTRIES_FOR_BATCH As String = "31"

    ''' <summary>
    ''' Racal error code 33.
    ''' </summary>
    ''' <remarks>
    ''' LMK key change storage is corrupted.
    ''' </remarks>
    Public Const ER_33_LMK_KEY_CHANGE_STORAGE_IS_CORRUPTED As String = "33"

    ''' <summary>
    ''' Racal error code 40.
    ''' </summary>
    ''' <remarks>
    ''' Invalid firmware checksum.
    ''' </remarks>
    Public Const ER_40_INVALID_FIRMWARE_CHECKSUM As String = "40"

    ''' <summary>
    ''' Racal error code 41.
    ''' </summary>
    ''' <remarks>
    ''' Internal hardware/software error.
    ''' </remarks>
    Public Const ER_41_INTERNAL_HARDWARE_SOFTWARE_ERROR As String = "41"

    ''' <summary>
    ''' Racal error code 42.
    ''' </summary>
    ''' <remarks>
    ''' DES failure.
    ''' </remarks>
    Public Const ER_42_DES_FAILURE As String = "42"

    ''' <summary>
    ''' Error code 51 
    ''' </summary>
    ''' <remarks>Invalid message header</remarks>
    Public Const ER_51_INVALID_MESSAGE_HEADER As String = "51"

    ''' <summary>
    ''' Error code 52.
    ''' </summary>
    ''' <remarks>Invalid Number of Commands field.</remarks>
    Public Const ER_52_INVALID_NUMBER_OF_COMMANDS As String = "52"

    ''' <summary>
    ''' Racal error code 80.
    ''' </summary>
    ''' <remarks>
    ''' Data length error.
    ''' </remarks>
    Public Const ER_80_DATA_LENGTH_ERROR As String = "80"

    ''' <summary>
    ''' Racal error code 90.
    ''' </summary>
    ''' <remarks>
    ''' Data parity error.
    ''' </remarks>
    Public Const ER_90_DATA_PARITY_ERROR As String = "90"

    ''' <summary>
    ''' Racal error code 91.
    ''' </summary>
    ''' <remarks>
    ''' LRC error.
    ''' </remarks>
    Public Const ER_91_LRC_ERROR As String = "91"

    ''' <summary>
    ''' Racal error code 92.
    ''' </summary>
    ''' <remarks>
    ''' Count value not between limits.
    ''' </remarks>
    Public Const ER_92_COUNT_VALUE_NOT_BETWEEN_LIMITS As String = "92"

    ''' <summary>
    ''' Racal error code ZZ.
    ''' </summary>
    ''' <remarks>
    ''' This error code may be internally used.
    ''' </remarks>
    Public Const ER_ZZ_UNKNOWN_ERROR As String = "ZZ"

    Private Shared _errors() As ThalesError = {New ThalesError("00", "No error"), _
                                               New ThalesError("01", "Verification failure"), _
                                               New ThalesError("02", "Inappropriate key length for algorithm"), _
                                               New ThalesError("03", "Invalid number of components"), _
                                               New ThalesError("04", "Invalid key type code"), _
                                               New ThalesError("05", "Invalid key length flag"), _
                                               New ThalesError("10", "Source key parity error"), _
                                               New ThalesError("11", "Destination key parity error"), _
                                               New ThalesError("12", "Contents of user storage not available"), _
                                               New ThalesError("13", "Master key parity error"), _
                                               New ThalesError("14", "PIN encrypted under LMK pair 02-03 is invalid"), _
                                               New ThalesError("15", "Invalid input data"), _
                                               New ThalesError("16", "Console or printer not ready/not connected"), _
                                               New ThalesError("17", "HSM is not in the authorized state"), _
                                               New ThalesError("18", "Document definition not loaded"), _
                                               New ThalesError("19", "Specified Diebold table is invalid"), _
                                               New ThalesError("20", "PIN block does not contain valid values"), _
                                               New ThalesError("21", "Invalid index value"), _
                                               New ThalesError("22", "Invalid account number"), _
                                               New ThalesError("23", "Invalid PIN block format code"), _
                                               New ThalesError("24", "PIN is fewer than 4 or more than 12 digits long"), _
                                               New ThalesError("25", "Decimalization table error"), _
                                               New ThalesError("26", "Invalid key scheme"), _
                                               New ThalesError("27", "Incompatible key length"), _
                                               New ThalesError("28", "Invalid key type"), _
                                               New ThalesError("29", "Function not permitted"), _
                                               New ThalesError("30", "Invalid reference number"), _
                                               New ThalesError("31", "Insuficcient solicitation entries for batch"), _
                                               New ThalesError("33", "LMK key change storage is corrupted"), _
                                               New ThalesError("40", "Invalid firmware checksum"), _
                                               New ThalesError("41", "Internal hardware/software error"), _
                                               New ThalesError("42", "DES failure"), _
                                               New ThalesError("51", "Invalid message header"), _
                                               New ThalesError("52", "Invalid number of command fields"), _
                                               New ThalesError("80", "Data length error"), _
                                               New ThalesError("90", "Data parity error"), _
                                               New ThalesError("91", "LRC error"), _
                                               New ThalesError("92", "Count value not between limits"), _
                                               New ThalesError("ZZ", "UNKNOWN ERROR")}

    ''' <summary>
    ''' Returns error help.
    ''' </summary>
    ''' <remarks>
    ''' This method returns a <see cref="ThalesError"/> object that contains
    ''' help for the specific error code.
    ''' </remarks>
    Public Shared Function GetError(ByVal errorCode As String) As ThalesError
        For i As Integer = 0 To _errors.GetUpperBound(0)
            If _errors(i).ErrorCode = errorCode Then
                Return _errors(i)
            End If
        Next
        Return Nothing
    End Function

End Class