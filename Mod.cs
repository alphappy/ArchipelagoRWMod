using BepInEx;
using System.Reflection;
using DevConsole;
using System.IO;

namespace alphappy.Archipelago
{
    [BepInPlugin(Const.PLUGIN_GUID, Const.PLUGIN_NAME, Const.PLUGIN_VERSION)]
    [BepInDependency("slime-cubed.devconsole", BepInDependency.DependencyFlags.HardDependency)]
    public class Mod : BaseUnityPlugin
    {
        private static bool startedInitialization = false;
        private static bool initializedWithoutException = false;
        public static BepInEx.Logging.ManualLogSource log;

        public static void LogToConsole(string msg) => GameConsole.WriteLine($"[Archipelago] {msg}");
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

        /// <summary>
        /// To be used with reflection methods such as <see cref="Type.GetMethod(string, BindingFlags)"/> to match everything.
        /// </summary>
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
                ClientContainer.Apply();
                CheckDetection.Hooks.Apply();
                CollectTokens.Hooks.Apply();
                Karma.Hooks.Apply();
                Gates.Hooks.Apply();
                Pearls.Hooks.Apply();
                Echoes.Hooks.Apply();
                FoodQuest.Hooks.Apply();
                DevConsoleIntegration.RegisterCommands();
                Collect.Physical.Hooks.Apply();
                MachineConnector.SetRegisteredOI(Const.PLUGIN_GUID, Settings.instance);

                if (!Directory.Exists(Const.SAVE_DATA_PATH)) Directory.CreateDirectory(Const.SAVE_DATA_PATH);

                Log("Initialization completed without exception");
                initializedWithoutException = true;
            }
            catch (Exception e) { Log(e); }
        }
    }
}