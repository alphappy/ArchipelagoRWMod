using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using DevConsole;
using IL.Stove.Sample.Session;
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
        internal static LoginResult lastLoginResult;

        public void Update()
        {
            if (Messenger.ClientInbox.collectedChecks.Count > 0)
            {
                CheckCollected(Messenger.ClientInbox.collectedChecks.Dequeue());
            }
        }

        internal static void Apply(Mod mod)
        {
            instance = new GameObject("Archipelago").AddComponent<ClientContainer>();
        }

        internal static bool Connected
        {
            get
            {
                bool connected = session is not null && session.Socket.Connected && lastLoginResult is LoginSuccessful;
                Messenger.GameInbox.connectionState = connected ? Messenger.GameInbox.ConnectionState.Connected : Messenger.GameInbox.ConnectionState.Disconnected;
                return connected;
            }
        }

        internal static bool EnsureConnected()
        {
            bool c = Connected;
            if (!c) Mod.LogToConsole("You are not connected to an AP room.");
            return c;
        }

        internal static void Say(string message)
        {
            if (!EnsureConnected()) return;
            if (message.Trim().Length == 0) return;
            session.Say(message);
        }

        internal static void Disconnect()
        {
            if (!EnsureConnected()) return;
            Mod.Log("Disconnecting current session");
            Messenger.GameInbox.connectionState = Messenger.GameInbox.ConnectionState.Disconnected;
            session.Socket.DisconnectAsync();
        }

        internal static void Connect(string server, string user, string password = null)
        {
            if (Connected)
            {
                Mod.LogToConsole("You are already connected to an AP room.");
                return;
            }

            Mod.Log($"Attempting to connect to {server} as {user}...");

            try
            {
                session = ArchipelagoSessionFactory.CreateSession(server);
                lastLoginResult = session.TryConnectAndLogin("Rain World", user, ItemsHandlingFlags.AllItems, password: password);
            }
            catch (Exception e)
            {
                lastLoginResult = new LoginFailure(e.GetBaseException().Message);
            }

            if (!lastLoginResult.Successful)
            {
                LoginFailure failure = (LoginFailure)lastLoginResult;
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
            successfulLoginInfo = (LoginSuccessful)lastLoginResult;
            Messenger.GameInbox.connectionState = Messenger.GameInbox.ConnectionState.Connected;
            session.Items.ItemReceived += Items_ItemReceived;
        }

        internal static void Items_ItemReceived(global::Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper helper)
        {
            var item = helper.DequeueItem();
            Mod.Log($"Received item #{item.ItemId}: '{item.ItemName}'");
            Messenger.JustReceivedThis(item);
            if (item.ItemName == "Karma cap increase") CountKarma();
            //else if (item.ItemName.StartsWith("Key to")) Messenger.GameInbox.receivedRegionKeys.Add(item.ItemName.Substring(7));
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