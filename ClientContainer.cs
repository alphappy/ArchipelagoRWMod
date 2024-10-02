using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System;
using System.Linq;
using UnityEngine;

namespace alphappy.Archipelago
{
    internal class ClientContainer : MonoBehaviour
    {
        internal static ArchipelagoSession session;
        internal static ClientContainer instance;
        internal static LoginSuccessful successfulLoginInfo;

        public void Update()
        {
            if (Messenger.ClientInbox.collectedChecks.Count > 0)
            {
                CheckCollected(Messenger.ClientInbox.collectedChecks.Dequeue());
            }
        }

        internal void StartClient()
        {
            Mod.Log("Session starting...");
            Connect("localhost:38281", "alphappy");
            Mod.Log("Session started successfully");
        }

        internal static void Apply(Mod mod)
        {
            instance = new GameObject("Archipelago").AddComponent<ClientContainer>();
            instance.Initialize();
        }

        internal void Initialize()
        {
            Mod.Log("Client initializing...");
            StartClient();
            Mod.Log("Client initialized successfully");
        }

        internal static void Connect(string server, string user)
        {
            LoginResult result;

            try
            {
                session = ArchipelagoSessionFactory.CreateSession(server);
                result = session.TryConnectAndLogin("Rain World", user, ItemsHandlingFlags.AllItems);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                Mod.Log($"Could not connect: {errorMessage}");
                return;
            }

            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be used to interact with the server and the returned `LoginSuccessful` contains some useful information about the initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            successfulLoginInfo = (LoginSuccessful)result;
            Messenger.GameInbox.connectionState = Messenger.GameInbox.ConnectionState.Connected;
            session.Items.ItemReceived += Items_ItemReceived;
        }

        internal static void Items_ItemReceived(global::Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper helper)
        {
            var item = helper.DequeueItem();
            Mod.Log($"Received item #{item.ItemId}: '{item.ItemName}'");
            Messenger.JustReceivedThis(item);
            if (item.ItemName == "Karma cap increase") CountKarma();
            else if (item.ItemName.StartsWith("Key to")) Messenger.GameInbox.receivedRegionKeys.Add(item.ItemName.Substring(7));
        }

        internal static void CheckCollected(string name)
        {
            Mod.Log($"Marking {name} as a checked location");
            session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName("Rain World", name));
        }

        internal static void CountKarma()
        {
            Messenger.GameInbox.karmaCap = session.Items.AllItemsReceived.Count(x => x.ItemName == "Karma cap increase");
            if (Messenger.GameInbox.karmaCap >= 5) Messenger.GameInbox.karmaCap++;
            if (Messenger.GameInbox.karmaCap > 9) Messenger.GameInbox.karmaCap = 9;
        }
    }
}