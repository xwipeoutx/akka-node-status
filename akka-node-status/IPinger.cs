using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace akka_node_status
{
    public interface IPinger
    {
        Task<IPStatus> Ping(string machineName);
    }
}