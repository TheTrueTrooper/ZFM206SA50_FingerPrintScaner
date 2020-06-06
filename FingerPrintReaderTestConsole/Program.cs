using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZFM206SA50_FingerPrintScaner;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
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

            FingerPrintScaner Tester = new FingerPrintScaner(Settings.ComPort);
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

            string ImageString = Settings.TestImage;
            byte[] TestImage = new byte[ImageString.Length / 2];
            for (int i = 0; i < ImageString.Length/2; i++)
            {
                TestImage[i] = Convert.ToByte(ImageString.Substring(i * 2, 2), 16);
            }

            const ConsoleKey ScanFingerKey = ConsoleKey.S;
            const ConsoleKey ReadSystemParamKey = ConsoleKey.Q;
            const ConsoleKey UploadImageFingerKey = ConsoleKey.W;
            const ConsoleKey SetAddressKey = ConsoleKey.T;
            const ConsoleKey ReadValidTemplateCountKey = ConsoleKey.A;
            const ConsoleKey ClearOrEmptyImageTemplatesKey = ConsoleKey.G;
            const ConsoleKey InitKey = ConsoleKey.R;
            string MenuMessage = $"Menu options are as follows:\n" +
                $"{ScanFingerKey}: For scanning the finger to generate a image.\n" +
                $"{UploadImageFingerKey}: For uploading last scanned finger image.\n" +
                $"{ReadSystemParamKey}: For reading system parameters.\n" +
                $"{ReadValidTemplateCountKey}: For getting count of valid templates.\n" +
                $"{ClearOrEmptyImageTemplatesKey}: For clearing out or deleting all of the Image Templates.\n" +
                $"{InitKey}: For repeating back through the init.\n" +
                $"{SetAddressKey}: For to Set a new address";

            const string ReturnMessage = "\n{0} Command returned:\n{1}Press any key to continue!";

            bool Exit = false;

            while (!Exit)
            {
                Console.Clear();
                Console.WriteLine(MenuMessage);
                ConsoleKey Key = Console.ReadKey().Key;
                switch(Key)
                {
                    case InitKey:
                        {
                            BasicCommandReturn Package = Tester.InitHandShake();
                            Console.WriteLine(ReturnMessage, "Init Hand Shake", Package);
                            Console.ReadKey();
                        }
                        break;
                    case ScanFingerKey:
                        {
                            BasicCommandReturn Package = Tester.GenerateImage();
                            Console.WriteLine(ReturnMessage, "Generate Image", Package);
                            Console.ReadKey();
                        }
                        break;
                    case ClearOrEmptyImageTemplatesKey:
                        {
                            BasicCommandReturn Package = Tester.ClearOrEmptyImageTemplates();
                            Console.WriteLine(ReturnMessage, "Clear Or Empty Image Templates", Package);
                            Console.ReadKey();
                        }
                        break;
                    case ReadSystemParamKey:
                        {
                            ReadSystemParamReturn Package = Tester.ReadSystemParam();
                            Console.WriteLine(ReturnMessage, "Read System Parameter", Package);
                            Console.ReadKey();
                        }
                        break;
                    case UploadImageFingerKey:
                        {
                            ImageReturn Package = Tester.UploadImage();
                            Console.WriteLine(ReturnMessage, "Upload Image Finger", Package);
                            Application.Run(new Form1(Package.Image));
                            Console.ReadKey();
                        }
                        break;
                    case ReadValidTemplateCountKey:
                        {
                            ReadValidTemplateCountReturn Package = Tester.ReadValidTemplateCount();
                            Console.WriteLine(ReturnMessage, "Read Valid Template Count", Package);
                            Console.ReadKey();
                        }
                        break;
                    //case ReadValidTemplateCountKey:
                    //    {
                    //        ReadValidTemplateCountReturn Package = Tester.DownloadImage("");
                    //        Console.WriteLine(ReturnMessage, "Read Valid Template Count", Package);
                    //        Console.ReadKey();
                    //    }
                    //    break;
                    case SetAddressKey:
                        {
                            Console.WriteLine("\nPlease enter a new address via 4 hex digits.");
                            bool Succes = false;
                            uint NewAddress = 0;
                            do
                            {
                                string HexNumString = "";
                                HexNumString += Console.ReadKey().KeyChar;
                                HexNumString += Console.ReadKey().KeyChar;
                                HexNumString += Console.ReadKey().KeyChar;
                                HexNumString += Console.ReadKey().KeyChar;
                                HexNumString += Console.ReadKey().KeyChar;
                                HexNumString += Console.ReadKey().KeyChar;
                                HexNumString += Console.ReadKey().KeyChar;
                                HexNumString += Console.ReadKey().KeyChar;
                                try
                                {
                                    NewAddress = uint.Parse(HexNumString, System.Globalization.NumberStyles.HexNumber);
                                    Succes = true;
                                }
                                catch
                                {
                                    Console.WriteLine("\nOpps! That is not a valid hex number with 1-F in the digits.");
                                    Succes = false;
                                }
                            } while (!Succes);
                            BasicCommandReturn Package = Tester.SetAddress(NewAddress);
                            Console.WriteLine(ReturnMessage, "Set Address", Package);
                            Console.ReadKey();
                        }
                        break;
                }

            }
            //Package = Tester.GenerateImage();
            //Package = Tester.UploadImage();
            
        }
    }
}
