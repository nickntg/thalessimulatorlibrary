namespace ThalesSimulatorLibrary.Core.Cryptography.LMK
{
    public enum LmkPair
    {
        /// <summary>
        /// LMK pair 00-01.
        /// </summary>
        /// <remarks>
        /// Contains the two smart card ""keys"" (Passwords if the HSM is configured for Password mode) required for setting the HSM into the Authorized state.
        /// </remarks>
        Pair0001 = 0,
        /// <summary>
        /// LMK pair 02-03.
        /// </summary>
        /// <remarks>
        /// Encrypts the PINs for host storage.
        /// </remarks>
        ///
        Pair0203 = 1,
        /// <summary>
        /// LMK pair 04-05.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Master Keys and double-length ZMKs. Encrypts Zone Master Key components under a Variant.
        /// </remarks>
        Pair0405 = 2,
        /// <summary>
        /// LMK pair 06-07.
        /// </summary>
        /// <remarks>
        /// Encrypts the Zone PIN keys for interchange transactions.
        /// </remarks>
        Pair0607 = 3,
        /// <summary>
        /// LMK pair 08-09.
        /// </summary>
        /// <remarks>
        /// Used for random number generation.
        /// </remarks>
        Pair0809 = 4,
        /// <summary>
        /// LMK pair 10-11.
        /// </summary>
        /// <remarks>
        /// Used for encrypting keys in HSM buffer areas.
        /// </remarks>
        Pair1011 = 5,
        /// <summary>
        /// LMK pair 12-13.
        /// </summary>
        /// <remarks>
        /// The initial set of Secret Values created by the user; used for generating all other Master Key pairs.
        /// </remarks>
        Pair1213 = 6,
        /// <summary>
        /// LMK pair 14-15.
        /// </summary>
        /// <remarks>
        /// Encrypts Terminal Master Keys, Terminal PIN Keys and PIN Verification Keys. Encrypts Card Verification Keys under a Variant.
        /// </remarks>
        Pair1415 = 7,
        /// <summary>
        /// LMK pair 16-17.
        /// </summary>
        /// <remarks>
        /// Encrypts Terminal Authentication Keys.
        /// </remarks>
        Pair1617 = 8,
        /// <summary>
        /// LMK pair 18-19
        /// </summary>
        /// <remarks>
        /// Encrypts reference numbers for solicitation mailers.
        /// </remarks>
        Pair1819 = 9,
        /// <summary>
        /// LMK pair 20-21.
        /// </summary>
        /// <remarks>
        /// Encrypts 'not on us' PIN Verification Keys and Card Verification Keys under a Variant.
        /// </remarks>
        Pair2021 = 10,
        /// <summary>
        /// LMK pair 22-23.
        /// </summary>
        /// <remarks>
        /// Encrypts Watchword Keys.
        /// </remarks>
        Pair2223 = 11,
        /// <summary>
        /// LMK pair 24-25.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Transport Keys.
        /// </remarks>
        Pair2425 = 12,
        /// <summary>
        /// LMK pair 26-27.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Authentication Keys.
        /// </remarks>
        Pair2627 = 13,
        /// <summary>
        /// LMK pair 28-29.
        /// </summary>
        /// <remarks>
        /// Encrypts Terminal Derivation Keys.
        /// </remarks>
        Pair2829 = 14,
        /// <summary>
        /// LMK pair 30-31.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Encryption Keys.
        /// </remarks>
        Pair3031 = 15,
        /// <summary>
        /// LMK pair 32-33.
        /// </summary>
        /// <remarks>
        /// Encrypts Data Encryption Keys.
        /// </remarks>
        Pair3233 = 16,
        /// <summary>
        /// LMK pair 34-35.
        /// </summary>
        /// <remarks>
        /// Encrypts RSA keys.
        /// </remarks>
        Pair3435 = 17,
        /// <summary>
        /// LMK pair 36-37.
        /// </summary>
        /// <remarks>
        /// Encrypts RSA MAC keys.
        /// </remarks>
        Pair3637 = 18,
        /// <summary>
        /// LMK pair 38-39.
        /// </summary>
        /// <remarks>
        /// LMK pair 38-39.
        /// </remarks>
        Pair3839 = 19
    }
}