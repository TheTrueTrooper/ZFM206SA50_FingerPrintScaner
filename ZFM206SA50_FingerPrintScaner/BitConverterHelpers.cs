using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    static class BitConverterHelpers
    {
        internal static ushort GetU16(byte[] Data, int IndexStart)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] Number = new byte[2];
                Array.Copy(Data, IndexStart, Number, 0, 2);
                Array.Reverse(Number);
                return BitConverter.ToUInt16(Number, 0);
            }
            else
                return BitConverter.ToUInt16(Data, IndexStart);
        }

        internal static uint GetU32(byte[] Data, int IndexStart)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] Number = new byte[4];
                Array.Copy(Data, IndexStart, Number, 0, 4);
                Array.Reverse(Number);
                return BitConverter.ToUInt32(Number, 0);
            }
            else
                return BitConverter.ToUInt32(Data, IndexStart);
        }
    }
}
