using System;
using System.Collections.Generic;
using System.Text;

namespace STL.DAL.Licensing
{
    public class RegistrationResult
    {
        public RegistrationResult() { }
        public RegistrationResult(Machine machine, string statusMsg, int currentlyRegisteredMachines, Interval registerInterval)
        {
            this.Machine = machine;
            this.StatusMsg = statusMsg;
            this.CurrentlyRegisteredMachines = currentlyRegisteredMachines;
            this.RegisterInterval = registerInterval;
        }
        public RegistrationResult(Machine machine, string statusMsg, Interval registerInterval)
        {
            this.Machine = machine;
            this.StatusMsg = statusMsg;
            this.RegisterInterval = registerInterval;
        }
        public RegistrationResult(Machine machine, string statusMsg)
        {
            this.Machine = machine;
            this.StatusMsg = statusMsg;
        }

        public Machine Machine
        { get; set; }
        public bool Registered
        {
            get
            {
                return Machine.Registered;
            }
        }
        public string StatusMsg
        { get; set; }
        public int CurrentlyRegisteredMachines
        { get; set; }
        public Interval RegisterInterval 
        { get; set; }
    }
}
