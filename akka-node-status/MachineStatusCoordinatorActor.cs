using System.Collections.Generic;
using Akka.Actor;

namespace akka_node_status
{
    public class MachineStatusCoordinatorActor : ReceiveActor
    {
        public class HeartBeat
        {
            public HeartBeat(string machineName)
            {
                MachineName = machineName;
            }

            public string MachineName { get; private set; }
        }

        public class MachineFound
        {
            public MachineFound(string machineName)
            {
                MachineName = machineName;
            }

            public string MachineName { get; private set; }
        }

        private readonly Dictionary<string, IActorRef> _machineActors = new Dictionary<string, IActorRef>();
        private readonly IPinger _pinger;

        public MachineStatusCoordinatorActor(IPinger pinger)
        {
            _pinger = pinger;
            Receive<MachineFound>(m => HandleMachineFound(m));
            Receive<HeartBeat>(hb => HandleHeartBeat(hb));
        }

        private void HandleMachineFound(MachineFound machineFound)
        {
            if (!_machineActors.ContainsKey(machineFound.MachineName))
            {
                var machineProp = Props.Create(() => new MachineActor(_pinger, machineFound.MachineName));
                _machineActors[machineFound.MachineName] = Context.ActorOf(machineProp);

                Context.System.ActorSelection(Addresses.ConsoleWriter.Path).Tell(new MachineStatus(machineFound.MachineName, null, null, null));
            }
        }

        private void HandleHeartBeat(HeartBeat hb)
        {
            if (!_machineActors.ContainsKey(hb.MachineName))
                return;

            _machineActors[hb.MachineName].Tell(new MachineActor.HeartBeat());
        }
    }
}