using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago
{
    internal static class Gates
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                // Consider gate requirement unmet if the key is not in possession.
                new Hook(
                    typeof(RegionGate).GetProperty(nameof(RegionGate.MeetRequirement), Mod.bfAll).GetGetMethod(),
                    typeof(Hooks).GetMethod(nameof(MeetRequirement), Mod.bfAll)
                    );
            }

            internal delegate bool orig_MeetRequirement(RegionGate self);
            internal static bool MeetRequirement(orig_MeetRequirement orig, RegionGate self)
            {
                if (!Messenger.ArchiMode) return orig(self);
                string thisRegion = self.room.world.region.name;
                var split = self.room.abstractRoom.name.Split('_');
                string otherRegion = split[1] == thisRegion ? split[2] : split[1];
                return Messenger.GameInbox.receivedRegionKeys.Contains(otherRegion);
            }
        }
    }
}
