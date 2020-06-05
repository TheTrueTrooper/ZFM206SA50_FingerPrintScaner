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

        public override string ToString()
        {
            const string ReturnBase = "Finger Print Scanner Package Returned:\n" +
                "For Device: {0}\n" +
                "Status: {1}\n" +
                "IsValid: {2}\n" +
                "Status Register: {3}\n" +
                "System Identifier Code: {4}\n" +
                "Finger Libary Size: {5}\n" +
                "Security Level: {6}\n" +
                "Device Address From Status: {7}\n" +
                "Data Package Size: {8}\n" +
                "Baud Rate: {9}:\n";

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, StatusRegister.ToString("X4"), 
                SystemIdentifierCode.ToString("X4"), FingerLibrarySize, SecurityLevel, DeviceAddressFromStatus.ToString("X4"),
                DataPackageSize, BaudRate);
        }
    }
}
