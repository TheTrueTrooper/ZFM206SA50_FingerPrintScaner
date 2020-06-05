using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner
{
    public enum Errors : byte
    {
        Success = 0x00,
        ErrorOnDataTransfer = 0x01,
        NoFingerOnSensor = 0x02,
        FailedToEnrollFinger = 0x03,
        FailTOGenerateFingerCharDueToImageDisorder = 0x06,
        FailTOGenerateFingerCharDueToLackOfImageResolution = 0x07,
        FingerPrintDoesntMatch = 0x08,
        FailedToFindMachingFinger = 0x09,
        FailedToCombineFingerChar = 0x0A,
        OutOfIndexAddressingError = 0x0B,
        ErrorReadingTemplateFromLib = 0x0C,
        ErrorUploadingTemplateToLib = 0x0D,
        ModualBusyOnFileTransfer = 0x0E,
        ErrorUploadingImage = 0x0F,
        FailedToDeleteTemplate = 0x10,
        FailedToClearFingerLib = 0x11,
        FailedToGenerateImageDueToInvalidPrimaryImage = 0x15,
        ErrorWritingFlash = 0x18,
        NoDefinintionError = 0x19,
        InvalidRegisterNumber = 0x1A,
        IncorrectConfigurationOfRegister = 0x1B,
        WrongNotepadPageNumber = 0x1C,
        FailToOperateCommunicationPort = 0x1D
    }
}
