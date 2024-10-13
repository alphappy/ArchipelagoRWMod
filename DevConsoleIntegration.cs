using DevConsole.Commands;

namespace alphappy.Archipelago
{
    /// <summary>
    /// Class containing all interaction with Dev Console.
    /// Methods which cannot run without Dev Console being enabled should be here.
    /// </summary>
    internal static class DevConsoleIntegration
    {
        internal static void RegisterCommands()
        {
            new CommandBuilder("apconnect").Run(Connect).AutoComplete(AutoComplete.Connect).Register();
            new CommandBuilder("apdisconnect").Run(Disconnect).Register();
            new CommandBuilder("apsay").Run(Say).Register();
            new CommandBuilder("apcollect").Run(Collect).Register();
        }

        internal static void Connect(string[] args)
        {
            switch (args.Length)
            {
                case < 2: Mod.LogToConsole("At least two arguments, host and name, must be specified."); break;
                case 2: ClientContainer.Connect(args[0], args[1]); break;
                default: ClientContainer.Connect(args[0], args[1], string.Join(" ", new ArraySegment<string>(args, 2, args.Length - 2))); break;
            }
        }

        internal static void Disconnect(string[] args) => ClientContainer.Disconnect();

        internal static void Say(string[] args)
        {
            if (args.Length == 0) return;
            ClientContainer.Say(string.Join(" ", args));
        }

        internal static void Collect(string[] args) => Messenger.JustCollectedThis(string.Join(" ", args));

        internal static class AutoComplete
        {
            internal static string[] Connect(string[] args)
            {
                if (args.Length == 0) { return new string[] { "archipelago.gg:", "localhost:", "localhost:38281" }; }
                return null;
            }
        }

    }
}
