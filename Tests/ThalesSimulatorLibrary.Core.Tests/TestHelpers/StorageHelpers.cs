using ThalesSimulatorLibrary.Core.Cryptography.LMK;

namespace ThalesSimulatorLibrary.Core.Tests.TestHelpers
{
    public static class StorageHelpers
    {
        public static void ReadLmks()
        {
            Storage.ReadLmks("lmk.txt");
        }
    }
}
