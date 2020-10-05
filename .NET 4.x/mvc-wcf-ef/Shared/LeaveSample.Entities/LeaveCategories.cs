using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveSample.Entities
{
    [Serializable]
    public enum LeaveCategories : byte
    {
        Annual,
        Medical, 
        Study
    }
}
