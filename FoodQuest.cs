using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago
{
    internal static class FoodQuest
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                // Detect food being eaten for food quest and remove Gourmand-only restriction.
                IL.PlayerSessionRecord.AddEat += PlayerSessionRecord_AddEat;
                // Allow the HUD element to be created for slugcats other than Gourmand.
                IL.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
            }

            private static void HUD_InitSinglePlayerHud(ILContext il)
            {
                ILCursor c = new(il);
                c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Equality")));
                c.MoveAfterLabels();
                c.EmitDelegate(YesItIsMeGourmand);
            }

            private static void PlayerSessionRecord_AddEat(ILContext il)
            {
                ILCursor c = new(il);
                for (int i = 0; i < 2; i++)
                {
                    c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Equality")));
                    c.MoveAfterLabels();
                    c.EmitDelegate(YesItIsMeGourmand);
                }

                c = new(il);
                for (int i = 0; i < 2; i++)
                {
                    c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(Room).GetMethod(nameof(Room.PlaySound), new Type[] {typeof(SoundID), typeof(float), typeof(float), typeof(float)})));
                    c.MoveAfterLabels();
                    c.Emit(OpCodes.Ldloc_2);
                    c.EmitDelegate(DetectEatenObject);
                }
            }

            internal static bool YesItIsMeGourmand(bool prev) => Messenger.FoodQuest || prev;
            internal static void DetectEatenObject(int index)
            {
                if (Messenger.FoodQuest) Messenger.JustCollectedThis($"FQ|{index + 1}");
            }
        }
    }
}
