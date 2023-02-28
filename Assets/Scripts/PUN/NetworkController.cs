using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace WTI.PUN
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        public static NetworkController Instance { get; private set; }

        public int GetClientId()
        {
            return PhotonNetwork.LocalPlayer.ActorNumber;   
        }

        private void Start()
        {
            Instance = this;
            Connect();
            GameManager.Instance.HideCreateRoomButton();
            GameManager.Instance.HideJoinRoomButton();
            GameManager.Instance.HideExitRoomButton();
            GameManager.Instance.HideDeviceCount();
            GameManager.Instance.HideRoomName();

            EventManager.onGetIsConnected += GetIsConnected;
            EventManager.onConnectToNetwork += Connect;
            EventManager.onCreateRoom += CreateRoom;
            EventManager.onJoinRoom += JoinRoom;
            EventManager.onLeaveRoom += ExitRoom;
        }

        private void OnDestroy()
        {
            EventManager.onConnectToNetwork -= Connect;
            EventManager.onCreateRoom -= CreateRoom;
            EventManager.onJoinRoom -= JoinRoom;
            EventManager.onLeaveRoom -= ExitRoom;
        }

        private bool GetIsConnected()
        {
            return PhotonNetwork.IsConnected;
        }

        public void Connect()
        {
            PhotonNetwork.ConnectUsingSettings();
            GameManager.Instance.HideReconnectButton();
        }

        public void CreateRoom()
        {
            int rand = Random.Range(1000, 10000);
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
            PhotonNetwork.CreateRoom(rand.ToString(), roomOptions);
            GameManager.Instance.SetRoomName(rand.ToString());
            GameManager.Instance.HideCreateRoomButton();
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
            GameManager.Instance.HideJoinRoomButton();
        }


        public void ExitRoom()
        {
            PhotonNetwork.LeaveRoom(PhotonNetwork.IsMasterClient);
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                GameManager.Instance.ShowJoinRoomButton();
            }
            else
            {
                GameManager.Instance.ShowCreateRoomButton();
            }
            GameManager.Instance.HideDeviceCount();
            GameManager.Instance.HideRoomName();
            GameManager.Instance.HideExitRoomButton();
        }

        #region Photon

        public override void OnConnected()
        {
            GameManager.Instance.HideReconnectButton();

            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                GameManager.Instance.ShowCreateRoomButton();
            }
            else
            {
                GameManager.Instance.ShowJoinRoomButton();
            }
        }

        public override void OnCreatedRoom()
        {
            GameManager.Instance.ShowDeviceCount();
            GameManager.Instance.ShowRoomName();
            GameManager.Instance.SetDeviceCount(0);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            if (!PhotonNetwork.IsConnected)
            {
                GameManager.Instance.ShowReconnectButton();
            }
            else
            {
                GameManager.Instance.ShowCreateRoomButton();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            GameManager.Instance.ShowReconnectButton();

            GameManager.Instance.HideCreateRoomButton();
            GameManager.Instance.HideJoinRoomButton();
            GameManager.Instance.HideExitRoomButton();
            GameManager.Instance.HideDeviceCount();
            GameManager.Instance.HideRoomName();
        }

        public override void OnJoinedLobby()
        {
            if (!PhotonNetwork.IsConnected)
            {
                GameManager.Instance.ShowReconnectButton();
            }

            else
            {
                GameManager.Instance.HideJoinRoomButton();
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            GameManager.Instance.ShowExitRoomButton();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (!PhotonNetwork.IsConnected)
            {
                GameManager.Instance.ShowReconnectButton();
            }

            else
            {
                GameManager.Instance.ShowJoinRoomButton();
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("player joined : " + newPlayer.ActorNumber);
            GameManager.Instance.SetDeviceCount(PhotonNetwork.CountOfPlayers - 1);
            if (PhotonNetwork.CountOfPlayers == 2)
            {
                OnDevicePaired();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            GameManager.Instance.SetDeviceCount(PhotonNetwork.CountOfPlayers - 1);
            if (otherPlayer.IsMasterClient)
            {
                ExitRoom();
            }
        }

        #endregion

        private void OnDevicePaired()
        {
            GameManager.Instance.HideDeviceCount();
            GameManager.Instance.HideRoomName();

        }
    }
}
