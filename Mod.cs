using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archipelago;
using System.Reflection;
using DevConsole.Commands;
using DevConsole;

namespace alphappy.Archipelago
{
    [BepInPlugin(Const.PLUGIN_GUID, Const.PLUGIN_NAME, Const.PLUGIN_VERSION)]
    [BepInDependency("slime-cubed.devconsole", BepInDependency.DependencyFlags.HardDependency)]
    public class Mod : BaseUnityPlugin
    {
        private static bool startedInitialization = false;
        private static bool initializedWithoutException = false;
        public static BepInEx.Logging.ManualLogSource log;

        public static void LogToConsole(string msg) => GameConsole.WriteLine(msg);
        public static void Log(object msg)
        {
            LogToConsole(msg.ToString());
            log.LogDebug(msg);
        }

        public static void Log(Exception exception)
        {
            LogToConsole(exception.ToString());
            log.LogError(exception);
        }

        public Mod() => log = Logger;

        internal static BindingFlags bfAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (startedInitialization) return;
            startedInitialization = true;
            try
            {
                ClientContainer.Apply(this);
                CheckDetection.Hooks.Apply();
                CollectTokens.Hooks.Apply();
                Karma.Hooks.Apply();
                Gates.Hooks.Apply();
                Pearls.Hooks.Apply();
                Echoes.Hooks.Apply();
                FoodQuest.Hooks.Apply();
                DevConsoleIntegration.RegisterCommands();
                Collect.Physical.Hooks.Apply();
            }
            catch (Exception e) { Log(e); }
            initializedWithoutException = true;
            Log("Initialization completed without exception");
        }
    }
}