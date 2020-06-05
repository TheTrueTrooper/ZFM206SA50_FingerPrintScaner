using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFM206SA50_FingerPrintScaner.Enums
{
    /// <summary>
    /// is 1, 2, 3, 4, 5.  
    /// Noting FAR(False Acceptance Rate) and FRR(False Rejection Rate) 
    /// At level 1, FAR is the highest and FRR bis the lowest; however at level 5, FAR is the lowest and FRR is the highest
    /// </summary>
    public enum SecurityLevels : byte
    {
        /// <summary>
        /// Lowest security
        /// </summary>
        SecurityLevel1 = 0x01,
        /// <summary>
        /// low security
        /// </summary>
        SecurityLevel2 = 0x02,
        /// <summary>
        /// Mid security
        /// </summary>
        SecurityLevel3 = 0x03,
        /// <summary>
        /// high security
        /// </summary>
        SecurityLevel4 = 0x04,
        /// <summary>
        /// highest security
        /// </summary>
        SecurityLevel5 = 0x05
    }
}
