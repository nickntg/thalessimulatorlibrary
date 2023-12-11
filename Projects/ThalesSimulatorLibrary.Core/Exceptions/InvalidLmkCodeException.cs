using System;

namespace ThalesSimulatorLibrary.Core.Exceptions
{
    public class InvalidLmkCodeException : Exception
    {
        public InvalidLmkCodeException() { }
        public InvalidLmkCodeException(string message) : base(message) { }
    }
}
