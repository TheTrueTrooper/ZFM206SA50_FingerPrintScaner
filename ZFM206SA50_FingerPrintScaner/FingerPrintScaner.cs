using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using ZFM206SA50_FingerPrintScaner.Enums;
using System.IO;
using System.Drawing;

namespace ZFM206SA50_FingerPrintScaner
{
    public class FingerPrintScaner : IDisposable
    {
        /// <summary>
        /// The default value that the device comes with for the baud rate
        /// </summary>
        const int DefaultBaudRate = 57600;

        /// <summary>
        /// The serial port that we will be using
        /// </summary>
        public SerialPort SerialPort;
        
        //start of message header with two bytes to mark the start
        /// <summary>
        /// the start of the devices messages
        /// </summary>
        readonly byte[] Header = new byte[] { 0xEF, 0x01 };

        //default is 0xFFFFFFFF
        /// <summary>
        /// The address of the device
        /// </summary>
        byte[] AddressBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        /// <summary>
        /// Gets if the device has been disposed
        /// </summary>
        bool IsDisposed = false;

        /// <summary>
        /// Gets the address of the device as a readable int (should assume 0s at start)
        /// </summary>
        public uint Address => BitConverterHelpers.GetU32(AddressBytes, 0);

        /// <summary>
        /// Gets the current chunk size
        /// </summary>
        public PackageLength DataPackageChunkSize { get; private set; }

        /// <summary>
        /// Gets the current baud rate as in the value in the enums name
        /// </summary>
        public BaudRates BaudRate = BaudRates.BaudRateOf57600;

        /// <summary>
        /// Creates a serial connection and sets up the class to begin a connection
        /// </summary>
        /// <param name="ComPort"></param>
        /// <param name="Address"></param>
        /// <param name="BaudRate"></param>
        /// <param name="DataPackageChunkSize"></param>
        public FingerPrintScaner(string ComPort, uint? Address = null, BaudRates BaudRate = BaudRates.BaudRateOf57600, PackageLength DataPackageChunkSize = PackageLength.LengthOf128, bool OpenNow = true)
        {
            if (!Enum.IsDefined(typeof(BaudRates), BaudRate))
                throw new Exception("BaudRate is not a valid baudrate!");
            if (!Enum.IsDefined(typeof(PackageLength), DataPackageChunkSize))
                throw new Exception("The chunk size is not a valid max chunk size for the device!");
            int BaudRateValue = DefaultBaudRate;
            switch(BaudRate)
            {
                case BaudRates.BaudRateOf9600:
                    BaudRateValue = 9600;
                    BaudRate = BaudRates.BaudRateOf9600;
                    break;
                case BaudRates.BaudRateOf19200:
                    BaudRateValue = 19200;
                    BaudRate = BaudRates.BaudRateOf19200;
                    break;
                case BaudRates.BaudRateOf28800:
                    BaudRateValue = 28800;
                    BaudRate = BaudRates.BaudRateOf28800;
                    break;
                case BaudRates.BaudRateOf38400:
                    BaudRateValue = 38400;
                    BaudRate = BaudRates.BaudRateOf38400;
                    break;
                case BaudRates.BaudRateOf48000:
                    BaudRateValue = 48000;
                    BaudRate = BaudRates.BaudRateOf48000;
                    break;
                case BaudRates.BaudRateOf57600:
                    BaudRateValue = 57600;
                    BaudRate = BaudRates.BaudRateOf57600;
                    break;
                case BaudRates.BaudRateOf67200:
                    BaudRateValue = 67200;
                    BaudRate = BaudRates.BaudRateOf67200;
                    break;
                case BaudRates.BaudRateOf76800:
                    BaudRateValue = 76800;
                    BaudRate = BaudRates.BaudRateOf76800;
                    break;
                case BaudRates.BaudRateOf86400:
                    BaudRateValue = 86400;
                    BaudRate = BaudRates.BaudRateOf86400;
                    break;
                case BaudRates.BaudRateOf96000:
                    BaudRateValue = 96000;
                    BaudRate = BaudRates.BaudRateOf96000;
                    break;
                case BaudRates.BaudRateOf105600:
                    BaudRateValue = 105600;
                    BaudRate = BaudRates.BaudRateOf105600;
                    break;
                case BaudRates.BaudRateOf115200:
                    BaudRateValue = 115200;
                    BaudRate = BaudRates.BaudRateOf115200;
                    break;
            }

            if (Address != null)
            {
                this.AddressBytes = BitConverterHelpers.GetBytes32(Address.Value);
            }

            this.DataPackageChunkSize = DataPackageChunkSize;
            SerialPort = new SerialPort(ComPort, BaudRateValue, Parity.None, 8, StopBits.One);

            if(OpenNow)
                SerialPort.Open();
        }

