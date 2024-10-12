using System.Runtime.CompilerServices;

namespace alphappy.Archipelago
{
    internal static class Pearls
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                // Detect when the player picks up a pearl and, if it is unique, collect it.
                IL.DataPearl.Update += DataPearl_Update;
            }

            private static void DataPearl_Update(ILContext il)
            {
                ILCursor c = new(il);
                c.GotoNext(MoveType.After, x => x.MatchLdfld(typeof(DataPearl).GetField(nameof(DataPearl.uniquePearlCountedAsPickedUp))));
                c.MoveAfterLabels();
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(PlayerGrabbedPearl);
            }

            internal static void PlayerGrabbedPearl(DataPearl self)
            {
                if (self.GetData().collected || !Messenger.ArchiMode) return;
                string name = self.AbstractPearl.dataPearlType.value;
                if (!name.StartsWith("Misc"))
                {
                    Messenger.JustCollectedThis($"Pe|{name}");
                    self.GetData().collected = true;
                }
            }
        }

        internal static ConditionalWeakTable<DataPearl, Data> table = new();
        internal static Data GetData(this DataPearl self) => table.GetOrCreateValue(self);

        internal class Data
        {
            internal bool collected;
        }
    }
}
