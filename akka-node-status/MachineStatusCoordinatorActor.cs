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

        public class Reset
        {
            public Reset(string machineName)
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
            Receive<Reset>(m => HandleReset(m));
            Receive<HeartBeat>(hb => HandleHeartBeat(hb));
        }

        private void HandleReset(Reset reset)
        {
            if (!_machineActors.ContainsKey(reset.MachineName))
            {
                var machineProp = Props.Create(() => new MachineActor(_pinger, reset.MachineName));
                _machineActors[reset.MachineName] = Context.ActorOf(machineProp);
            }
            _machineActors[reset.MachineName].Tell(new MachineActor.Reset());
        }

        private void HandleHeartBeat(HeartBeat hb)
        {
            if (!_machineActors.ContainsKey(hb.MachineName))
                return;

            _machineActors[hb.MachineName].Tell(new MachineActor.HeartBeat());
        }
    }
}