using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago
{
    internal static class Messenger
    {
        internal static class ClientInbox
        {
            internal static Queue<string> collectedChecks = new();
        }

        internal static class GameInbox
        {
            internal static Queue<ItemInfo> receivedItems = new();
            internal static int karmaCap = 0;
            internal static List<string> receivedRegionKeys = new();
            internal enum ConnectionState { Unconnected, Connected, Disconnected }
            internal static ConnectionState connectionState = ConnectionState.Unconnected;
        }

        internal static void JustCollectedThis(string item) => ClientInbox.collectedChecks.Enqueue(item);
        internal static void JustReceivedThis(ItemInfo item) => GameInbox.receivedItems.Enqueue(item);
        internal static bool ArchiMode => GameInbox.connectionState == GameInbox.ConnectionState.Connected
                    || GameInbox.connectionState == GameInbox.ConnectionState.Disconnected;
        internal static bool FoodQuest => ArchiMode;
    }
}
