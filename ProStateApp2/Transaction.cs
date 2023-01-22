using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProStateApp2
{
    class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; internal set; }
        public string Description { get; internal set; }
        public DateTime Date { get; internal set; }
        public string AccountFrom { get; internal set; }
        public string AccountTo { get; internal set; }
        public string FromName { get; internal set; }
        public string ToName { get; internal set; }
    }
}
