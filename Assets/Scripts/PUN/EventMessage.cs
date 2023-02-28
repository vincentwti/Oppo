using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace WTI.PUN
{
    public class EventMessage : MonoBehaviour, IOnEventCallback
    {
        private const int shakeEventCode = 1;
        private const int drawLineEventCode = 2;
        private const int phoneAngleEventCode = 3;

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            EventManager.onDrawingLine += (senderClientId, positionList) => SendLineEvent(positionList);
            EventManager.onPhoneSideShaked += (senderClientId) => SendShakeEvent();
            EventManager.onPhoneTilted += (senderClinetId, angle) => SendPhoneAngleEvent(angle);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void SendShakeEvent()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.RaiseEvent(shakeEventCode, null, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendUnreliable);
                //PlayParticle();
            }
        }

        public void SendLineEvent(List<Vector3> vectorPoints)
        {
            object[] points = new object[vectorPoints.Count];
            for (int i = 0; i < vectorPoints.Count; i++)
            {
                points[i] = vectorPoints[i];
            }
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.RaiseEvent(drawLineEventCode, points, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendUnreliable);
            }
        }

        public void SendPhoneAngleEvent(float angle)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.RaiseEvent(phoneAngleEventCode, angle, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendUnreliable);
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            Debug.Log("get event");
            switch (photonEvent.Code)
            {
                case shakeEventCode:
                    GameManager.Instance.PlayParticle();
                    break;
                case drawLineEventCode:
                    List<Vector3> points = new List<Vector3>();
                    object[] positionObjects = (object[])photonEvent.CustomData;
                    for (int i = 0; i < positionObjects.Length; i++)
                    {
                        points.Add((Vector3)positionObjects[i]);
                    }
                    GameManager.Instance.SetDrawingLine(points);
                    break;
                case phoneAngleEventCode:
                    GameManager.Instance.SetKiteAngle((float)photonEvent.CustomData);
                    break;

            }
        }
    }
}
