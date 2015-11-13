
using System;
using System.Net.NetworkInformation;

namespace akka_node_status
{
    public class MachineStatus
    {
        public MachineStatus(string machineName, bool isActive, DateTimeOffset? lastHeard, IPStatus? pingStatus)
        {
            IsActive = isActive;
            LastHeard = lastHeard;
            PingStatus = pingStatus;
            MachineName = machineName;
        }

        public string MachineName { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset? LastHeard { get; private set; }
        public IPStatus? PingStatus { get; private set; }
    }
}
