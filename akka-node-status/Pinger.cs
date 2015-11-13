using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace akka_node_status
{
    public class Pinger : IPinger
    {
        public async Task<IPStatus> Ping(string machineName)
        {
            await Task.Delay(new Random().Next(500));
            return (IPStatus)new Random().Next(10002, 10010);
        }
    }
}