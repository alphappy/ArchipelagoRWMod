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
                if (!Messenger.ArchiMode) return;
                if (Messenger.GameInbox.receivedItems.Count == 0) return;
                ItemInfo item = Messenger.GameInbox.receivedItems.Dequeue();
                Mod.Log($"Seeing {item.ItemName}");
                if (item.ItemName.StartsWith("Key to")) Messenger.GameInbox.receivedRegionKeys.Add(item.ItemName.Substring(7));
                else if (receivers.TryGetValue(item.ItemName, out var action))
                {
                    Mod.Log($"Attempting to award item {item.ItemName}");
                    if (action?.Invoke(self) == false)
                    {
                        Mod.Log($"Item was not awarded; going back to queue");
                        Messenger.GameInbox.receivedItems.Enqueue(item);
                    }
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

            { "Stun Trap", game => game.TrapStun() },
            { "Timer Trap", game => game.TrapCycleTimer() },
            { "Zoomies Trap", game => game.TrapZoomiesPlayer() },
            { "Red Lizard Trap", game => game.TrapSpawnCreatureNearby(CreatureTemplate.Type.RedLizard) },
            { "Red Centipede Trap", game => game.TrapSpawnCreatureNearby(CreatureTemplate.Type.RedCentipede) },
            { "Spitter Spider Trap", game => game.TrapSpawnCreatureNearby(CreatureTemplate.Type.SpitterSpider) },
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
            Player player = game.FirstAnyPlayer?.realizedCreature as Player;
            if (player.room is null) return false;
            player.Stun(85);
            return true;
        }

        internal static bool TrapCycleTimer(this RainWorldGame game, int seconds = 120)
        {
            game.world.rainCycle.timer += 40 * seconds;
            return true;
        }

        internal static bool TrapZoomiesPlayer(this RainWorldGame game)
        {
            Player player = game.FirstAnyPlayer?.realizedCreature as Player;
            if (player.room is null) return false;
            player.room.updateList.Add(player);
            return true;
        }

        internal static bool TrapSpawnCreatureNearby(this RainWorldGame game, CreatureTemplate.Type template)
        {
            Player player = game.FirstAnyPlayer?.realizedCreature as Player;
            if (player.room is Room room && game.world.GetAbstractRoom(room.abstractRoom.connections.Pick()) is AbstractRoom abstractRoom)
            {
                AbstractCreature crit = new(game.world, StaticWorld.GetCreatureTemplate(template), null, abstractRoom.RandomNodeInRoom(), game.GetNewID());
                abstractRoom.AddEntity(crit);
                if (abstractRoom.realizedRoom is not null)
                {
                    crit.RealizeInRoom();
                    crit.abstractAI.RealAI.tracker.SeeCreature(player.abstractCreature);
                }
                return true;
            }
            return false;
        }
    }
}
