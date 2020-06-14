using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public class BasicCommandReturn
    {
        protected BasicCommandReturn(){}

        public uint DeviceAddress { internal protected set; get; }
        public bool Valid { internal protected set; get; }
#pragma warning disable CS0414
        public Errors Status { internal protected set; get; }
#pragma warning restore CS0414

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
