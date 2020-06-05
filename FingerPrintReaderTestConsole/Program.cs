using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZFM206SA50_FingerPrintScaner;
//using System.IO.Ports;

namespace FingerPrintReaderTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //SerialPort SerialPort = new SerialPort("Com3", 57600, Parity.None, 8, StopBits.One);
            //SerialPort.Open();
            //byte[] Pakage = new byte[] { 0xEF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x04, 0x17, 0x00, 0x00, 0x1C };
            //SerialPort.Write(Pakage, 0, Pakage.Length);
            //FingerPrintScaner Tester = new FingerPrintScaner("Com3");
            //Tester.InitHandShake();
            //for(int i = 0; i < 12; i++)
            //    Console.Write(((byte)Tester.SerialPort.ReadByte()).ToString("X2"));
            //Console.ReadKey();
            //Tester.SetAddress(0x0001);
            //for (int i = 0; i < 12; i++)
            //    Console.Write(((byte)Tester.SerialPort.ReadByte()).ToString("X2"));
            //Console.ReadKey();
            //Tester.SetAddress(0xFFFFFFFF);
            //for (int i = 0; i < 12; i++)
            //    Console.Write(((byte)Tester.SerialPort.ReadByte()).ToString("X2"));
            //Console.ReadKey();

            FingerPrintScaner Tester = new FingerPrintScaner("Com3");
            BasicCommandReturn Package = Tester.InitHandShake();
            ReadSystemParamReturn Status = Tester.ReadSystemParam();
            //while (true)
            //{
            //    Package = Tester.GenerateImage();
            //    if (Package.Status == Errors.Success)
            //        Console.WriteLine("There is a finger on the sensor!");
            //    else
            //        Console.WriteLine("There is no finger on the sensor....");
            //    Thread.Sleep(500);
            //}
            Package = Tester.GenerateImage();
            Package = Tester.UploadImage();
            while(true)
            {
                Console.Write(((byte)Tester.SerialPort.ReadByte()).ToString("X2"));
            }
        }
    }
}
