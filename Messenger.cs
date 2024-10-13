using Archipelago.MultiClient.Net.Models;

namespace alphappy.Archipelago
{
    internal static class Messenger
    {
        internal static class ClientInbox
        {
            internal static Queue<string> collectedChecks = new();
            internal static Queue<string> toBeSaved = new();
            internal static bool beatTheGame = false;
        }

        internal static class GameInbox
        {
            internal static Queue<ItemInfo> receivedItems = new();
            internal static int karmaCap = 0;
            internal static List<string> receivedRegionKeys = new();
            internal enum ConnectionState { Unconnected, Connected, Disconnected }
            internal static ConnectionState connectionState = ConnectionState.Unconnected;
            internal static Dictionary<string, int> alreadyAwarded = new();
        }

        /// <summary>
        /// Reset the current state of the inboxes.
        /// </summary>
        internal static void Reset()
        {
            ClientInbox.collectedChecks.Clear();
            ClientInbox.toBeSaved.Clear();
            GameInbox.receivedItems.Clear();
            GameInbox.receivedRegionKeys.Clear();
            GameInbox.connectionState = GameInbox.ConnectionState.Unconnected;
            GameInbox.alreadyAwarded.Clear();
        }

        /// <summary>
        /// Tell the client that a check was just collected.
        /// </summary>
        /// <param name="item"></param>
        internal static void JustCollectedThis(string item) => ClientInbox.collectedChecks.Enqueue(item);
        /// <summary>
        /// Tell the game that an item was just received from the server.
        /// If the item is previously awarded filler, it is not awarded again.
        /// </summary>
        /// <param name="item"></param>
        internal static void JustReceivedThis(ItemInfo item)
        {
            if (IsFiller(item.ItemName) && GameInbox.alreadyAwarded.GetOrDefault(item.ItemName, 0) > 0)
            {
                Mod.Log($"Ignoring {item.ItemName} because it was already awarded previously");
                GameInbox.alreadyAwarded[item.ItemName] -= 1;
                return;
            }
            GameInbox.receivedItems.Enqueue(item);
            ClientInbox.toBeSaved.Enqueue(item.ItemName);
        }

        /// <summary>
        /// Determines whether an item is a filler item that should not be awarded a second time.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static bool IsFiller(string item)
        {
            if (item.StartsWith("Key to")) return false;
            if (item == "Karma cap increase") return false;
            return true;
        }

        internal static bool ArchiMode => GameInbox.connectionState == GameInbox.ConnectionState.Connected
                    || GameInbox.connectionState == GameInbox.ConnectionState.Disconnected;
        internal static bool FoodQuest => ArchiMode;
    }
}
