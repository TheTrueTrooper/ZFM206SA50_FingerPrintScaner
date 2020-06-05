using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner.Enums
{
    enum SystemParam : byte
    {
        BaudRate = 0x04,
        SecurityLevel = 0x05,
        DataPackageLength = 0x06
    }
}
