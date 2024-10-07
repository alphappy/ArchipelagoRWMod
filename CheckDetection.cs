using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago
{
    internal static class CheckDetection
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                // Detect when passages are completed, or when Wanderer pips are earned.
                On.WinState.CycleCompleted += WinState_CycleCompleted;
                // Detect when an Echo is encountered.
                On.SaveState.GhostEncounter += SaveState_GhostEncounter;
            }

            private static void SaveState_GhostEncounter(On.SaveState.orig_GhostEncounter orig, SaveState self, GhostWorldPresence.GhostID ghost, RainWorld rainWorld)
            {
                orig(self, ghost, rainWorld);
                if (Messenger.ArchiMode) Messenger.JustCollectedThis($"Ec|{ghost}");
            }

            private static void WinState_CycleCompleted(On.WinState.orig_CycleCompleted orig, WinState self, RainWorldGame game)
            {
                orig(self, game);
                if (!Messenger.ArchiMode) return;
                foreach (WinState.EndgameTracker tracker in self.endgameTrackers)
                {
                    if (tracker.ID.value == "Traveller" && tracker is WinState.BoolArrayTracker wanderer)
                    {
                        Messenger.JustCollectedThis($"Wa|{wanderer.progress.Count(e => e):D2}");
                    }
                    if (!tracker.GoalAlreadyFullfilled && tracker.GoalFullfilled)
                    {
                        Messenger.JustCollectedThis($"Pa|{tracker.ID.value}");
                    }
                }
            }
        }
    }
}
