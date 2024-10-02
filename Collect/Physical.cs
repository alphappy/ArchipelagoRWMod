using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago.Collect
{
    internal static class Physical
    {
        internal static class Hooks
        {
            internal static void Apply()
            {
                On.RainWorldGame.Update += RainWorldGame_Update;
            }

            private static void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame self)
            {
                orig(self);
                if (Messenger.ArchiMode && Messenger.GameInbox.receivedItems.Peek() is ItemInfo item && receivers.TryGetValue(item.ItemName, out var action))
                {
                    if (action?.Invoke(self) == true) Messenger.GameInbox.receivedItems.Pop(out _);
                }
            }
        }

        internal static Dictionary<string, Func<RainWorldGame, bool>> receivers = new()
        {
            { "Karma cap increase", game => game.GetStorySession.UpdateKarma() },
            { "Rock", game => game.GetGenericItem(AbstractPhysicalObject.AbstractObjectType.Rock) },
            { "Grenade", game => game.GetGenericItem(AbstractPhysicalObject.AbstractObjectType.ScavengerBomb) },
            { "Fuit", game => game.GetGenericItem(AbstractPhysicalObject.AbstractObjectType.DangleFruit) },
            { "Spear", game => game.GetSpear() },
        };

        internal static bool GetGenericItem(this RainWorldGame game, AbstractPhysicalObject.AbstractObjectType type)
        {
            Player player = game.FirstAnyPlayer.realizedCreature as Player;
            if (player.room is null) return false;
            var obj = new AbstractPhysicalObject(player.room.world, type, null, player.abstractCreature.pos, player.room.game.GetNewID());
            player.room.abstractRoom.AddEntity(obj);
            obj.RealizeInRoom();
            return true;
        }

        internal static bool GetSpear(this RainWorldGame game, bool explosive = false, bool electric = false)
        {
            Player player = game.FirstAnyPlayer.realizedCreature as Player;
            if (player.room is null) return false;
            var obj = new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), explosive, electric);
            player.room.abstractRoom.AddEntity(obj);
            obj.RealizeInRoom();
            return true;
        }

        internal static bool TrapStun(this RainWorldGame game)
        {
            Player player = game.FirstAnyPlayer.realizedCreature as Player;
            if (player.room is null) return false;

            return true;
        }
    }
}
