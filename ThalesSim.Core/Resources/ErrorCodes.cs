/*
 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

namespace ThalesSim.Core.Resources
{
    public class ErrorCodes
    {
        /// <summary>
        /// Thales error code 00.
        /// </summary>
        /// <remarks>No error.</remarks>    
        public const string ER_00_NO_ERROR = "00";

        /// <summary>
        /// Thales error code 01.
        /// </summary>
        /// <remarks>
        /// Verification failure.
        /// </remarks>
        public const string ER_01_VERIFICATION_FAILURE = "01";

        /// <summary>
        /// Thales error code 02.
        /// </summary>
        /// <remarks>
        /// Inappropriate key length for algorithm.
        /// </remarks>
        public const string ER_02_INAPPROPRIATE_KEY_LENGTH_FOR_ALGORITHM = "02";

        /// <summary>
        /// Thales error code 03.
        /// </summary>
        /// <remarks>
        /// Invalid number of components.
        /// </remarks>
        public const string ER_03_INVALID_NUMBER_OF_COMPONENTS = "03";

        /// <summary>
        /// Thales error code 04.
        /// </summary>
        /// <remarks>
        /// Invalid key type code.
        /// </remarks>
        public const string ER_04_INVALID_KEY_TYPE_CODE = "04";

        /// <summary>
        /// Thales error code 05.
        /// </summary>
        /// <remarks>
        /// Invalid key length flag.
        /// </remarks>
        public const string ER_05_INVALID_KEY_LENGTH_FLAG = "05";

        /// <summary>
        /// Thales error code 05.
        /// </summary>
        /// <remarks>
        /// Invalid hash identifier.
        /// </remarks>
        public const string ER_05_INVALID_HASH_IDENTIFIER = "05";

        /// <summary>
        /// Thales error code 10.
        /// </summary>
        /// <remarks>
        /// Source key parity error.
        /// </remarks>
        public const string ER_10_SOURCE_KEY_PARITY_ERROR = "10";

        /// <summary>
        /// Thales error code 11.
        /// </summary>
        /// <remarks>
        /// Destination key parity error.
        /// </remarks>
        public const string ER_11_DESTINATION_KEY_PARITY_ERROR = "11";

        /// <summary>
        /// Thales error code 12.
        /// </summary>
        /// <remarks>
        /// Contents of user storage not available.
        /// </remarks>
        public const string ER_12_CONTENTS_OF_USER_STORAGE_NOT_AVAILABLE = "12";

        /// <summary>
        /// Thales error code 13.
        /// </summary>
        /// <remarks>
        /// Master key parity error.
        /// </remarks>
        public const string ER_13_MASTER_KEY_PARITY_ERROR = "13";

        /// <summary>
        /// Thales error code 14.
        /// </summary>
        /// <remarks>
        /// PIN encrypted under LMK pair 02-03 is invalid.
        /// </remarks>
        public const string ER_14_PIN_ENCRYPTED_UNDER_LMK_PAIR_02_03_IS_INVALID = "14";

        /// <summary>
        /// Thales error code 15.
        /// </summary>
        /// <remarks>
        /// Invalid input data.
        /// </remarks>
        public const string ER_15_INVALID_INPUT_DATA = "15";

        /// <summary>
        /// Thales error code 16.
        /// </summary>
        /// <remarks>
        /// Console or printer not ready/not connected.
        /// </remarks>
        public const string ER_16_CONSOLE_OR_PRINTER_NOT_READY_NOT_CONNECTED = "16";

        /// <summary>
        /// Thales error code 17.
        /// </summary>
        /// <remarks>
        /// HSM is not in the authorized state.
        /// </remarks>
        public const string ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE = "17";

        /// <summary>
        /// Thales error code 18.
        /// </summary>
        /// <remarks>
        /// Document definition not loaded.
        /// </remarks>
        public const string ER_18_DOCUMENT_DEFINITION_NOT_LOADED = "18";

        /// <summary>
        /// Thales error code 19.
        /// </summary>
        /// <remarks>
        /// Specified Diebold table is invalid.
        /// </remarks>
        public const string ER_19_SPECIFIED_DIEBOLD_TABLE_IS_INVALID = "19";

        /// <summary>
        /// Thales error code 20.
        /// </summary>
        /// <remarks>
        /// PIN block does not contain valid values.
        /// </remarks>
        public const string ER_20_PIN_BLOCK_DOES_NOT_CONTAIN_VALID_VALUES = "20";

        /// <summary>
        /// Thales error code 21.
        /// </summary>
        /// <remarks>
        /// Invalid index value.
        /// </remarks>
        public const string ER_21_INVALID_INDEX_VALUE = "21";

        /// <summary>
        /// Thales error code 22.
        /// </summary>
        /// <remarks>
        /// Invalid account number.
        /// </remarks>
        public const string ER_22_INVALID_ACCOUNT_NUMBER = "22";

        /// <summary>
        /// Thales error code 23.
        /// </summary>
        /// <remarks>
        /// Invalid PIN block format code.
        /// </remarks>
        public const string ER_23_INVALID_PIN_BLOCK_FORMAT_CODE = "23";

        /// <summary>
        /// Thales error code 24.
        /// </summary>
        /// <remarks>
        /// PIN is fewer than 4 or more than 12 digits long.
        /// </remarks>
        public const string ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG = "24";

        /// <summary>
        /// Thales error code 25.
        /// </summary>
        /// <remarks>
        /// Decimalization table error.
        /// </remarks>
        public const string ER_25_DECIMALIZATION_TABLE_ERROR = "25";

        /// <summary>
        /// Thales error code 26.
        /// </summary>
        /// <remarks>
        /// Invalid key scheme.
        /// </remarks>
        public const string ER_26_INVALID_KEY_SCHEME = "26";

        /// <summary>
        /// Thales error code 27.
        /// </summary>
        /// <remarks>
        /// Incompatible key length.
        /// </remarks>
        public const string ER_27_INCOMPATIBLE_KEY_LENGTH = "27";

        /// <summary>
        /// Thales error code 28.
        /// </summary>
        /// <remarks>
        /// Invalid key type.
        /// </remarks>
        public const string ER_28_INVALID_KEY_TYPE = "28";

        /// <summary>
        /// Thales error code 29.
        /// </summary>
        /// <remarks>
        /// Function not permitted.
        /// </remarks>
        public const string ER_29_FUNCTION_NOT_PERMITTED = "29";

        /// <summary>
        /// Thales error code 30.
        /// </summary>
        /// <remarks>
        /// Invalid reference number.
        /// </remarks>
        public const string ER_30_INVALID_REFERENCE_NUMBER = "30";

        /// <summary>
        /// Thales error code 31.
        /// </summary>
        /// <remarks>
        /// Insuficcient solicitation entries for batch.
        /// </remarks>
        public const string ER_31_INSUFICCIENT_SOLICITATION_ENTRIES_FOR_BATCH = "31";

        /// <summary>
        /// Thales error code 33.
        /// </summary>
        /// <remarks>
        /// LMK key change storage is corrupted.
        /// </remarks>
        public const string ER_33_LMK_KEY_CHANGE_STORAGE_IS_CORRUPTED = "33";

        /// <summary>
        /// Thales error code 40.
        /// </summary>
        /// <remarks>
        /// Invalid firmware checksum.
        /// </remarks>
        public const string ER_40_INVALID_FIRMWARE_CHECKSUM = "40";

        /// <summary>
        /// Thales error code 41.
        /// </summary>
        /// <remarks>
        /// Internal hardware/software error.
        /// </remarks>
        public const string ER_41_INTERNAL_HARDWARE_SOFTWARE_ERROR = "41";

        /// <summary>
        /// Thales error code 42.
        /// </summary>
        /// <remarks>
        /// DES failure.
        /// </remarks>
        public const string ER_42_DES_FAILURE = "42";

        /// <summary>
        /// Error code 51 
        /// </summary>
        /// <remarks>Invalid message header</remarks>
        public const string ER_51_INVALID_MESSAGE_HEADER = "51";

        /// <summary>
        /// Error code 52.
        /// </summary>
        /// <remarks>Invalid Number of Commands field.</remarks>
        public const string ER_52_INVALID_NUMBER_OF_COMMANDS = "52";

        /// <summary>
        /// Thales error code 80.
        /// </summary>
        /// <remarks>
        /// Data length error.
        /// </remarks>
        public const string ER_80_DATA_LENGTH_ERROR = "80";

        /// <summary>
        /// Thales error code 90.
        /// </summary>
        /// <remarks>
        /// Data parity error.
        /// </remarks>
        public const string ER_90_DATA_PARITY_ERROR = "90";

        /// <summary>
        /// Thales error code 91.
        /// </summary>
        /// <remarks>
        /// LRC error.
        /// </remarks>
        public const string ER_91_LRC_ERROR = "91";

        /// <summary>
        /// Thales error code 92.
        /// </summary>
        /// <remarks>
        /// Count value not between limits.
        /// </remarks>
        public const string ER_92_COUNT_VALUE_NOT_BETWEEN_LIMITS = "92";

        /// <summary>
        /// Thales error code ZZ.
        /// </summary>
        /// <remarks>
        /// This error code may be internally used.
        /// </remarks>
        public const string ER_ZZ_UNKNOWN_ERROR = "ZZ";
    }
}
