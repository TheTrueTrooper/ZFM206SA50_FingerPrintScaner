using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner.Enums
{
    public enum PackageLength : byte
    {
        LengthOf32 = 0x00,
        LengthOf64 = 0x01,
        LengthOf128 = 0x02,
        LengthOf256 = 0x03,
    }
}
