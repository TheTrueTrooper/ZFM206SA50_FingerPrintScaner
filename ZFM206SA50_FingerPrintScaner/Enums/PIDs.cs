using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public enum PIDs : byte
    {
        CommandPacket = 0x01,
        DataPacket = 0x02,
        AcknowledgePacket = 0x07,
        DataEndPacket = 0x08
    }
}
