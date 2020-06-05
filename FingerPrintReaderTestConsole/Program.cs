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
            #region OldTestCode1
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
            #endregion

            FingerPrintScaner Tester = new FingerPrintScaner("Com4");
            //BasicCommandReturn Package = 
            Tester.InitHandShake();
            //ReadSystemParamReturn Status = Tester.ReadSystemParam();

            #region OldTestCode2
            //while (true)
            //{
            //    Package = Tester.GenerateImage();
            //    if (Package.Status == Errors.Success)
            //        Console.WriteLine("There is a finger on the sensor!");
            //    else
            //        Console.WriteLine("There is no finger on the sensor....");
            //    Thread.Sleep(500);
            //}
            #endregion

            const ConsoleKey ScanFingerKey = ConsoleKey.S;
            const ConsoleKey ReadSystemParam = ConsoleKey.Q;
            const ConsoleKey UploadImageFingerKey = ConsoleKey.W;
            string MenuMessage = $"Menu options are as follows:\n" +
                $"{ScanFingerKey}: For scanning the finger to generate a image.\n" +
                $"{UploadImageFingerKey}: For uploading last scanned finger image.\n" +
                $"{ReadSystemParam}: For reading system parameters";

            const string ReturnMessage = "\n{0} Command returned:\n{1}Press any key to continue!";

            bool Exit = false;

            while(!Exit)
            {
                Console.Clear();
                Console.WriteLine(MenuMessage);
                ConsoleKey Key = Console.ReadKey().Key;
                switch(Key)
                {
                    case ScanFingerKey:
                        {
                            BasicCommandReturn Package = Tester.GenerateImage();
                            Console.WriteLine(ReturnMessage, "Generate Image", Package);
                            Console.ReadKey();
                        }
                        break;

                    case ReadSystemParam:
                        {
                            ReadSystemParamReturn Package = Tester.ReadSystemParam();
                            Console.WriteLine(ReturnMessage, "Read System Parameter", Package);
                            Console.ReadKey();
                        }
                        break;
                }

            }
            //Package = Tester.GenerateImage();
            //Package = Tester.UploadImage();
            //while(true)
            //{
            //    Console.Write(((byte)Tester.SerialPort.ReadByte()).ToString("X2"));
            //}
        }
    }
}
