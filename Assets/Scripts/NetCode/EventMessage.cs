using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System;

namespace WTI.NetCode
{
    [Serializable]
    public class PositionListData
    {
        public List<Vector3> data;
    }

    [Serializable]
    public class Parameter
    {
        public object value;
        public Type type;
    }

    [Serializable]
    public class EventMessageData
    {
        public string methodName;
        public string[] parameters;
        public Type[] parameterTypes;
    }

    public class EventMessage : NetworkBehaviour
    {
        private const byte SIDE_SHAKE_EVENT_CODE = 1;
        private const byte FORWARD_SWING_EVENT_CODE = 2;
        private const byte DRAW_LINE_EVENT_CODE = 3;
        private const byte TILT_ANGLE_EVENT_CODE = 4;
        private const byte GOAL_KEEPER_POSITION_EVENT_CODE = 5;
        private const byte FOOTBALL_POSITION_EVENT_CODE = 6;
        private const byte SHOOT_EVENT_CODE = 7;
        private const byte STRIKER_SELECTION_EVENT_CODE = 8;
        private const byte GOAL_KEEPER_SELECTION_EVENT_CODE = 9;
        private const byte SHOOT_TIMER_STARTED_EVENT_CODE = 10;
        private const byte SHOOT_TIMER_ENDED_EVENT_CODE = 11;
        private const byte NEXT_ROUND_EVENT_CODE = 12;
        private const byte SEND_FOOTBALL_PLAYER_DATA_EVENT_CODE = 13;
        private const byte SEND_RESET_MATCH_EVENT_CODE = 14;
        private const byte PART_OF_DAY_EVENT_CODE = 15;
        private const byte WEATHER_EVENT_CODE = 16;

        private const byte DISCONNECT_EVENT_CODE = 00;

        public enum TargetMessage
        {
            All = 0,
            AllClient = 1,
            Client = 2,
            Server = 3
        }

        private void Start()
        {
            //EventManager.onNetworkConnected += OnNetworkConnected;
            //EventManager.sendOnLineDrawn += SendDrawLineEvent;
            //EventManager.sendOnPhoneShaked += SendPhoneShakeEvent;
            //EventManager.sendOnPhoneTilted += SendPhoneTiltAngle;

            EventManager.onPhoneSideShaked += SendPhoneShakeEvent;
            EventManager.onDrawingLine += SendPhoneDrawLineEvent;
            EventManager.onPhoneTilted += SendPhoneTiltAngle;
            EventManager.onCapturedPhotoSent += SendCapturedPhoto;
            EventManager.onEncodedCapturedPhotoSent += SendEncodedCapturedPhoto;
            EventManager.onGoalKeeperPositionUpdated += SendGoalKeeperPosition;
            EventManager.onFootballUpdated += SendFootballPosition;
            EventManager.onShootAnimationPlayed += SendShootTriggered;
            EventManager.onStrikerSelected += SendStrikerOccupied;
            EventManager.onGoalKeeperSelected += SendGoalKeeperOccupied;
            EventManager.onShootTimerStarted += SendShootTimerStarted;
            EventManager.onShootTimerEnded += SendShootTimerEnded;
            EventManager.onNextRoundStarted += SendNextRoundStarted;
            EventManager.onFootballDataSent += SendFootballPlayerData;
            EventManager.onResetMatchStarted += SendResetMatch;
            EventManager.onPartOfDayChanged += SendPartOfDay;
            EventManager.onWeatherChanged += SendWeather;

            EventManager.onOtherPlayerDisconnected += SendOtherPlayerDisconnectedStat;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        //private void OnNetworkConnected()
        //{
        //    NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(messageName, ReceiveMessage);
        //    Debug.Log("OnNetworkSpawn");
        //}

        public override void OnNetworkSpawn()
        {
            NetworkManager.CustomMessagingManager.OnUnnamedMessage += ReceiveMessage;
            Debug.Log("OnNetworkSpawn");
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.CustomMessagingManager.OnUnnamedMessage -= ReceiveMessage;
        }

        public void SendUnnamedMessage(string dataToSend)
        {
            Debug.Log("data to send : " + dataToSend);
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            // Tip: Placing the writer within a using scope assures it will
            // be disposed upon leaving the using scope
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(MessageType());

                // Write our string message
                writer.WriteValueSafe(dataToSend);
                if (IsServer)
                {
                    // This is a server-only method that will broadcast the unnamed message.
                    // Caution: Invoking this method on a client will throw an exception!
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    // This method can be used by a client or server (client to server or server to client)
                    customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                }
            }
        }

