using System;

namespace ThalesSimulatorLibrary.Core.Exceptions
{
    public class InvalidKeySchemeException : Exception
    {
        public InvalidKeySchemeException() { }
        public InvalidKeySchemeException(string message) : base(message) { }
    }
}
