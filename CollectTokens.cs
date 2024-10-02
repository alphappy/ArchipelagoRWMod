using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.Archipelago
{
    internal static class CollectTokens
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                // Make every token available to every Slugcat.
                On.CollectToken.AvailableToPlayer += CollectToken_AvailableToPlayer; 
                // Prevent tokens from being "collected" for save data purpose and detect collection in the first place.
                IL.CollectToken.Pop += CollectToken_Pop;
                // Lie about the collection status of tokens so `Room.Loaded` will actually place them.
                On.PlayerProgression.MiscProgressionData.GetTokenCollected_string_bool += MiscProgressionData_GetTokenCollected_string_bool;
            }

            private static bool MiscProgressionData_GetTokenCollected_string_bool(On.PlayerProgression.MiscProgressionData.orig_GetTokenCollected_string_bool orig, PlayerProgression.MiscProgressionData self, string tokenString, bool sandbox)
            {
                return !Messenger.ArchiMode && orig(self, tokenString, sandbox);
            }

            internal static void CollectToken_Pop(ILContext il)
            {
                // Place a label just before the `for` loop.
                ILCursor b = new(il);
                b.GotoNext(x => x.MatchStloc(0));
                b.GotoPrev(MoveType.Before, x => x.MatchLdcI4(0));
                ILLabel target = b.MarkLabel();

                // Move the cursor to just after the sound plays.
                ILCursor c = new(il);
                c.GotoNext(MoveType.After, 
                    x => x.MatchCallOrCallvirt(typeof(Room).GetMethod(nameof(Room.PlaySound), new Type[] { typeof(SoundID), typeof(Vector2) })));
                c.MoveAfterLabels();

                // Mark the token as collected and prevent the actual unlock from happening in Archi mode.
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(TokenCollected);
                c.Emit(OpCodes.Brtrue, target);
            }
            internal static bool TokenCollected(CollectToken self)
            {
                if (!Messenger.ArchiMode) return false;
                var data = self.placedObj.data as CollectToken.CollectTokenData;
                string color = "gold";
                if (data.isBlue) color = "blue";
                else if (data.isRed) color = "red";
                else if (data.isGreen) color = "green";
                else if (data.isWhite) color = "white";
                Messenger.JustCollectedThis($"CT|{color}|{data.tokenString}");
                return true;
            }
            internal static bool CollectToken_AvailableToPlayer(On.CollectToken.orig_AvailableToPlayer orig, CollectToken self)
            {
                return Messenger.ArchiMode || orig(self);
            }
        }
    }
}
