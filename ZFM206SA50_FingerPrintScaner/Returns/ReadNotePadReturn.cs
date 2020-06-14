using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public class ReadNotePadReturn : BasicCommandReturn
    {
        static Encoding AssumedEncoding = Encoding.ASCII;

        private ReadNotePadReturn() { }

        public byte[] RawBytes { internal set; get; }
        public Encoding EncodingAs = AssumedEncoding;
        public string String => EncodingAs.GetString(RawBytes);

        public static implicit operator ReadNotePadReturn(RecievePackage Package)
        {
            byte[] RawBytes = new byte[32];
            Array.Copy(Package.Data, 1, RawBytes, 0, 32);
            return new ReadNotePadReturn()
            {
                DeviceAddress = Package.DeviceAddress,
                Valid = Package.Valid,
                Status = (Errors)Package.Data[0],
                RawBytes = RawBytes,
            };
        }

        public override string ToString()
        {
            const string ReturnBase = "Finger Print Scanner Package Returned:\n" +
                "For Device: {0}\n" +
                "Status: {1}\n" +
                "IsValid: {2}\n" +
                "Raw Bytes: {3}\n" +
                "Assumed Encoding: {4}\n" +
                "Currently Encoding As: {5}\n" +
                "String: {6}\n";

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, BitConverter.ToString(RawBytes).Replace("-", ""), AssumedEncoding, EncodingAs, String);
        }
    }
}
