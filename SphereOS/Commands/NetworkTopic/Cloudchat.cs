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
            new Apps.CloudChat().Init();

            return ReturnCode.Success;
        }
    }
}
