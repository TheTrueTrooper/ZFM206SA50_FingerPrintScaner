using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner.Enums
{
    public enum BaudRates : byte
    {
        BaudRateOf9600 = 0x01,
        BaudRateOf19200 = 0x02,
        BaudRateOf28800 = 0x03,
        BaudRateOf38400 = 0x04,
        BaudRateOf48000 = 0x05,
        BaudRateOf57600 = 0x06,
        BaudRateOf67200 = 0x07,
        BaudRateOf76800 = 0x08,
        BaudRateOf86400 = 0x09,
        BaudRateOf96000 = 0x0A,
        BaudRateOf105600 = 0x0B,
        BaudRateOf115200 = 0x0C,
    }
}
