using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Entity
{
    public class JobOnHandData
    {
        public string Username { get; set; }
        public int AmountOwnerJob { get; set; }
        public int AmountDelegateJob { get; set; }
    }
}
