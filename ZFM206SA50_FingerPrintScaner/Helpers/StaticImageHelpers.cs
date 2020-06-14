using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    static class StaticImageHelpers
    {
        internal const uint ImageByteCount = 73728;

        internal const int ImageWidth = 256;
        internal const int ImageHieght = 288;

        internal static byte[] GetUncompressedImageBytes(byte[] RawCompressedBuffer)
        {
            byte[] RawUncompressedImageBuffer = new byte[RawCompressedBuffer.Length * 2];
            for (int i = 0; i < RawCompressedBuffer.Length; i++)
            {
                //ImageBuffer[i * 2] = (byte)(RawBuffer[i] & 0xF0);
                //ImageBuffer[i * 2 + 1] = (byte)((RawBuffer[i] & 0x0F) << 4);
                RawUncompressedImageBuffer[i * 2] = (byte)((RawCompressedBuffer[i] & 0xF0) | 0xF);
                RawUncompressedImageBuffer[i * 2 + 1] = (byte)(((RawCompressedBuffer[i] & 0x0F) << 4) | 0x0F);
            }
            return RawUncompressedImageBuffer;
        }

        internal static byte[] GetCompressedImageBytes(byte[] RawUncompressedImageBuffer)
        {
            byte[] RawCompressedBuffer = new byte[RawUncompressedImageBuffer.Length / 2];
            for (int i = 0; i < RawCompressedBuffer.Length; i++)
            {
                RawCompressedBuffer[i] = (byte)((RawUncompressedImageBuffer[i * 2] & 0xF0) | ((RawUncompressedImageBuffer[i * 2 + 1] & 0xF0) >> 4));
            }
            return RawCompressedBuffer;
        }

        internal static Image GetImageFromBytes(byte[] RawUncompressedImageBuffer)
        {
            Bitmap Image = new Bitmap(ImageWidth, ImageHieght, PixelFormat.Format8bppIndexed);
            BitmapData ImageData = Image.LockBits(new Rectangle(0, 0,
                                                                Image.Width,
                                                                Image.Height),
                                                  ImageLockMode.WriteOnly,
                                                  Image.PixelFormat);
            IntPtr pNative = ImageData.Scan0;
            Marshal.Copy(RawUncompressedImageBuffer, 0, pNative, RawUncompressedImageBuffer.Length);

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

            return Image;
        }

        internal static byte[] GetBytesFromImage(Image Image)
        {
            return GetBytesFromImage(new Bitmap(Image));
        }

        internal static byte[] GetBytesFromImage(Bitmap Image)
        {
            byte[] Return = new byte[Image.Width * Image.Height];
            for (int y = 0; y < Image.Height; y++)
                for (int x = 0; x < Image.Width; x++)
                {
                    Color Pixel = Image.GetPixel(x, y);
                    Return[x + y * Image.Width] = (byte)(Pixel.R * 0.3 + Pixel.B * 0.11 + Pixel.G * 0.59);
                }

            return Return;
        }
    }
}
