using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ZFM206SA50_FingerPrintScaner
{
    public class ImageReturn : BasicCommandReturn
    {
        public Image GrayScaleImage { private set; get; }

        public byte[] ImageBuffer { private set; get; }

        public static implicit operator ImageReturn(StreamReturn StreamReturn)
        {
            byte[] ImageBytes = StaticImageHelpers.GetUncompressedImageBytes(StreamReturn.Stream.ToArray());

            return new ImageReturn()
            {
                DeviceAddress = StreamReturn.DeviceAddress,
                Valid = StreamReturn.Valid,
                Status = StreamReturn.Status,
                ImageBuffer = ImageBytes,
                GrayScaleImage = StaticImageHelpers.GetImageFromBytes(ImageBytes)
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

            return string.Format(ReturnBase, DeviceAddress.ToString("X4"), Status, Valid, BitConverter.ToString(ImageBuffer).Replace("-",""));
        }
    }
}
