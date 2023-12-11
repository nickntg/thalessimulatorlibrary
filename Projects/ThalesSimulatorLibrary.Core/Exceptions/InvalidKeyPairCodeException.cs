using System;

namespace ThalesSimulatorLibrary.Core.Exceptions
{
    public class InvalidKeyPairCodeException : Exception
    {
        public InvalidKeyPairCodeException() { }
        public InvalidKeyPairCodeException(string message) : base(message) { }
    }
}
