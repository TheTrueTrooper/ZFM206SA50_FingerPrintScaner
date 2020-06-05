using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner.Enums
{
    enum CommandCodes : byte
    {
        InitHandShake = 0x17,
        SetAddress = 0x15,
        SetSystParam = 0x0E,
        ReadSystParam = 0x0F,
        ReadValidTemplateNumber = 0x1D,
        GenerateImage = 0x01,
        UploadImage = 0x0A,
        DownloadImage = 0x0B,
        GenerateImageTemplate = 0x02,
        RegenerateImageTemplate = 0x05,
        UploadImageTemplate = 0x08,
        DownloadImageTemplate = 0x09,
        SaveOrStoreImageTemplate = 0x06,
        LoadImageTemplate = 0x07,
        DeleteImageTemplate = 0x0C,
        ClearOrEmptyImageTemplate = 0x0D,
        TryMatchImageTemplate = 0x03,
        TryMatchAnyImageTemplate = 0x04,
        GenerateRandomNumber = 0x14,
        WriteNotePad = 0x18,
        ReadNotePad = 0x19
    }
}
