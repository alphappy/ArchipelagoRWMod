using DevConsole;
using DevConsole.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago
{
    internal static class DevConsoleIntegration
    {
        internal static void RegisterCommands()
        {
            new CommandBuilder("apconnect").Run(Connect).AutoComplete(AutoComplete.Connect).Register();
            new CommandBuilder("apdisconnect").Run(Disconnect).Register();
            new CommandBuilder("apsay").Run(Say).Register();
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
