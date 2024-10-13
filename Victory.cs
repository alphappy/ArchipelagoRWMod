using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago
{
    internal static class Victory
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                On.Player.Update += Player_Update;
            }

            private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
            {
                orig(self, eu);
                if (Messenger.ArchiMode && self.room is Room room && room.abstractRoom.name == "SB_L01" && self.firstChunk.pos.y < -500f)
                {
                    Messenger.ClientInbox.beatTheGame = true;
                }
            }
        }
    }
}
