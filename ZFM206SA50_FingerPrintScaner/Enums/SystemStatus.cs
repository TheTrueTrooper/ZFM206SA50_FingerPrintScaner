using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    [Flags]
    public enum SystemStatus : ushort
    {
        Busy = 0x0001,
        LastFingerWasMatched = 0x0002,
        DeviceHandShakingPasword = 0x0004,
        ImageBufferHasValidFingerImage = 0x0008,
        Reserved4 = 0x0010,
        Reserved5 = 0x0020,
        Reserved6 = 0x0040,
        Reserved7 = 0x0080,
        Reserved8 = 0x0100,
        Reserved9 = 0x0200,
        Reserved10 = 0x0400,
        Reserved11 = 0x0800,
        Reserved12 = 0x1000,
        Reserved13 = 0x2000,
        Reserved14 = 0x4000,
        Reserved15 = 0x8000,
    }
}