        private void SendPhoneShakeEvent(ulong senderClientId)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(SIDE_SHAKE_EVENT_CODE);
                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendPhoneDrawLineEvent(ulong senderClientId, List<Vector3> points)
        {
            var writer = new FastBufferWriter(256000000, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(DRAW_LINE_EVENT_CODE);

                writer.WriteValueSafe(points.ToArray());
                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer, NetworkDelivery.ReliableFragmentedSequenced);
                    }
                }
            }
        }

        private void SendPhoneTiltAngle(ulong senderClientId, float angle)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(TILT_ANGLE_EVENT_CODE);
                writer.WriteValueSafe(angle);
                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendCapturedPhoto(ulong senderClientId, byte[] photo, Vector2 size)
        {
            //NetworkController.Instance.ipText.text = "SendCapturedPhoto : " + photo.Length;
            try
            {
                var writer = new FastBufferWriter(10000000, Allocator.Temp);
                var customMessagingManager = NetworkManager.CustomMessagingManager;
                using (writer)
                {

                    // Write our message type
                    writer.WriteValueSafe(FORWARD_SWING_EVENT_CODE);
                    writer.WriteValueSafe(photo);
                    writer.WriteValueSafe(size);
                    if (IsServer)
                    {
                        customMessagingManager.SendUnnamedMessageToAll(writer);
                    }
                    else
                    {
                        if (NetworkManager.IsConnectedClient)
                        {
                            // This method can be used by a client or server (client to server or server to client)
                            customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NetworkController.Instance.ipText.text = "err : " + exc.Message;
            }
        }

        private void SendEncodedCapturedPhoto(ulong senderClientId, string encodedPhoto, Vector2 size)
        {
            //NetworkController.Instance.ipText.text = "SendCapturedPhoto : " + photo.Length;
            try
            {
                var writer = new FastBufferWriter(10000000, Allocator.Temp);
                var customMessagingManager = NetworkManager.CustomMessagingManager;
                using (writer)
                {

                    // Write our message type
                    writer.WriteValueSafe(FORWARD_SWING_EVENT_CODE);
                    writer.WriteValueSafe(encodedPhoto);
                    writer.WriteValueSafe(size);
                    if (IsServer)
                    {
                        customMessagingManager.SendUnnamedMessageToAll(writer);
                    }
                    else
                    {
                        if (NetworkManager.IsConnectedClient)
                        {
                            // This method can be used by a client or server (client to server or server to client)
                            customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer, NetworkDelivery.ReliableFragmentedSequenced);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NetworkController.Instance.ipText.text = "err : " + exc.Message;
            }
        }

        private void SendGoalKeeperPosition(ulong senderClientId, Vector3 position, Vector3 handPosition)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(GOAL_KEEPER_POSITION_EVENT_CODE);
                writer.WriteValueSafe(position);
                writer.WriteValueSafe(handPosition);

                if (IsServer)
                {
                    //customMessagingManager.SendUnnamedMessageToAll(writer);
                    List<ulong> clientsIds = new List<ulong>();
                    clientsIds.AddRange((List<ulong>)NetworkManager.ConnectedClientsIds);
                    clientsIds.Remove(senderClientId);
                    customMessagingManager.SendUnnamedMessage(clientsIds, writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendFootballPosition(ulong senderClientId, Vector3 position)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                try
                {
                    // Write our message type
                    writer.WriteValueSafe(FOOTBALL_POSITION_EVENT_CODE);
                    writer.WriteValueSafe(position);

                    if (IsServer)
                    {
                        //customMessagingManager.SendUnnamedMessageToAll(writer);
                        List<ulong> clientsIds = new List<ulong>();
                        clientsIds.AddRange((List<ulong>)NetworkManager.ConnectedClientsIds);
                        clientsIds.Remove(senderClientId);
                        customMessagingManager.SendUnnamedMessage(clientsIds, writer);
                    }
                    else
                    {
                        if (NetworkManager.IsConnectedClient)
                        {
                            //customMessagingManager.SendUnnamedMessageToAll(writer);
                            customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                        }
                    }
                }
                catch (Exception exc)
                {
                    NetworkController.Instance.errorMessage = "err : " + exc.Message;
                }
            }
        }

        private void SendShootTriggered(ulong senderClientId)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                try
                {
                    // Write our message type
                    writer.WriteValueSafe(SHOOT_EVENT_CODE);

                    if (IsServer)
                    {
                        //customMessagingManager.SendUnnamedMessageToAll(writer);
                        List<ulong> clientsIds = new List<ulong>();
                        clientsIds.AddRange((List<ulong>)NetworkManager.ConnectedClientsIds);
                        clientsIds.Remove(senderClientId);
                        customMessagingManager.SendUnnamedMessage(clientsIds, writer);
                        NetworkController.Instance.errorMessage = "SendShootTriggered";
                    }
                    else
                    {
                        if (NetworkManager.IsConnectedClient)
                        {
                            //customMessagingManager.SendUnnamedMessageToAll(writer);
                            customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                        }
                    }
                }
                catch (Exception exc)
                {
                    NetworkController.Instance.errorMessage = "err : " + exc.Message;
                }
            }
        }

        private void SendStrikerOccupied(ulong senderClientId, bool isSelected)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(STRIKER_SELECTION_EVENT_CODE);
                writer.WriteValueSafe(isSelected);

                if (IsServer)
                {
                    List<ulong> clientsIds = new List<ulong>();
                    clientsIds.AddRange((List<ulong>)NetworkManager.ConnectedClientsIds);
                    clientsIds.Remove(senderClientId);
                    customMessagingManager.SendUnnamedMessage(clientsIds, writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendGoalKeeperOccupied(ulong senderClientId, bool isSelected)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(GOAL_KEEPER_SELECTION_EVENT_CODE);
                writer.WriteValueSafe(isSelected);

                if (IsServer)
                {
                    //customMessagingManager.SendUnnamedMessageToAll(writer);
                    List<ulong> clientsIds = new List<ulong>();
                    clientsIds.AddRange((List<ulong>)NetworkManager.ConnectedClientsIds);
                    clientsIds.Remove(senderClientId);
                    customMessagingManager.SendUnnamedMessage(clientsIds, writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendShootTimerStarted(ulong senderClientId)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(SHOOT_TIMER_STARTED_EVENT_CODE);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendShootTimerEnded(ulong senderClientId)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(SHOOT_TIMER_ENDED_EVENT_CODE);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendNextRoundStarted(ulong senderClientId)
        {
            NetworkController.Instance.errorMessage = "SendNextRoundStarted";
            Debug.Log("SendNextRoundStarted");
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(NEXT_ROUND_EVENT_CODE);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                    Debug.Log("SendNextRound to All");
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
            Debug.Log("SendNextRoundStarted Succeed");
        }

        private void SendFootballPlayerData(FootballPlayerData data)
        {
            Debug.Log("SendFootballPlayerData " + data.matchDataList.Count);
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                List<int> p1WinList = new List<int>();

                for (int i = 0; i < data.matchDataList.Count; i++)
                {
                    if (data.matchDataList[i].winnerId == (int)ScoreController.Player.player1)
                    {
                        p1WinList.Add(1);
                    }
                    else if (data.matchDataList[i].winnerId == (int)ScoreController.Player.player2)
                    {
                        p1WinList.Add(0);
                    }
                }

                // Write our message type
                writer.WriteValueSafe(SEND_FOOTBALL_PLAYER_DATA_EVENT_CODE);
                writer.WriteValueSafe(data.isStrikerSelected);
                writer.WriteValueSafe(data.isGoalKeeperSelected);
                writer.WriteValueSafe(data.shootTime);
                writer.WriteValueSafe(data.p1Score);
                writer.WriteValueSafe(data.p2Score);
                writer.WriteValueSafe(p1WinList.ToArray());

                string listString = "";
                for (int i = 0; i < p1WinList.Count; i++)
                {
                    listString += p1WinList[i] + ",";
                }
                Debug.Log("scoreList : " + listString);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
            Debug.Log("SendFootballPlayerData Succeed");
        }

        private void SendResetMatch(ulong senderClientId)
        {
            NetworkController.Instance.errorMessage = "SendResetMatch";
            Debug.Log("SendResetMatch");
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(SEND_RESET_MATCH_EVENT_CODE);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                    Debug.Log("SendNextRound to All");
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
            Debug.Log("SendResetMatch Succeed");
        }

        private void SendPartOfDay(ulong senderClientId, int part)
        {
            NetworkController.Instance.errorMessage = "SendPartOfDay";
            Debug.Log("SendPartOfDay");
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(PART_OF_DAY_EVENT_CODE);
                writer.WriteValueSafe(part);

                if (IsServer)
                {
                    List<ulong> clientsIds = new List<ulong>();
                    clientsIds.AddRange((List<ulong>)NetworkManager.ConnectedClientsIds);
                    clientsIds.Remove(senderClientId);
                    customMessagingManager.SendUnnamedMessage(clientsIds, writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendWeather(ulong senderClientId, int weather)
        {
            NetworkController.Instance.errorMessage = "SendWeather";
            Debug.Log("SendWeather");
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(WEATHER_EVENT_CODE);
                writer.WriteValueSafe(weather);

                if (IsServer)
                {
                    List<ulong> clientsIds = new List<ulong>();
                    clientsIds.AddRange((List<ulong>)NetworkManager.ConnectedClientsIds);
                    clientsIds.Remove(senderClientId);
                    customMessagingManager.SendUnnamedMessage(clientsIds, writer);
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
        }

        private void SendOtherPlayerDisconnectedStat(ulong senderClientId, ulong clientId)
        {
            NetworkController.Instance.errorMessage = "SendOtherPlayerDisconnectedStat";
            Debug.Log("SendOtherPlayerDisconnectedStat");
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            using (writer)
            {

                // Write our message type
                writer.WriteValueSafe(DISCONNECT_EVENT_CODE);
                writer.WriteValueSafe(clientId);

                if (IsServer)
                {
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                    Debug.Log("SendNextRound to All");
                }
                else
                {
                    if (NetworkManager.IsConnectedClient)
                    {
                        // This method can be used by a client or server (client to server or server to client)
                        customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                    }
                }
            }
            Debug.Log("SendOtherPlayerDisconnectedStat Succeed");
        }

        protected virtual byte MessageType()
        {
            return 0;
        }

        private void ReceiveMessage(ulong clientId, FastBufferReader reader)
        {
            // Read the message type value that is written first when we send
            // this unnamed message.
            reader.ReadValueSafe(out byte messageType);
            // Example purposes only, you might handle this in a more optimal way
            Debug.Log("message type : " + messageType.ToString());
            switch (messageType)
            {
                //case 0:
                //    OnReceivedUnnamedMessage(clientId, reader);
                //    break;
                case SIDE_SHAKE_EVENT_CODE:
                    OnReceivedSideShakeEventMessage(clientId, reader);
                    break;
                case FORWARD_SWING_EVENT_CODE:
                    OnReceivedForwardSwingEventMessage(clientId, reader);
                    break;
                case DRAW_LINE_EVENT_CODE:
                    OnReceivedDrawLineEventMessage(clientId, reader);
                    break;
                case TILT_ANGLE_EVENT_CODE:
                    OnReceivedPhoneTiltEventMessage(clientId, reader);
                    break;
                case GOAL_KEEPER_POSITION_EVENT_CODE:
                    OnReceivedGoalKeeperPositionEventMessage(clientId, reader);
                    break;
                case FOOTBALL_POSITION_EVENT_CODE:
                    OnReceivedFootballPositionEventMessage(clientId, reader);
                    break;
                case SHOOT_EVENT_CODE:
                    OnReceivedShootEventMessage(clientId, reader);
                    break;
                case STRIKER_SELECTION_EVENT_CODE:
                    OnReceivedStrikerSelectedEventMessage(clientId, reader);
                    break;
                case GOAL_KEEPER_SELECTION_EVENT_CODE:
                    OnReceivedGoalKeeperSelectedEventMessage(clientId, reader);
                    break;
                case SHOOT_TIMER_STARTED_EVENT_CODE:
                    OnReceivedStartShootTimerEventMessage(clientId, reader);
                    break;
                case SHOOT_TIMER_ENDED_EVENT_CODE:
                    OnReceivedEndShootTimerEventMessage(clientId, reader);
                    break;
                case NEXT_ROUND_EVENT_CODE:
                    OnReceivedNextRoundEventMessage(clientId, reader);
                    break;
                case SEND_FOOTBALL_PLAYER_DATA_EVENT_CODE:
                    OnReceivedFootballPlayerDataEventMessage(clientId, reader);
                    break;
                case SEND_RESET_MATCH_EVENT_CODE:
                    OnReceivedResetMatchEventMessage(clientId, reader);
                    break;
                case PART_OF_DAY_EVENT_CODE:
                    OnReceivedPartOfDayEventMessage(clientId, reader);
                    break;
                case WEATHER_EVENT_CODE:
                    OnReceivedWeatherEventMessage(clientId, reader);
                    break;
                case DISCONNECT_EVENT_CODE:
                    OnReceivedOtherPlayerDisconnectedEventMessage(clientId, reader);
                    break;
            }
        }

        protected void OnReceivedUnnamedMessage(ulong clientId, FastBufferReader reader)
        {
            var stringMessage = string.Empty;
            reader.ReadValueSafe(out stringMessage);
            if (IsServer)
            {
                Debug.Log($"Server received unnamed message of type ({MessageType()}) from client " +
                    $"({clientId}) that contained the string: \"{stringMessage}\"");

                // As an example, we could also broadcast the client message to everyone
                SendUnnamedMessage($"Newly connected client sent this greeting: \"{stringMessage}\"");
            }
            else
            {
                Debug.Log(stringMessage);
            }
        }

        protected void OnReceivedSideShakeEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received message");

            if (IsServer)
            {
                //SendPhoneShakeEvent();
            }
            OnPhoneSideShaked();
        }

        protected void OnReceivedForwardSwingEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received message");
            //reader.ReadValueSafe(out byte[] photo);
            reader.ReadValueSafe(out string encodedPhoto);
            reader.ReadValueSafe(out Vector2 size);
            Debug.Log("encoded : " + encodedPhoto);
            if (IsServer)
            {
                //SendPhoneShakeEvent();
            }
            OnPhoneForwardSwing(encodedPhoto, (int)size.x, (int)size.y);
        }

        protected void OnReceivedDrawLineEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received message");
            reader.ReadValueSafe(out Vector3[] points);

            //PositionListData positionData = JsonUtility.FromJson<PositionListData>(pointsJson);
            //Debug.Log("json : " + pointsJson);
            if (IsServer)
            {
                //SendPhoneDrawLineEvent(points.ToList());
            }
            OnLineDrawn(points.ToList());
        }

        protected void OnReceivedPhoneTiltEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("received message");
            reader.ReadValueSafe(out float angle);

            if (IsServer)
            {
                //SendPhoneTiltAngle(angle);
            }
            OnPhoneAngleChanged(angle);
        }

        protected void OnReceivedGoalKeeperPositionEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedGoalKeeperPositionEventMessage message");
            reader.ReadValueSafe(out Vector3 position);
            reader.ReadValueSafe(out Vector3 handPosition);
            if (IsServer)
            {
                SendGoalKeeperPosition(clientId, position, handPosition);
            }
            OnGoalKeeperPositionUpdated(position, handPosition);
        }

        protected void OnReceivedFootballPositionEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedFootballPositionEventMessage message");
            reader.ReadValueSafe(out Vector3 position);
            if (IsServer)
            {
                SendFootballPosition(clientId, position);
            }
            OnFootballPositionUpdated(position);
        }

        protected void OnReceivedShootEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedShootEventMessage message");
            if (IsServer)
            {
                SendShootTriggered(clientId);
            }
            OnShootTriggered();
        }

        protected void OnReceivedStrikerSelectedEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedStrikerSelectedEventMessage message");
            reader.ReadValueSafe(out bool isSelected);
            if (IsServer)
            {
                SendStrikerOccupied(clientId, isSelected);
            }
            OnStrikerSelected(clientId, isSelected);
        }

        protected void OnReceivedGoalKeeperSelectedEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedGoalKeeperSelectedEventMessage message");
            reader.ReadValueSafe(out bool isSelected);
            if (IsServer)
            {
                SendGoalKeeperOccupied(clientId, isSelected);
            }
            OnGoalKeeperSelected(clientId, isSelected);
        }

        protected void OnReceivedStartShootTimerEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedStartShootTimerEventMessage message");
            if (IsServer)
            {
                //SendShootTimerStarted(clientId);
            }
            OnShootTimerStarted();
        }

        protected void OnReceivedEndShootTimerEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedEndShootTimerEventMessage message");
            if (IsServer)
            {
                //SendShootTimerEnded(clientId);
            }
            OnShootTimerEnded();
        }

        protected void OnReceivedNextRoundEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedNextRoundEventMessage message");
            if (IsServer)
            {
                //SendNextRoundStarted(clientId);
            }
            OnNextRoundStarted();
        }

        protected void OnReceivedFootballPlayerDataEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedFootballPlayerDataEventMessage message");
            reader.ReadValueSafe(out bool isStrikerSelected);
            reader.ReadValueSafe(out bool isGoalKeeperSelected);
            reader.ReadValueSafe(out float shootTime);
            reader.ReadValueSafe(out int p1Score);
            reader.ReadValueSafe(out int p2Score);
            reader.ReadValueSafe(out int[] p1Wins);


            List<MatchData> matchDataList = new List<MatchData>();
            for (int i = 0; i < p1Wins.Length; i++)
            {
                matchDataList.Add(new MatchData
                {
                    match = i,
                    winnerId = (p1Wins[i] == 1 ? (int)ScoreController.Player.player1 : (int)ScoreController.Player.player2)
                });
            }

            FootballPlayerData data = new FootballPlayerData
            {
                isStrikerSelected = isStrikerSelected,
                isGoalKeeperSelected = isGoalKeeperSelected,
                shootTime = shootTime,
                p1Score = p1Score,
                p2Score = p2Score,
                matchDataList = matchDataList
            };

            if (IsServer)
            {
                //SendFootballPlayerData(data);
            }
            OnFootballPlayerDataUpdated(data);
        }

        protected void OnReceivedResetMatchEventMessage(ulong clientId, FastBufferReader reader)
        {
            Debug.Log("OnReceivedResetMatchEventMessage message");
            if (IsServer)
            {
                //SendNextRoundStarted(clientId);
            }
            OnResetMatchStarted();
        }

        protected void OnReceivedPartOfDayEventMessage(ulong client, FastBufferReader reader)
        {
            Debug.Log("OnReceivedPartOfDayEventMessage message");
            reader.ReadValueSafe(out int part);
            if (IsServer)
            {
                SendPartOfDay(client, part);
            }
            OnPartOfDayChanged(part);
        }

        protected void OnReceivedWeatherEventMessage(ulong client, FastBufferReader reader)
        {
            Debug.Log("OnPartOfDay message");
            reader.ReadValueSafe(out int weather);
            if (IsServer)
            {
                SendWeather(client, weather);
            }
            OnWeatherChanged(weather);
        }

        protected void OnReceivedOtherPlayerDisconnectedEventMessage(ulong clientid, FastBufferReader reader)
        {
            Debug.Log("OnReceivedOtherPlayerDisconnectedEventMessage");
            reader.ReadValueSafe(out ulong disconnectedClientId);
            OnOtherPlayerDisconnected(disconnectedClientId);
        }

        private void OnPhoneSideShaked()
        {
            Debug.Log("OnPhoneShaked");
            GameManager.Instance.PlayParticle();
        }

        private void OnPhoneForwardSwing(byte[] photo)
        {
            Debug.Log("OnPhoneShaked");
            GameManager.Instance.SetPicture(photo);
        }

        private void OnPhoneForwardSwing(byte[] photo, int width, int height)
        {
            Debug.Log("OnPhoneShaked");
            GameManager.Instance.SetPicture(photo, width, height);
        }

        private void OnPhoneForwardSwing(string encodedPhoto)
        {
            Debug.Log("OnPhoneShaked");
            GameManager.Instance.SetPicture(encodedPhoto);
        }

        private void OnPhoneForwardSwing(string encodedPhoto, int width, int height)
        {
            Debug.Log("OnPhoneShaked");
            GameManager.Instance.SetPicture(encodedPhoto, width, height);
        }


        private void OnLineDrawn(List<Vector3> points)
        {
            Debug.Log("OnLineDrawn");
            GameManager.Instance.SetDrawingLine(points);
        }

        private void OnPhoneAngleChanged(float angle)
        {
            Debug.Log("OnPhoneAngleChanged");
            GameManager.Instance.SetKiteAngle(angle);
            NetworkController.Instance.ipText.text = "angle : " + angle;
        }

        private void OnGoalKeeperPositionUpdated(Vector3 position, Vector3 handPosition)
        {
            GameManager.Instance.UpdateGoalKeeperPosition(position, handPosition);
        }

        private void OnFootballPositionUpdated(Vector3 position)
        {
            //Debug.Log("ball position : " + position);
            GameManager.Instance.UpdateFootballPosition(position);
        }

        private void OnShootTriggered()
        {
            GameManager.Instance.Shoot();
        }

        private void OnStrikerSelected(ulong clientId, bool isSelected)
        {
            FootballController.Instance.OnStrikerSelected(clientId, isSelected);
        }

        private void OnGoalKeeperSelected(ulong clientId, bool isSelected)
        {
            FootballController.Instance.OnGoalKeeperSelected(clientId, isSelected);
        }

        private void OnShootTimerStarted()
        {
            FootballController.Instance.StartShootTimer();
        }

        private void OnShootTimerEnded()
        {
            FootballController.Instance.ForwardShoot();
        }

        private void OnNextRoundStarted()
        {
            NetworkController.Instance.errorMessage = "OnNextRoundStarted";
            FootballController.Instance.NextRound();
        }

        private void OnFootballPlayerDataUpdated(FootballPlayerData data)
        {
            FootballController.Instance.UpdatePlayerData(data);
        }

        private void OnResetMatchStarted()
        {
            NetworkController.Instance.errorMessage = "OnNextRoundStarted";
            FootballController.Instance.ResetMatch();
        }

        private void OnPartOfDayChanged(int part)
        {
            FootballController.Instance.OnPartOfDayChanged(part);
        }

        private void OnWeatherChanged(int weather)
        {
            FootballController.Instance.OnWeatherChanged(weather);
        }

        private void OnOtherPlayerDisconnected(ulong clientId)
        {
            FootballController.Instance.OnDisconnected(clientId);
        }
    }
}