        /// <summary>
        /// opens connection if closed. 
        /// Note unless in constructor flag is set to false serial constructor will already have opened 
        /// </summary>
        public void OpenConnection()
        {
            SerialPort.Open();
        }

        /// <summary>
        /// Closes connection if open;
        /// </summary>
        public void CloseConnection()
        {
            SerialPort.Close();
        }

        #region BaseCommunicationMethods
        /// <summary>
        /// Sends a package
        /// </summary>
        /// <param name="Type">internal enum to mark package type</param>
        /// <param name="Data">The data to send in pakage</param>
        void SendPacket(PIDs Type, byte[] Data)
        {
            byte[] Packet = new byte[Data.Length + 11];

            byte[] Length = BitConverter.GetBytes((ushort)(Data.Length + 2));

            if (BitConverter.IsLittleEndian)
                Array.Reverse(Length);

            //mark packet start with header
            Array.Copy(Header, Packet, Header.Length);
            //Mark dest with address
            Array.Copy(AddressBytes, 0, Packet, 2, AddressBytes.Length);
            //Add identifier for pakage type
            Packet[6] = (byte)Type;
            //Set the length of the data.
            Array.Copy(Length, 0, Packet, 7, Length.Length);
            //load message 
            Array.Copy(Data, 0, Packet, 9, Data.Length);  

            byte[] CheckSum;
            {
                ushort CheckSumCalc = 0;
                int ReadTo = Packet.Length - 2;
                for (int i = 6; i < ReadTo; i++)
                    CheckSumCalc += Packet[i];
                CheckSum = BitConverter.GetBytes(CheckSumCalc);
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(CheckSum);

            Array.Copy(CheckSum, 0, Packet, Packet.Length - 2, CheckSum.Length);

            SerialPort.Write(Packet, 0, Packet.Length);
        }

        /// <summary>
        /// Gets a packages return
        /// </summary>
        /// <returns>The data package with the base decoded</returns>
        RecievePackage GetReturn()
        {
            byte[] Data = new byte[9];
            for (byte i = 0; i < 9; i++)
                Data[i] = (byte)SerialPort.ReadByte();
            if (!(Data[0] == 0xEF && Data[1] == 0x01))
                return null;
            uint Address;
            PIDs PID = (PIDs)Data[6];
            ushort Length;
            ushort CheckSum = 0;
            if (BitConverter.IsLittleEndian)
            {
                byte[] Number = new byte[4];
                Array.Copy(Data, 2, Number, 0, 4);
                Array.Reverse(Number);
                Address = BitConverter.ToUInt32(Number, 0);
            }
            else
                Address = BitConverter.ToUInt32(Data, 2);
            if (BitConverter.IsLittleEndian)
            {
                byte[] Number = new byte[2];
                Array.Copy(Data, 7, Number, 0, 2);
                Array.Reverse(Number);
                Length = BitConverter.ToUInt16(Number, 0);
            }
            else
                Length = BitConverter.ToUInt16(Data, 7);
            for (byte i = 6; i < 9; i++)
                CheckSum += Data[i];

            Data = new byte[Length];
            for (byte i = 0; i < Length; i++)
                Data[i] = (byte)SerialPort.ReadByte();

            return new RecievePackage(PID, Address, Data, CheckSum);
        }

        /// <summary>
        /// Gets any stream like packages
        /// </summary>
        /// <returns>Returns package with the stream of the item</returns>
        StreamReturn GetStream()
        {
            StreamReturn Return = GetReturn();
            RecievePackage Chunk;
            //int Pointer = 0;
            do
            {
                Chunk = GetReturn();
                Return.Stream.Write(Chunk.Data, 0, Chunk.Data.Length - 2);
                //Return.Stream.Position = 0;
                //Pointer += Chunk.Data.Length;
            } while (Chunk.PakageType == PIDs.DataPacket);
            return Return;
        }

        /// <summary>
        /// sends a package data like a stream to the device.
        /// Note uses classes DataPackageChunkSize for data chunk sizes in a single transm
        /// </summary>
        /// <param name="Data">The data to send</param>
        void SendDataStream(byte[] Data)
        {
            MemoryStream DataStream = new MemoryStream(Data);
            DataStream.Position = 0;
            int DataSize = 128;
            switch(this.DataPackageChunkSize)
            {
                case PackageLength.LengthOf32:
                    DataSize = 32;
                    break;
                case PackageLength.LengthOf64:
                    DataSize = 64;
                    break;
                case PackageLength.LengthOf128:
                    DataSize = 128;
                    break;
                case PackageLength.LengthOf256:
                    DataSize = 256;
                    break;
            }
            byte[] Chunk = new byte[DataSize];
            while (DataStream.Length - DataStream.Position >= DataSize)
            {
                DataStream.Read(Chunk, 0, Chunk.Length);
                SendPacket(PIDs.DataPacket, Chunk);
            }
            Chunk = new byte[DataStream.Length - DataStream.Position];
            DataStream.Read(Chunk, 0, Chunk.Length);
            SendPacket(PIDs.DataEndPacket, Chunk);
        }

        /// <summary>
        /// Sends a basic command to the device
        /// </summary>
        /// <param name="Command">The command op-code as an enum</param>
        /// <param name="Data">Any data that goes with the command</param>
        void SendCommand(CommandCodes Command, byte[] Data = null)
        {
            byte[] Packet;
            if (Data == null)
                Packet = new byte[1] { (byte)Command };
            else
            {
                Packet = new byte[1 + Data.Length];
                Packet[0] = (byte)Command;
                Array.Copy(Data, 0, Packet, 1, Data.Length);
            }
            SendPacket(PIDs.CommandPacket, Packet);
        }
        #endregion

        #region BasicSystemCommandsMethods
        /// <summary>
        /// The fist command that should be sent with the device to ready it for any other commands
        /// </summary>
        /// <returns></returns>
        public BasicCommandReturn InitHandShake()
        {
            SendCommand(CommandCodes.InitHandShake, new byte[] { 0x00 });
            return GetReturn();
        }

        /// <summary>
        /// Sets the address in the device then sets it in the class to match
        /// </summary>
        /// <param name="Address">the new address to use</param>
        /// <returns></returns>
        public BasicCommandReturn SetAddress(uint Address)
        {
            byte[] NewAddress = BitConverterHelpers.GetBytes32(Address);
            SendCommand(CommandCodes.SetAddress, NewAddress);
            this.AddressBytes = NewAddress;
            return GetReturn();
        }

        /// <summary>
        /// Sets a param 
        /// </summary>
        /// <param name="ParamNumber">the param to act on</param>
        /// <param name="Contents">the value to set it to</param>
        /// <returns></returns>
        BasicCommandReturn SetSystemParam(SystemParam ParamNumber, byte Contents)
        {
            SendCommand(CommandCodes.SetSystParam, new byte[] { (byte)ParamNumber, Contents });
            return GetReturn();
        }

        /// <summary>
        /// Sets the systems baud rate then sets 
        /// </summary>
        /// <param name="BaudRate"></param>
        /// <returns></returns>
        public BasicCommandReturn SetSystemBaudRate(BaudRates BaudRate)
        {
            if (!Enum.IsDefined(typeof(BaudRates), BaudRate))
                throw new Exception("BaudRate is not a valid baudrate!");
            this.BaudRate = BaudRate;
            BasicCommandReturn Return = SetSystemParam(SystemParam.BaudRate, (byte)BaudRate);
            if (Return.Status != Errors.Success || !Return.Valid)
                return Return;
            int BaudRateValue = DefaultBaudRate;
            switch (BaudRate)
            {
                case BaudRates.BaudRateOf9600:
                    BaudRateValue = 9600;
                    BaudRate = BaudRates.BaudRateOf9600;
                    break;
                case BaudRates.BaudRateOf19200:
                    BaudRateValue = 19200;
                    BaudRate = BaudRates.BaudRateOf19200;
                    break;
                case BaudRates.BaudRateOf28800:
                    BaudRateValue = 28800;
                    BaudRate = BaudRates.BaudRateOf28800;
                    break;
                case BaudRates.BaudRateOf38400:
                    BaudRateValue = 38400;
                    BaudRate = BaudRates.BaudRateOf38400;
                    break;
                case BaudRates.BaudRateOf48000:
                    BaudRateValue = 48000;
                    BaudRate = BaudRates.BaudRateOf48000;
                    break;
                case BaudRates.BaudRateOf57600:
                    BaudRateValue = 57600;
                    BaudRate = BaudRates.BaudRateOf57600;
                    break;
                case BaudRates.BaudRateOf67200:
                    BaudRateValue = 67200;
                    BaudRate = BaudRates.BaudRateOf67200;
                    break;
                case BaudRates.BaudRateOf76800:
                    BaudRateValue = 76800;
                    BaudRate = BaudRates.BaudRateOf76800;
                    break;
                case BaudRates.BaudRateOf86400:
                    BaudRateValue = 86400;
                    BaudRate = BaudRates.BaudRateOf86400;
                    break;
                case BaudRates.BaudRateOf96000:
                    BaudRateValue = 96000;
                    BaudRate = BaudRates.BaudRateOf96000;
                    break;
                case BaudRates.BaudRateOf105600:
                    BaudRateValue = 105600;
                    BaudRate = BaudRates.BaudRateOf105600;
                    break;
                case BaudRates.BaudRateOf115200:
                    BaudRateValue = 115200;
                    BaudRate = BaudRates.BaudRateOf115200;
                    break;
            }
            SerialPort.Close();
            SerialPort.BaudRate = BaudRateValue;
            SerialPort.Open();
            return Return;
        }

        public BasicCommandReturn SetSystemSecurityLevel(SecurityLevels SecurityLevel)
        {
            if (!Enum.IsDefined(typeof(SecurityLevels), SecurityLevel))
                throw new Exception("The Security Level is not a valid level for the device!");
            return SetSystemParam(SystemParam.SecurityLevel, (byte)SecurityLevel);
        }

        public BasicCommandReturn SetSystemPackageChunkLength(PackageLength DataPackageChunkSize)
        {
            if (!Enum.IsDefined(typeof(PackageLength), DataPackageChunkSize))
                throw new Exception("The chunk size is not a valid max chunk size for the device!");
            this.DataPackageChunkSize = DataPackageChunkSize;
            return SetSystemParam(SystemParam.DataPackageLength, (byte)DataPackageChunkSize);
        }

        public ReadSystemParamReturn ReadSystemParam()
        {
            SendCommand(CommandCodes.ReadSystParam);
            return GetReturn();
        }

        public ReadValidTemplateCountReturn ReadValidTemplateCount()
        {
            SendCommand(CommandCodes.ReadValidTemplateCount);
            return GetReturn();
        }
        #endregion

        #region ImageCommandsMethods
        public BasicCommandReturn GenerateImage()
        {
            SendCommand(CommandCodes.GenerateImage);
            return GetReturn();
        }

        public ImageReturn UploadImageToPC()
        {
            SendCommand(CommandCodes.UploadImage);
            return GetStream();
        }

        public BasicCommandReturn DownloadImageToSlave(Image Image)
        {
            if (Image.Height != StaticImageHelpers.ImageHieght && Image.Width != StaticImageHelpers.ImageWidth)
                throw new Exception("Image is not the correct size. Image bust be 8 bits (one byte) per pixel in a 256 by 288 pixel canvas for 73,728 pixels and is assumed to be grey scaled image.");

            byte[] ImageBytes = StaticImageHelpers.GetBytesFromImage(Image);
            return DownloadImageToSlave(ImageBytes);
        }

        public BasicCommandReturn DownloadImageToSlave(byte[] ImageBitmapAsBytes, bool AlreadyCompressed = false)
        {
            if(!AlreadyCompressed)
                ImageBitmapAsBytes = StaticImageHelpers.GetCompressedImageBytes(ImageBitmapAsBytes);

            SendCommand(CommandCodes.DownloadImage);
            BasicCommandReturn Return = GetReturn();
            if (Return.Status != Errors.Success)
                return Return;
            SendDataStream(ImageBitmapAsBytes);
            return Return;
        }
        #endregion

        #region ImageTemplateOrCharactorCommandMethods
        public BasicCommandReturn GenerateImageTemplate(ImageTemplateBuffers StoreAtBuffer = ImageTemplateBuffers.ImageTemplateBuffer2)
        {
            SendCommand(CommandCodes.GenerateImageTemplate, new byte[] { (byte)StoreAtBuffer });
            return GetReturn();
        }

        public BasicCommandReturn MergeOrRegenerateImageTemplate()
        {
            SendCommand(CommandCodes.RegenerateImageTemplate);
            return GetReturn();
        }

        public StreamReturn UploadImageTemplateToPC(ImageTemplateBuffers UploadFromStoreBuffer)
        {
            SendCommand(CommandCodes.UploadImageTemplate, new byte[] { (byte)UploadFromStoreBuffer });
            return GetStream();
        }

        public BasicCommandReturn DownLoadImageTemplateToSlave(ImageTemplateBuffers DownloadToStoreBuffer, byte[] ImageTemplateAsBytes)
        {
            SendCommand(CommandCodes.UploadImageTemplate, new byte[] { (byte)DownloadToStoreBuffer });
            BasicCommandReturn Return = GetReturn();
            if (Return.Status != Errors.Success)
                return Return;
            SendDataStream(ImageTemplateAsBytes);
            return Return;
        }

        public BasicCommandReturn DownLoadImageTemplateToSlave(ImageTemplateBuffers DownloadToStoreBuffer, Stream ImageTemplateAsStream)
        {
            MemoryStream ImageTemplateBitmapAsMemoryStream = new MemoryStream();
            ImageTemplateAsStream.Position = 0;
            ImageTemplateAsStream.CopyTo(ImageTemplateBitmapAsMemoryStream);
            return DownLoadImageTemplateToSlave(DownloadToStoreBuffer, ImageTemplateBitmapAsMemoryStream.ToArray());
        }

        public BasicCommandReturn StoreOrSaveImageTemplateOnSlaveFlash(ImageTemplateBuffers StoreBufferToSaveToFlash, ushort PageIndex)
        {
            byte[] Index = BitConverterHelpers.GetBytes16(PageIndex);
            SendCommand(CommandCodes.SaveOrStoreImageTemplate, new byte[] { (byte)StoreBufferToSaveToFlash, Index[0], Index[1] });
            return GetReturn();
        }

        public BasicCommandReturn LoadImageTemplateFromSlaveFlash(ImageTemplateBuffers StoreBufferToSaveToFlash, ushort PageIndex)
        {
            byte[] Index = BitConverterHelpers.GetBytes16(PageIndex);
            SendCommand(CommandCodes.LoadImageTemplate, new byte[] { (byte)StoreBufferToSaveToFlash, Index[0], Index[1] });
            return GetReturn();
        }

        public BasicCommandReturn DeleteImageTemplateFromSlaveFlash(ImageTemplateBuffers StoreBufferToSaveToFlash, ushort PageIndex, ushort NumberOfPagesToDelete)
        {
            byte[] Index = BitConverterHelpers.GetBytes16(PageIndex);
            byte[] Number = BitConverterHelpers.GetBytes16(NumberOfPagesToDelete);
            SendCommand(CommandCodes.DeleteImageTemplate, new byte[] { (byte)StoreBufferToSaveToFlash, Index[0], Index[1], Number[0], Number[1] });
            return GetReturn();
        }

        public BasicCommandReturn ClearOrEmptyImageTemplates()
        {
            SendCommand(CommandCodes.ClearOrEmptyImageTemplates);
            return GetReturn();
        }

        public MatchReturn MatchTemplates()
        {
            SendCommand(CommandCodes.TryMatchImageTemplate);
            return GetReturn();
        }

        public MatchReturn MatchTemplates(ImageTemplateBuffers StoreBufferToSaveToMatch, ushort StartAtPageIndex, ushort PageCount)
        {
            byte[] PageStartIndex = BitConverterHelpers.GetBytes16(StartAtPageIndex);
            byte[] PageCountNumber = BitConverterHelpers.GetBytes16(PageCount);
            SendCommand(CommandCodes.TryMatchAnyImageTemplate, new byte[] { (byte)StoreBufferToSaveToMatch, PageStartIndex[0], PageStartIndex[1], PageCountNumber[0], PageCountNumber[1] });
            return GetReturn();
        }
        #endregion

        #region OtherMiscCommands
        public RandomNumberReturn GenerateRandomNumber()
        {
            SendCommand(CommandCodes.GenerateRandomNumber);
            return GetReturn();
        }

        const byte PageSize = 32;
        const byte NumberOfPages = 16;

        public BasicCommandReturn RawWriteToNotePadOnSlave(byte NotePadPageIndex, byte[] Data)
        {
            if (Data == null)
                throw new Exception("Data Passed is null. Did you want to write to the flash?");
            if (NotePadPageIndex >= NumberOfPages)
                throw new Exception($"Page number is greater than {NumberOfPages}. There are only {NumberOfPages} of memory avalible for for the notepad.");
            if(Data.Length == PageSize)
                throw new Exception($"Data Passed is not a full page. Writes of a raw nature must be a full {PageSize} byte page block size due to hardware restrictions.");
            SendCommand(CommandCodes.WriteNotePad, new byte[] { NotePadPageIndex,
                Data[0], Data[1], Data[2], Data[3], Data[4], Data[5], Data[6], Data[7],
                Data[8], Data[9], Data[10], Data[11], Data[12], Data[13], Data[14], Data[15],
                Data[16], Data[17], Data[18], Data[19], Data[20], Data[21], Data[22], Data[23],
                Data[24], Data[25], Data[26], Data[27], Data[28], Data[29], Data[30], Data[31] });
            return GetReturn();
        }

        public BasicCommandReturn WriteToNotePadOnSlave(byte NotePadPageIndex, byte[] Data)
        {
            if (Data == null)
                throw new Exception("Data Passed is null. Did you want to write to the flash?");
            if (Data.Length > PageSize)
                throw new Exception($"Data Passed is too big. To write to a single page block the page must be less than {PageSize} bytes. Try breaking up your writes; however, note that there are only {NumberOfPages} pages or blocks.");
            byte[] WriteBuffer = new byte[32] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Array.Copy(Data, WriteBuffer, Data.Length);
            return RawWriteToNotePadOnSlave(NotePadPageIndex, WriteBuffer);
        }

        public BasicCommandReturn WriteToNotePadOnSlave(byte NotePadPageIndex, string String, Encoding Encoding = null)
        {
            if (Encoding == null)
                Encoding = Encoding.ASCII;
            byte[] Data = Encoding.GetBytes(String);
            if (Data == null)
                throw new Exception("Data passed is null. Did you want to write to the flash?");
            if (Data.Length > PageSize)
                throw new Exception($"Data of passed string is too big. To write to a single page block the page must be less than {PageSize} bytes. Try breaking up your writes or using a different ecoding; however, note that there are only {NumberOfPages} pages or blocks. Also note that the default encoding is ascii and is the smallest size.");
            return WriteToNotePadOnSlave(NotePadPageIndex, Data);
        }

        public BasicCommandReturn WriteEntireNotePadOnSlave(string String, Encoding Encoding = null)
        {
            const short MaxSize = 512;
            byte[] Data = Encoding.GetBytes(String);
            if (Data.Length > MaxSize)
                throw new Exception($"Data of passed string is too big. To write to the entire note the page must be less than {MaxSize} bytes. Try using a different ecoding or shorten your data. Also note that the default encoding is ascii and is the smallest size.");
            MemoryStream DataStream = new MemoryStream(Data);
            BasicCommandReturn Return;
            byte Page = 0;
            byte[] Chunk = new byte[PageSize];
            while (DataStream.Length - DataStream.Position >= PageSize)
            {
                DataStream.Read(Chunk, 0, Chunk.Length);
                Return = WriteToNotePadOnSlave(Page, Chunk);
                Page++;
                if (Return.Status != Errors.Success || !Return.Valid)
                    return Return;
            }
            Chunk = new byte[DataStream.Length - DataStream.Position];
            DataStream.Read(Chunk, 0, Chunk.Length);
            Return = WriteToNotePadOnSlave(Page, Chunk);
            return Return;
        }

        public ReadNotePadReturn ReadFromNotePadOnSlave(byte NotePadPageIndex)
        {
            if (NotePadPageIndex >= NumberOfPages)
                throw new Exception($"Page number is greater than {NumberOfPages}. There are only {NumberOfPages} of memory avalible for for the notepad.");
            SendCommand(CommandCodes.ReadNotePad, new byte[] { NotePadPageIndex });
            return GetReturn();
        }

        public ReadNotePadReturn ReadEntireNotePadOnSlave()
        {
            ReadNotePadReturn Return = ReadFromNotePadOnSlave(0);
            if (Return.Status != Errors.Success || !Return.Valid)
                return Return;
            byte[] CurrentString = Return.RawBytes;
            for (byte Page = 1; Page < NumberOfPages; Page++)
            {
                Return = ReadFromNotePadOnSlave(Page);
                if (Return.Status != Errors.Success || !Return.Valid)
                {
                    Return.RawBytes = CurrentString;
                    return Return;
                }
                int Pointer = CurrentString.Length;
                Array.Resize(ref CurrentString, CurrentString.Length + Return.RawBytes.Length);
                Array.Copy(Return.RawBytes, 0, CurrentString, Pointer, Return.RawBytes.Length);
            }
            Return.RawBytes = CurrentString;
            return Return;
        }

        #endregion

        #region CleanCodeMethods
        public void Dispose()
        {
            if(!IsDisposed)
                SerialPort.Dispose();
        }

        ~FingerPrintScaner()
        {
            if (!IsDisposed)
                SerialPort.Dispose();
        }
        #endregion
    }
}
