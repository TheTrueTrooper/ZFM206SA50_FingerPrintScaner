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
        private ReadSystemParamReturn(){}

        public SystemStatus StatusRegister { private set; get; }
        public ushort SystemIdentifierCode { private set; get; }
        public ushort FingerLibrarySize { private set; get; }
        public SecurityLevels SecurityLevel { private set; get; }
        public uint DeviceAddressFromStatus { private set; get; }
        public PackageLength DataPackageSize { private set; get; }
        public BaudRates BaudRate { private set; get; }


        public static implicit operator ReadSystemParamReturn(RecievePackage Package)
        {
            return new ReadSystemParamReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0],
                StatusRegister = (SystemStatus)BitConverterHelpers.GetU16(Package.Data, 1),
                SystemIdentifierCode = BitConverterHelpers.GetU16(Package.Data, 3),
                FingerLibrarySize = BitConverterHelpers.GetU16(Package.Data, 5),
                SecurityLevel = (SecurityLevels)Package.Data[8],
                DeviceAddressFromStatus = BitConverterHelpers.GetU32(Package.Data, 9),
                DataPackageSize = (PackageLength)Package.Data[14],
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

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, StatusRegister, 
                SystemIdentifierCode.ToString("X4"), FingerLibrarySize, SecurityLevel, DeviceAddressFromStatus.ToString("X4"),
                DataPackageSize, BaudRate);
        }
    }
}
