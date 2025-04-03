using System;
using System.Collections.Generic;
using System.Web;

namespace STL.DAL.Licensing
{
    public class Machine
    {
        public string MachineId
        { get; set; }
        public bool Registered
        { get; set; }
        public int UserId
        { get; set; }
        public string CustomerApplicationId
        { get; set; }
        public string MachineName
        { get; set; }
    }
}