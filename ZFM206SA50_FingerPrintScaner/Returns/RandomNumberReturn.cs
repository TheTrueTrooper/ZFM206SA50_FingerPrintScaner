using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public class RandomNumberReturn : BasicCommandReturn
    {
        protected RandomNumberReturn(){}

        public ushort UnsignedRandomNumber { private set; get; }
        public short SignedRandomNumber => (short)UnsignedRandomNumber;

        public static implicit operator RandomNumberReturn(RecievePackage Package)
        {
            return new RandomNumberReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0],
                UnsignedRandomNumber = BitConverterHelpers.GetU16(Package.Data, 1),
            };
        }

        public override string ToString()
        {
            const string ReturnBase = "Finger Print Scanner Package Returned:\n" +
                "For Device: {0}\n" +
                "Status: {1}\n" +
                "IsValid: {2}\n" +
                "Unsigned Random Number: {3}\n" +
                "Signed Random Number: {4}\n";

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, UnsignedRandomNumber, SignedRandomNumber);
        }

    }
}
