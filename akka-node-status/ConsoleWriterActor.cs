using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace akka_node_status
{
    public class ConsoleWriterActor : ReceiveActor
    {
        public class WriteToConsole { }

        private readonly Dictionary<string, MachineStatus> _machineStatuses;

        public ConsoleWriterActor()
        {
            _machineStatuses = new Dictionary<string, MachineStatus>();
            Receive<MachineStatus>(s => UpdateMachineStatus(s));
            Receive<WriteToConsole>(c => Render());
        }

        private void Render()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("{0} Machine(s)", _machineStatuses.Count);
            Console.WriteLine();

            foreach (var status in _machineStatuses.OrderBy(s => s.Key))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(status.Key);

                Console.ForegroundColor = status.Value.IsActive ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
                Console.Write(status.Value.IsActive ? "Active" : "Gone");

                Console.ResetColor();
                if (status.Value.LastHeard != null)
                    Console.Write(" {0}s ago", Math.Floor(Context.System.Scheduler.Now.Subtract(status.Value.LastHeard.Value).TotalSeconds));

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (!status.Value.IsActive)
                    Console.Write(" Ping: {0}", status.Value.PingStatus.HasValue ? status.Value.PingStatus.Value.ToString() : "Waiting...");

                Console.WriteLine();
            }
            Console.ResetColor();
        }

        private void UpdateMachineStatus(MachineStatus status)
        {
            _machineStatuses[status.MachineName] = status;
            Self.Tell(new WriteToConsole());
        }
    }
}