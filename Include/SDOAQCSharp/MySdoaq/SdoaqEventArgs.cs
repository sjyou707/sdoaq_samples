using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQCSharp
{
    public sealed class SdoaqEventArgs : EventArgs
    {
        public SDOAQ.SDOAQ_API.eErrorCode ErrorCode { get; }
        public string ErrorMessage { get; }
        public SdoaqEventArgs(SDOAQ.SDOAQ_API.eErrorCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
