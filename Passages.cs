namespace alphappy.Archipelago
{
    internal static class Passages
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                On.WinState.CycleCompleted += WinState_CycleCompleted;
            }

            /// <summary>
            /// Report any Passage progress to the client.
            /// </summary>
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
