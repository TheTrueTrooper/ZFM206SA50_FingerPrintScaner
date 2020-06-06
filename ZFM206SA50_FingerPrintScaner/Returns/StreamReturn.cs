using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ZFM206SA50_FingerPrintScaner
{
    public class StreamReturn : BasicCommandReturn
    {
        public MemoryStream Stream { internal set; get; } = new MemoryStream();

        public static implicit operator StreamReturn(RecievePackage Package)
        {
            return new StreamReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0]
            };
        }

        public override string ToString()
        {
            const string ReturnBase = "Finger Print Scanner Package Returned:\n" +
                "For Device: {0}\n" +
                "Status: {1}\n" +
                "IsValid: {2}\n" +
                "Stream Data is as follows:\n" +
                "{3}\n";

            byte[] Buffer = Stream.ToArray();
            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, BitConverter.ToString(Buffer).Replace("-", ""));
        }
    }
}
