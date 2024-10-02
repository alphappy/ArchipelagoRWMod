using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
