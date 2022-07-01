using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo.Utilities
{
    public enum VidoeTransferResult
    {
        Success=0,
        InProgress=1,
        Skipped=2,
        FailedFirstDataRecording = 10,
        FailedNoLoanId =11,
        FailedNoBytesVideo=12,
        FailedUpladFailed=13,
        FailedFinalDataRecording=14
    }
}
