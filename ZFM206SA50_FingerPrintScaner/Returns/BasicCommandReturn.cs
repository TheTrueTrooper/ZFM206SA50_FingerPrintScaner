using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public class BasicCommandReturn
    {
        public uint DeviceAddress;
        public bool Valid;
#pragma warning disable CS0414
        public Errors Status;
#pragma warning restore CS0414

        internal BasicCommandReturn()
        {

        }

        public static implicit operator BasicCommandReturn(RecievePackage Package)
        {
            return new BasicCommandReturn()
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
                "IsValid: {2}\n";

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid);
        }
    }
}
