using LeaveSample.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveSample.UI.Web.SignalR
{
    public interface ILeaveActionClient
    {
        Task LeaveApplied(Leave leave);
        Task LeaveCancelled(Leave leave);
        Task LeaveUpdated(Leave leave);
    }
}
