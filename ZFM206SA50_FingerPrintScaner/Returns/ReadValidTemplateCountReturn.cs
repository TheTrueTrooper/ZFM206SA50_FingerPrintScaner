using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public class ReadValidTemplateCountReturn : BasicCommandReturn
    {
        public ushort ValidTemplateCount;

        public static implicit operator ReadValidTemplateCountReturn(RecievePackage Package)
        {
            return new ReadValidTemplateCountReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0],
                ValidTemplateCount = BitConverterHelpers.GetU16(Package.Data, 1)
            };
        }

        public override string ToString()
        {
            const string ReturnBase = "Finger Print Scanner Package Returned:\n" +
                "For Device: {0}\n" +
                "Status: {1}\n" +
                "IsValid: {2}\n" +
                "Valid Template Count: {3}\n";

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, ValidTemplateCount);
        }
    }
}
