using System;

namespace ThalesSimulatorLibrary.Core.Exceptions
{
    public class InvalidVariantException : Exception
    {
        public InvalidVariantException() { }
        public InvalidVariantException(string message) : base(message) { }
    }
}