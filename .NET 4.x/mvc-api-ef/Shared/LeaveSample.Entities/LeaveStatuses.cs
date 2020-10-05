using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveSample.Entities
{
    [Serializable]
    public enum LeaveStatuses : byte
    {
        Pending,
        Cancelled,
        Approved,
        Rejected
    }
}
