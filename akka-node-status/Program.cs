using System;
using System.Threading;
using Akka.Actor;

namespace akka_node_status
{
    public static class Program
    {
        public static void Main()
        {
            var actorSystem = ActorSystem.Create("MachineStatuses");

            var consoleWriteProps = Props.Create(() => new ConsoleWriterActor());
            actorSystem.ActorOf(consoleWriteProps, Addresses.ConsoleWriter.Name);

            var coordinatorProps = Props.Create(() => new MachineStatusCoordinatorActor(new Pinger()));
            var coordinator = actorSystem.ActorOf(coordinatorProps, "Coordinator");

            var random = new Random();
            while (true)
            {
                if (random.Next(100) > 90)
                    coordinator.Tell(new MachineStatusCoordinatorActor.MachineFound("Machine " + random.Next(1, 10)));
                 else
                     coordinator.Tell(new MachineStatusCoordinatorActor.HeartBeat("Machine " + random.Next(1, 10)));

                Thread.Sleep(200);
            }
        }
    }
}