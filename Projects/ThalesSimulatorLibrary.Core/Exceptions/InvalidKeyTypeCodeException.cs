using System;

namespace ThalesSimulatorLibrary.Core.Exceptions
{
    public class InvalidKeyTypeCodeException : Exception
    {
        public InvalidKeyTypeCodeException() { }
        public InvalidKeyTypeCodeException(string message) : base(message) { }
    }
}