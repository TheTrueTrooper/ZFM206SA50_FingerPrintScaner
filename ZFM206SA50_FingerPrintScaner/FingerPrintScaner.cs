using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using ZFM206SA50_FingerPrintScaner.Enums;
using System.IO;

namespace ZFM206SA50_FingerPrintScaner
{
    public class FingerPrintScaner
    {
        const int DefaultBaudRate = 57600;

        public SerialPort SerialPort;
        
        //start of message header with two bytes to mark the start
        readonly byte[] Header = new byte[] { 0xEF, 0x01 };

        //default is 0xFFFFFFFF
        byte[] Address = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };


        public FingerPrintScaner(string ComPort, uint? Address = null, int BaudRate = DefaultBaudRate)
        {
            if (Address != null)
            {
                this.Address = BitConverter.GetBytes(Address.Value);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(this.Address);
            }
            SerialPort = new SerialPort(ComPort, BaudRate, Parity.None, 8, StopBits.One);

            SerialPort.Open();
        } 

        void SendPacket(PIDs Type, byte[] Data)
        {
            byte[] Packet = new byte[Data.Length + 11];

            byte[] Length = BitConverter.GetBytes((ushort)(Data.Length + 2));

            if (BitConverter.IsLittleEndian)
                Array.Reverse(Length);

            //mark packet start with header
            Array.Copy(Header, Packet, Header.Length);
            //Mark dest with address
            Array.Copy(Address, 0, Packet, 2, Address.Length);
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

        void SendDataStream(byte[] Data, PackageLength Length = PackageLength.LengthOf128)
        {
            if (!Enum.IsDefined(typeof(PackageLength), Length))
                throw new Exception("Lenthis not a valid length");
            MemoryStream DataStream = new MemoryStream(Data);
            DataStream.Position = 0;
            int DataSize = 128;
            switch(Length)
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
            while (DataStream.Length - DataStream.Position < DataSize)
            {
                DataStream.Read(Chunk, 0, Chunk.Length);
                SendPacket(PIDs.DataPacket, Chunk);
            }
            Chunk = new byte[DataStream.Length - DataStream.Position];
            DataStream.Read(Chunk, 0, Chunk.Length);
            SendPacket(PIDs.DataEndPacket, Chunk);
        }

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

        public BasicCommandReturn InitHandShake()
        {
            SendCommand(CommandCodes.InitHandShake, new byte[] { 0x00 });
            return GetReturn();
        }

        public BasicCommandReturn SetAddress(uint Address)
        {
            byte[] NewAddress = BitConverter.GetBytes(Address);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(NewAddress);
            SendCommand(CommandCodes.SetAddress, NewAddress);
            this.Address = NewAddress;
            return GetReturn();
        }

        BasicCommandReturn SetSystemParam(SystemParam ParamNumber, byte Contents)
        {
            SendCommand(CommandCodes.SetSystParam, new byte[] { (byte)ParamNumber, Contents });
            return GetReturn();
        }

        public BasicCommandReturn SetSystemBaudRate(BaudRates Rate)
        {
            return SetSystemParam(SystemParam.BaudRate, (byte)Rate);
        }

        public BasicCommandReturn SetSystemSecurityLevel(SecurityLevels SecurityLevel)
        {
            return SetSystemParam(SystemParam.SecurityLevel, (byte)SecurityLevel);
        }

        public BasicCommandReturn SetSystemPackageLengthg(PackageLength Length)
        {
            return SetSystemParam(SystemParam.DataPackageLength, (byte)Length);
        }

        public ReadSystemParamReturn ReadSystemParam()
        {
            SendCommand(CommandCodes.ReadSystParam, new byte[] { });
            return GetReturn();
        }

        public ReadValidTemplateCountReturn ReadValidTemplateCount()
        {
            SendCommand(CommandCodes.ReadValidTemplateCount, new byte[] { });
            return GetReturn();
        }

        public BasicCommandReturn GenerateImage()
        {
            SendCommand(CommandCodes.GenerateImage, new byte[] { });
            return GetReturn();
        }

        public ImageReturn UploadImage()
        {
            SendCommand(CommandCodes.UploadImage, new byte[] { });
            return GetStream();
        }

        public BasicCommandReturn DownloadImage(byte[] ImageAsBytes, PackageLength Length = PackageLength.LengthOf128)
        {
            SendCommand(CommandCodes.DownloadImage, new byte[] { });
            BasicCommandReturn Return = GetReturn();
            if(Return.Status != Errors.Success)
                return Return;
            SendDataStream(ImageAsBytes, Length);
            return Return;
        }

        public BasicCommandReturn ClearOrEmptyImageTemplates()
        {
            SendCommand(CommandCodes.ClearOrEmptyImageTemplates, new byte[] { });
            return GetReturn();
        }

        

    }
}
