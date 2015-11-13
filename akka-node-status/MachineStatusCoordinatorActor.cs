using System.Collections.Generic;
using Akka.Actor;

namespace akka_node_status
{
    public class MachineStatusCoordinatorActor : TypedActor, IHandle<MachineStatusCoordinatorActor.HeartBeat>, IHandle<MachineStatusCoordinatorActor.Reset>
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
        }

        public void Handle(Reset message)
        {
            if (!_machineActors.ContainsKey(message.MachineName))
            {
                var machineProp = Props.Create(() => new MachineActor(_pinger, message.MachineName));
                _machineActors[message.MachineName] = Context.ActorOf(machineProp);
            }
            _machineActors[message.MachineName].Tell(new MachineActor.Reset());
        }

        public void Handle(HeartBeat message)
        {
            if (!_machineActors.ContainsKey(message.MachineName))
                return;

            _machineActors[message.MachineName].Tell(new MachineActor.HeartBeat());
        }
    }
}