using System.Runtime.InteropServices;

namespace VendaPues.Backend.Helpers
{
    public interface IRuntimeInformationWrapper
    {
        bool IsOSPlatform(OSPlatform osPlatform);
    }
}