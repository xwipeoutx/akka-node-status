using System;
using System.Net.NetworkInformation;
using Akka.Actor;

namespace akka_node_status
{
    public class MachineActor : ReceiveActor
    {
        public class HeartBeat { }
        private class Flatlined { }
        private class PingRequest { }

        private readonly IPinger _pinger;
        private readonly string _machineName;
        private ICancelable _cancelable;
        private DateTimeOffset _lastHeard;

        public MachineActor(IPinger pinger, string machineName)
        {
            _pinger = pinger;
            _machineName = machineName;
            Become(Alive);
        }

        private void Alive()
        {
            Receive<HeartBeat>(hb => HandleHeartBeat());
            Receive<Flatlined>(d => HandleFlatline());
        }

        private void HandleHeartBeat()
        {
            _lastHeard = Context.System.Scheduler.Now;
            Context.System.ActorSelection(Addresses.ConsoleWriter.Path).Tell(new MachineStatus(_machineName, true, _lastHeard, null));

            if (_cancelable != null)
                _cancelable.Cancel();

            _cancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(TimeSpan.FromSeconds(2), Self, new Flatlined(), Self);
        }

        private void HandleFlatline()
        {
            Context.System.ActorSelection(Addresses.ConsoleWriter.Path).Tell(new MachineStatus(_machineName, false, _lastHeard, null));
            Become(Dead);
            Self.Tell(new PingRequest());
        }

        private void Dead()
        {
            Receive<HeartBeat>(hb =>
            {
                HandleHeartBeat();
                Become(Alive);
            });

            Receive<PingRequest>(r => DoPing());
            Receive<IPStatus>(r => HandlePingResponse(r));
        }

        private void HandlePingResponse(IPStatus ipStatus)
        {
            Context.System.ActorSelection(Addresses.ConsoleWriter.Path).Tell(new MachineStatus(_machineName, false, _lastHeard, ipStatus));
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(1), Self, new PingRequest(), Self);
        }

        private void DoPing()
        {
            var self = Self;
            _pinger.Ping(_machineName).ContinueWith(status =>
            {
                if (status.IsCanceled || status.IsFaulted)
                    return;

                self.Tell(status.Result);
            });
        }
    }
}