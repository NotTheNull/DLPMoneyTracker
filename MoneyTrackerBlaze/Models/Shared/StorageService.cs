using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlaze.Models.Shared
{
    public class StorageService<T>
    {
        public T Data { get; set; }
        public string ReturnURL { get; set; }
        
    }
}
