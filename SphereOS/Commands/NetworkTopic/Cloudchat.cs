namespace SphereOS.Commands.NetworkTopic
{
    internal class Cloudchat : Command
    {
        public Cloudchat() : base("cloudchat")
        {
            Description = "Start CloudChat!";

            Topic = "network";
        }

        internal override ReturnCode Execute(string[] args)
        {
            new ConsoleApps.CloudChat().Init();

            return ReturnCode.Success;
        }
    }
}
