using LeaveSample.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LeaveSample.Services.Contracts
{
    [Serializable]
    [DataContract]
    public class ListLeavesResponse
    {
        [DataMember]
        public List<Leave> Leaves { get; set; }

        [DataMember]
        public int TotalRowCount { get; set; }
    }
}
