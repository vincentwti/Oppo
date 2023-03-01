using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public int maxUser = 3;

    public ParticleSystem particle;
    public LineBrush lineBrush;
    public KiteController[] kites;
    public WebCamera webCamera;
    public FootballController footballController;
    public SelectableControlType controlTypeController;

    public HUDController hud;
    public Color serverColor = Color.blue;
    public Color clientColor = Color.red;
    public UnityEvent onServer;
    public UnityEvent onClient;

    public int curDebugClickCount;
    private int debugClick = 5;

    public float elapsedTime = 0;
    public float resetTime = 0.5f;

    public bool IsServer { get; private set; }
    public bool IsConnected { get; private set; }

    public enum ControlType
    {
        KITE = 0,
        SHAKEDRAW = 1,
        DRAW = 2,
        PICTURE = 3
    }
    public ControlType controlType;

    public static GameManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        
        
        //controlType = ControlType.SHAKEDRAW;
        
        
        Settings.networkType = Settings.NetworkType.NetCode;

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            IsServer = true;
        }

        //if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        //{
        //    IsServer = true;
        //}
        EventManager.onControlTypeSelected += OnControlTypeSelected;
        //EventManager.onConnectToNetwork += CheckControlType;
        //EventManager.onLeaveRoom += ShowMainMenu;

        HideCaptureButton();
        HideCameraTexture();
        HideClearLineButton();

    }

    private void OnDestroy()
    {
        EventManager.onControlTypeSelected -= OnControlTypeSelected;
        //EventManager.onConnectToNetwork -= CheckControlType;
        EventManager.onLeaveRoom -= ShowMainMenu;
    }

    private void ShowMainMenu()
    {
        if (!IsServer)
        {
            controlTypeController.gameObject.SetActive(true);
            controlTypeController.tweenFade.PlayBackward(null);
            HideCameraTexture();
        }
    }

    public void CheckControlType()
    {
        //NetworkController.Instance.ipText.text = "Control Type : " + controlType.ToString();
        if (controlType == ControlType.PICTURE)
        {
            EventManager.onUseCameraSelected?.Invoke();
        }
        else
        {
            HideCameraTexture();
            HideCaptureButton();
        }
    }

    //private void Update()
    //{
    //    if (curDebugClickCount > 0)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        if (elapsedTime >= resetTime)
    //        {
    //            elapsedTime = 0;
    //            curDebugClickCount = 0;
    //        }
    //    }

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        curDebugClickCount += 1;
    //        if (curDebugClickCount >= debugClick)
    //        {
    //            EventManager.onLeaveRoom?.Invoke();
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("space pressed");
    //        EventManager.sendNetworkMessage?.Invoke(JsonUtility.ToJson(new EventMessageData { methodName = "OnPhoneShaked", parameters = null }));
    //    }
    //}

    //private void OnDebugCountAdded()
    //{
    //    curDebugClickCount += 1;
    //    if (curDebugClickCount >= debugClick)
    //    {
    //        curDebugClickCount = 0;
    //        if (IsServer)
    //        {
    //            IsServer = false;
    //            onClient?.Invoke();
    //            ShowControlTypeDropdown();
    //        }
    //        else
    //        {
    //            IsServer = true;
    //            onServer?.Invoke();
    //            HideControlTypeDropdown();
    //        }
    //    }
    //}

    //public void ChangeServerDebugColor(UnityEngine.UI.Image image)
    //{
    //    image.color = serverColor;
    //    Debug.Log("change color : " + serverColor);
    //}

    //public void ChangeClientDebugColor(UnityEngine.UI.Image image)
    //{
    //    image.color = clientColor;
    //    Debug.Log("change color : " + clientColor);
    //}

    private void OnControlTypeSelected(int index)
    {
        controlType = (ControlType)index;
    }

    public void PlayParticle()
    {
        particle.Play();
    }

    public void ShowReconnectButton()
    {
        hud.reconnectButton.gameObject.SetActive(true);
    }

    public void HideReconnectButton()
    {
        hud.reconnectButton.gameObject.SetActive(false);
    }

    public void ShowCreateRoomButton()
    {
        hud.createRoomButton.gameObject.SetActive(true);
    }

    public void HideCreateRoomButton()
    {
        hud.createRoomButton.gameObject.SetActive(false);
    }

    public void ShowJoinRoomButton()
    {
        hud.joinRoomButton.gameObject.SetActive(true);
        hud.roomNameInputField.gameObject.SetActive(true);
    }

    public void HideJoinRoomButton()
    {
        hud.joinRoomButton.gameObject.SetActive(false);
        hud.roomNameInputField.gameObject.SetActive(false);
    }

    public void ShowExitRoomButton()
    {
        hud.exitRoomButton.gameObject.SetActive(true);
    }

    public void HideExitRoomButton()
    {
        hud.exitRoomButton.gameObject.SetActive(false);
    }

    public void ShowDeviceCount()
    {
        hud.connectedDeviceCountText.gameObject.SetActive(true);
    }

    public void HideDeviceCount()
    {
        hud.connectedDeviceCountText.gameObject.SetActive(false);
    }

    public void SetDeviceCount(int count)
    {
        hud.connectedDeviceCountText.text = "Connected Device : " + count.ToString();
    }

    public void ShowRoomName()
    {
        hud.roomNameText.gameObject.SetActive(true);
    }

    public void HideRoomName()
    {
        hud.roomNameText.gameObject.SetActive(false);
    }

    public void ShowControlTypeDropdown()
    {
        //hud.controlTypeDropdown.gameObject.SetActive(true);
    }

    public void HideControlTypeDropdown()
    {
        //hud.controlTypeDropdown.gameObject.SetActive(false);
    }

    public void ShowCaptureButton()
    {
        hud.captureCameraButton.gameObject.SetActive(true);
    }

    public void ShowCameraTexture()
    {
        webCamera.photoImage.transform.parent.gameObject.SetActive(true);
    }

    public void HideCameraTexture()
    {
        webCamera.photoImage.transform.parent.gameObject.SetActive(false);
        webCamera.capturedPhotoImage.gameObject.SetActive(false);
    }

    public void HideCaptureButton()
    {
        hud.captureCameraButton.gameObject.SetActive(false);
    }

    public void ShowClearLineButton()
    {
        hud.clearLineButton.gameObject.SetActive(true);    
    }

    public void HideClearLineButton()
    {
        hud.clearLineButton.gameObject.SetActive(false);
    }

    public void SetRoomName(string roomName)
    {
        hud.roomNameText.text = roomName;
    }

    public void SetDrawingLine(List<Vector3> points)
    {
        lineBrush.SetLine(points);
    }

    public void SetKiteAngle(float angle)
    {
        foreach (KiteController kite in kites)
        {
            kite.SetAngle(angle);
        }
    }

    public void SetPicture(byte[] photo)
    {
        webCamera.SetPhoto(photo);
    }

    public void SetPicture(byte[] photo, int width, int height)
    {
        webCamera.SetPhoto(photo, width, height);
    }

    public void SetPicture(string encodedPhoto)
    {
        webCamera.SetPhoto(encodedPhoto);
    }

    public void SetPicture(string encodedPhoto, int width, int height)
    {
        webCamera.SetPhoto(encodedPhoto, width, height);
    }

    public void UpdateGoalKeeperPosition(Vector3 position, Vector3 handPosition)
    {
        footballController.UpdateGoalKeeperPosition(position, handPosition);
    }

    public void UpdateFootballPosition(Vector3 position)
    {
        footballController.UpdateFootballPosition(position);
    }

    public void Shoot()
    {
        footballController.Shoot();
    }

    public ulong GetClientId()
    {
        if (Settings.networkType == Settings.NetworkType.NetCode)
        {
            return WTI.NetCode.NetworkController.Instance == null ? 0 : WTI.NetCode.NetworkController.Instance.GetClientId();
        }
        else
        {
            return WTI.PUN.NetworkController.Instance == null ? 0 : (ulong)WTI.PUN.NetworkController.Instance.GetClientId();
        }
    }
}
