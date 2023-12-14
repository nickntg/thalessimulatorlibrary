namespace ThalesSimulatorLibrary.Core.Cryptography.PIN
{
    public enum PinBlockFormat
    {
        Unspecified = 0,
        AnsiX98 = 1,
        Docutel = 2,
        Diebold = 3,
        Plus = 4,
        Iso95641Format1 = 5,
        Emv = 34,
        PayNowPayLater = 35,
        PinChangeWithoutOldPin = 41,
        PinChangeWithOldPin = 42,
        Iso95641Format3 = 47
    }
}
