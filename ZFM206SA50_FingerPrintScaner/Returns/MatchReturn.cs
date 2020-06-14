using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public class MatchReturn : BasicCommandReturn
    {
        protected MatchReturn(){}

        public ushort MatchingScore { internal protected set; get; }

        public static implicit operator MatchReturn(RecievePackage Package)
        {
            return new MatchReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0],
                MatchingScore = BitConverterHelpers.GetU16(Package.Data, 1),
            };
        }

        public override string ToString()
        {
            const string ReturnBase = "Finger Print Scanner Package Returned:\n" +
                "For Device: {0}\n" +
                "Status: {1}\n" +
                "IsValid: {2}\n" +
                "Matching Score: {3}\n";

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, MatchingScore);
        }

    }
}
