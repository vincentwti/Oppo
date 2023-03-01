using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System;
using Unity.Netcode.Transports.UTP;

namespace WTI.NetCode
{

    /// <summary>
    /// FOR NETCODE
    /// </summary>
    public class NetworkController : MonoBehaviour
    {
        public TMPro.TMP_Text statusText;
        public TMPro.TMP_Text ipText;
        public NetworkTransport transport;

        public string errorMessage;

        public static NetworkController Instance { get; private set; }

        public ulong GetClientId()
        {
            return NetworkManager.Singleton.LocalClientId;
        }

        private IEnumerator Start()
        {
            yield return null;
            Debug.Log("START");
            Instance = this;
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
            NetworkManager.Singleton.ConnectionApprovalCallback += OnCheckApprovalConnection;
            NetworkManager.Singleton.OnTransportFailure += OnFailed;
            
            EventManager.onGetIsConnected += GetIsConnected;
            EventManager.onUseCameraSelected += OnWebCameraUsed;

            GameManager.Instance.HideCreateRoomButton();
            GameManager.Instance.HideJoinRoomButton();
            GameManager.Instance.HideExitRoomButton();
            GameManager.Instance.HideDeviceCount();
            GameManager.Instance.HideRoomName();

            yield return null;
            
            EventManager.onConnectToNetwork += Connect;
            EventManager.onLeaveRoom += ExitRoom;

            Connect();
            //if (GameManager.Instance.IsServer)
            //{
            //    //Connect();
            //    GameManager.Instance.HideControlTypeDropdown();
            //}
            //else
            //{ 
            //    GameManager.Instance.ShowControlTypeDropdown();
            //}
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnected;
                NetworkManager.Singleton.ConnectionApprovalCallback -= OnCheckApprovalConnection;
                NetworkManager.Singleton.OnTransportFailure -= OnFailed;
            }
            EventManager.onConnectToNetwork -= Connect;
            EventManager.onLeaveRoom -= ExitRoom;
            EventManager.onUseCameraSelected -= OnWebCameraUsed;
        }

        //private void Log(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
        //{
        //    Debug.Log(eventType + " " + clientId);
        //}

        private bool GetIsConnected()
        {
            return NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer;
        }

        private void OnFailed()
        {
            statusText.text = "Failed";
            errorMessage = "Failed";
        }

        public void Connect()
        {
            Debug.Log("Connect");
            try
            {
                if (GameManager.Instance.IsServer)
                //if(true)
                {
                    if (NetworkManager.Singleton.StartServer())
                    {
                        errorMessage = "Server connected";
                        GameManager.Instance.HideCreateRoomButton();
                        GameManager.Instance.HideReconnectButton();
                        EventManager.onNetworkConnected?.Invoke();
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (NetworkManager.Singleton.StartClient())
                    {
                        errorMessage = "client connected";
                        GameManager.Instance.HideJoinRoomButton();
                        GameManager.Instance.HideReconnectButton();
                        GameManager.Instance.CheckControlType();
                        EventManager.onNetworkConnected?.Invoke();
                        statusText.text = "connect button";
                    }
                    else
                    {
                        statusText.text = "failed";
                        errorMessage = "failed";
                    }
                }
            }
            catch
            {
                statusText.text = "error";
            }
        }


        public void ExitRoom()
        {
            Debug.Log("ExitRoom");
            try
            {
                //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId); //this is for kick a client
                NetworkManager.Singleton.Shutdown(true);

                //PhotonNetwork.LeaveRoom(PhotonNetwork.IsMasterClient);
                if (GameManager.Instance.IsServer)
                {
                    GameManager.Instance.ShowReconnectButton();
                    GameManager.Instance.HideControlTypeDropdown();
                }
                else
                {
                    GameManager.Instance.ShowControlTypeDropdown();
                }
                GameManager.Instance.HideDeviceCount();
                GameManager.Instance.HideRoomName();
                GameManager.Instance.HideExitRoomButton();
                GameManager.Instance.ShowReconnectButton();
                statusText.text = "exited room";
            }
            catch (Exception exc)
            {
                statusText.text = exc.Message;
            }
        }

        #region NetCode

        private void OnCheckApprovalConnection(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse resp)
        {
            resp.Approved = (NetworkManager.Singleton.ConnectedClientsIds.Count < GameManager.Instance.maxUser + 1);
            statusText.text = "response : " + resp.Approved;
        }

        public void OnServerStarted()
        {
            GameManager.Instance.HideReconnectButton();
            //GameManager.Instance.ShowExitRoomButton();
            statusText.text = "started";
        }

        public void OnConnected(ulong clientId)
        {
            Debug.LogWarning("OnConnected : " + clientId + "  " + NetworkManager.Singleton.ConnectedClients.Count);
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                GameManager.Instance.HideReconnectButton();
                //GameManager.Instance.ShowExitRoomButton();
                statusText.text = "connected";

                OnDevicePaired();
                FootballController.Instance.CheckState();
            }
            else if (GameManager.Instance.IsServer)
            {
                FootballController.Instance.SendPlayerData();
                if (NetworkManager.Singleton.ConnectedClients.Count > 2 && !FootballController.Instance.isStarted)
                {
                    FootballController.Instance.StartMatch();
                }
            }
        }

        public void OnDisconnected(ulong clientId)
        {
            Debug.Log("Disconnect : " + clientId);
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                statusText.text = "disconnected";

                //GameManager.Instance.ShowReconnectButton();
                GameManager.Instance.HideCreateRoomButton();
                GameManager.Instance.HideJoinRoomButton();
                GameManager.Instance.HideExitRoomButton();
                GameManager.Instance.HideDeviceCount();
                GameManager.Instance.HideRoomName();

            }
            if (NetworkManager.Singleton.IsServer)
            {
                FootballController.Instance.OnDisconnected(clientId);
                EventManager.onOtherPlayerDisconnected?.Invoke(GameManager.Instance.GetClientId(), clientId);
            }

            Connect();
        }

        public void OnApplicationFocus(bool focus)
        {
            if (NetworkManager.Singleton.IsConnectedClient)
            {
                Connect();
            }
        }

        public void OnJoinedRoom()
        {
            GameManager.Instance.ShowExitRoomButton();
        }

        public void OnWebCameraUsed()
        {
            ipText.text = "WebCam open";
            GameManager.Instance.ShowCaptureButton();
            //GameManager.Instance.ShowCameraTexture();
        }

        #endregion

        private void OnDevicePaired()
        {
            GameManager.Instance.HideControlTypeDropdown();
            GameManager.Instance.HideDeviceCount();
            GameManager.Instance.HideRoomName();
        }

        //private void OnGUI()
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.fontSize = 50;
        //    style.normal.textColor = Color.red;

        //    GUI.Label(new Rect(40, 100, 300, 60), "IP : " + ((UnityTransport)transport).ConnectionData.Address.ToString()+ ":" + ((UnityTransport)transport).ConnectionData.Port + " " + ((UnityTransport)transport).ConnectionData.ServerListenAddress.ToString(), style);
        //    GUI.Label(new Rect(40, 160, 300, 60), "msg : " + errorMessage, style);

        //}
    }
}
