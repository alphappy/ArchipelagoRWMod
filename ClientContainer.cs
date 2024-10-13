using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using System.IO;

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
            if (Messenger.ClientInbox.collectedChecks.TryPop(out string item))
            {
                CheckCollected(item);
            }
            if (Messenger.ClientInbox.toBeSaved.TryPop(out string save))
            {
                File.AppendAllText(saveFilepath, $"{save}\n");
            }
            if (Messenger.ClientInbox.beatTheGame)
            {
                Complete();
                Messenger.ClientInbox.beatTheGame = false;
            }
        }
        /// <summary>
        /// Add a component to a <see cref="GameObject"/> to put the client on an update loop separate from the <see cref="RainWorld"/>.
        /// </summary>
        internal static void Apply()
        {
            instance = new GameObject("Archipelago").AddComponent<ClientContainer>();
        }

        /// <summary>
        /// Check whether the session is <em>currently</em> connected, regardless of whether the game is currently in ArchiMode.
        /// </summary>
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

        /// <summary>
        /// Connect to an AP room.
        /// If successful, <see cref="session"/> will contain an <see cref="ArchipelagoSession"/> from which all AP operations will be conducted.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
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
            Messenger.Reset();
            PrepareSaveData(session.RoomState.Seed, successfulLoginInfo.Slot);
            Messenger.GameInbox.connectionState = Messenger.GameInbox.ConnectionState.Connected;
            session.Items.ItemReceived += Items_ItemReceived;
        }

        internal static string saveFilepath = "";

        /// <summary>
        /// Prepare a save data file for a given AP seed.  This file will appear in ModConfigs.
        /// </summary>
        /// <param name="seed"></param>
        internal static void PrepareSaveData(string seed, int slotNumber)
        {
            Messenger.GameInbox.alreadyAwarded.Clear();
            saveFilepath = $"{Const.SAVE_DATA_PATH}\\{seed}_{slotNumber}";
            if (File.Exists(saveFilepath))
            {
                Mod.Log($"Loading save data for room '{seed}', slot {slotNumber}...");
                string[] lines = File.ReadAllLines(saveFilepath);
                foreach (string line in lines)
                {
                    if (line == "Karma cap increase") continue;
                    if (Messenger.GameInbox.alreadyAwarded.TryGetValue(line, out _))
                    {
                        Messenger.GameInbox.alreadyAwarded[line] += 1;
                    }
                    else
                    {
                        Messenger.GameInbox.alreadyAwarded[line] = 1;
                    }
                }
            }
            else
            {
                Mod.Log($"No save data!  Creating save data for room '{seed}', slot {slotNumber}...");
                File.WriteAllText(saveFilepath, "");
            }
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

        /// <summary>
        /// Counts up current Karma cap increases to compute what should be current max Karma.
        /// </summary>
        internal static void CountKarma()
        {
            Messenger.GameInbox.karmaCap = session.Items.AllItemsReceived.Count(x => x.ItemName == "Karma cap increase");
            if (Messenger.GameInbox.karmaCap >= 5) Messenger.GameInbox.karmaCap++;
            if (Messenger.GameInbox.karmaCap > 9) Messenger.GameInbox.karmaCap = 9;
        }

        internal static void Complete()
        {
            StatusUpdatePacket statusUpdatePacket = new()
            {
                Status = ArchipelagoClientState.ClientGoal
            };
            session.Socket.SendPacket(statusUpdatePacket);
        }
    }
}