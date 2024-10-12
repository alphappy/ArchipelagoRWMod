namespace alphappy.Archipelago
{
    internal static class Echoes
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                // Set the karma cap according to the archi instead of according to the number of visited echoes after encountering an echo.
                IL.SaveState.GhostEncounter += SaveState_GhostEncounter;
                // Make echoes spawn without needing to be primed first (Remix setting).
                IL.World.SpawnGhost += World_SpawnGhost;
            }

            private static void World_SpawnGhost(ILContext il)
            {
                ILCursor c = new(il);
                c.GotoNext(x => x.MatchNewobj(typeof(GhostWorldPresence)));
                c.GotoPrev(MoveType.After, x => x.MatchLdloc(2));
                c.Emit(OpCodes.Ldloc_1);
                c.EmitDelegate(SpawnTheGhostAnyway);
            }

            internal static bool SpawnTheGhostAnyway(int previouslyTalkedTo, bool prev)
            {
                if (Messenger.ArchiMode && Settings.boolPrePrimedEchoes.Value) return previouslyTalkedTo < 2;
                return prev;
            }

            private static void SaveState_GhostEncounter(ILContext il)
            {
                ILCursor c = new(il);
                c.GotoNext(x => x.MatchLdfld(typeof(DeathPersistentSaveData).GetField(nameof(DeathPersistentSaveData.karmaCap))));
                c.GotoPrev(MoveType.After, x => x.MatchLdloc(1));
                c.EmitDelegate(ReplaceKarmaCap);
            }

            internal static int ReplaceKarmaCap(int prev) => Messenger.ArchiMode ? Messenger.GameInbox.karmaCap : prev;
        }
    }
}
