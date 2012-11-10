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

namespace ThalesSim.Core.Cryptography.PIN
{
    /// <summary>
    /// This class is used to encrypt/decrypt PINs. Currently implementations are dummy.
    /// </summary>
    public class Encrypt
    {
        /// <summary>
        /// Encrypts a PIN for host storage.
        /// </summary>
        /// <param name="pin">Clear PIN.</param>
        /// <returns>Encrypted PIN.</returns>
        public static string EncryptPinForHostStorage (string pin)
        {
            return "0" + pin;
        }

        /// <summary>
        /// Decrypts a PIN encrypted for host storage.
        /// </summary>
        /// <param name="encryptedPin">Encrypted PIN.</param>
        /// <returns>Clear PIN.</returns>
        public static string DecryptPinUnderHostStorage (string encryptedPin)
        {
            return encryptedPin.Substring(1);
        }

        /// <summary>
        /// Encrypts a PIN for host storage using the Thales method.
        /// </summary>
        /// <param name="pin">Clear PIN.</param>
        /// <returns>Encrypted PIN.</returns>
        public static string EncryptPinForHostStorageThales (string pin)
        {
            return EncryptPinForHostStorage(pin);
        }

        /// <summary>
        /// Decrypts a PIN encrypted for host storage using the Thales method.
        /// </summary>
        /// <param name="encryptedPin">Encrypted PIN.</param>
        /// <returns>Clear PIN.</returns>
        public static string DecryptPinUnderHostStorageThales (string encryptedPin)
        {
            return DecryptPinUnderHostStorage(encryptedPin);
        }
    }
}
