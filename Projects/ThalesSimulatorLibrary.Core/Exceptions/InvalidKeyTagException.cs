using System;

namespace ThalesSimulatorLibrary.Core.Exceptions
{
    public class InvalidKeyTagException : Exception
    {
        public InvalidKeyTagException() { }
        public InvalidKeyTagException(string message) : base(message) { }
    }
}
