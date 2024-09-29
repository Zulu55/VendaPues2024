using System.Runtime.InteropServices;

namespace VendaPues.Backend.Helpers
{
    public class RuntimeInformationWrapper : IRuntimeInformationWrapper
    {
        public bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
    }
}