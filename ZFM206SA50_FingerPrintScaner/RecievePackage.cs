using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZFM206SA50_FingerPrintScaner.Enums;

namespace ZFM206SA50_FingerPrintScaner
{
    public class RecievePackage
    {
        internal PIDs PakageType;
        internal uint DeviceAddress;
        internal byte[] Data;
        internal bool Valid;

        internal RecievePackage(PIDs PakageType, uint DeviceAddress, byte[] Data, ushort CheckSum)
        {
            this.PakageType = PakageType;
            this.DeviceAddress = DeviceAddress;
            this.Data = Data;
            ushort Check;
            if (BitConverter.IsLittleEndian)
            {
                byte[] Number = new byte[2];
                Array.Copy(Data, Data.Length - 2, Number, 0, 2);
                Array.Reverse(Number);
                Check = BitConverter.ToUInt16(Number, 0);
            }
            else
                Check = BitConverter.ToUInt16(Data, 7);
            for (byte i = 0; i < Data.Length - 2; i++)
                CheckSum += Data[i];
            Valid = CheckSum == Check;
        }
    }
}
