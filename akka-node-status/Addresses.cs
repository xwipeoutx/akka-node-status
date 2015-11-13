namespace akka_node_status
{
    public static class Addresses
    {
        public static readonly ActorMetaData ConsoleWriter = new ActorMetaData("consoleWriter", "akka://MachineStatuses/user/consoleWriter");

        public class ActorMetaData
        {
            public ActorMetaData(string name, string path)
            {
                Name = name;
                Path = path;
            }

            public string Name { get; private set; }

            public string Path { get; private set; }
        }
    }
}