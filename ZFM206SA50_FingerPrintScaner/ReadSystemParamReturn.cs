using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZFM206SA50_FingerPrintScaner.Enums;

namespace ZFM206SA50_FingerPrintScaner
{
    public class ReadSystemParamReturn : BasicCommandReturn
    {
        public ushort StatusRegister;
        public ushort SystemIdentifierCode;
        public ushort FingerLibrarySize;
        public SecurityLevels SecurityLevel;
        public uint DeviceAddressFromStatus;
        public PackageLength DataPackageSize;
        public BaudRates BaudRate;


        internal ReadSystemParamReturn()
        {

        }

        static ushort GetU16(byte[] Data, int IndexStart)
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

        static uint GetU32(byte[] Data, int IndexStart)
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

        public static implicit operator ReadSystemParamReturn(RecievePackage Package)
        {
            return new ReadSystemParamReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0],
                StatusRegister = GetU16(Package.Data, 1),
                SystemIdentifierCode = GetU16(Package.Data, 3),
                FingerLibrarySize = GetU16(Package.Data, 5),
                SecurityLevel = (SecurityLevels)Package.Data[8],
                DeviceAddressFromStatus = GetU32(Package.Data, 9),
                DataPackageSize = (PackageLength)14,
                BaudRate = (BaudRates)Package.Data[16]
            };
        }
    }
}
