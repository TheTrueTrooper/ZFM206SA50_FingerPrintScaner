using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public class SearchForMatchReturn : MatchReturn
    {
        private SearchForMatchReturn(){}

        public ushort PageIndexOfMatch { private set; get; }

        public static implicit operator SearchForMatchReturn(RecievePackage Package)
        {
            return new SearchForMatchReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0],
                MatchingScore = BitConverterHelpers.GetU16(Package.Data, 1),
                PageIndexOfMatch = BitConverterHelpers.GetU16(Package.Data, 1)
            };
        }

        public override string ToString()
        {
            const string ReturnBase = "Finger Print Scanner Package Returned:\n" +
                "For Device: {0}\n" +
                "Status: {1}\n" +
                "IsValid: {2}\n" +
                "Matching Score: {3}\n" +
                "Page Index Of Match: {4}\n";

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, MatchingScore, PageIndexOfMatch);
        }
    }
}
