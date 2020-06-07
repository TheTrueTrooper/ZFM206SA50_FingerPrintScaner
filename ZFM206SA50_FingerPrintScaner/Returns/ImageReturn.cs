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
        const int ImageWidth = 256;
        const int ImageHieght = 288;

        public Image GrayScaleImage { private set; get; }

        public byte[] ImageBuffer { private set; get; }

        public static implicit operator ImageReturn(StreamReturn StreamReturn)
        {
            byte[] RawBuffer = StreamReturn.Stream.ToArray();
            byte[] ImageBuffer = new byte[RawBuffer.Length * 2];
            for(int i = 0; i < RawBuffer.Length; i++)
            {
                //ImageBuffer[i * 2] = (byte)(RawBuffer[i] & 0xF0);
                //ImageBuffer[i * 2 + 1] = (byte)((RawBuffer[i] & 0x0F) << 4);
                ImageBuffer[i * 2] = (byte)((RawBuffer[i] & 0xF0)|0xF);
                ImageBuffer[i * 2 + 1] = (byte)(((RawBuffer[i] & 0x0F) << 4)|0x0F);
            }

            Bitmap Image = new Bitmap(ImageWidth, ImageHieght, PixelFormat.Format8bppIndexed);
            BitmapData ImageData = Image.LockBits(new Rectangle(0, 0,
                                                                Image.Width,
                                                                Image.Height),
                                                  ImageLockMode.WriteOnly,
                                                  Image.PixelFormat);
            IntPtr pNative = ImageData.Scan0;
            Marshal.Copy(ImageBuffer, 0, pNative, ImageBuffer.Length);

            ColorPalette Palette = Image.Palette;
            Color[] Entries = Palette.Entries;
            for (int i = 0; i < 256; i++)
            {
                Color Value = new Color();
                Value = Color.FromArgb((byte)i, (byte)i, (byte)i);
                Entries[i] = Value;
            }
            Image.Palette = Palette;

            Image.UnlockBits(ImageData);

            return new ImageReturn()
            {
                DeviceAddress = StreamReturn.DeviceAddress,
                Valid = StreamReturn.Valid,
                Status = StreamReturn.Status,
                ImageBuffer = ImageBuffer,
                GrayScaleImage = Image
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
