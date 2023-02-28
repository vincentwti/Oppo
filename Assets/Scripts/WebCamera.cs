using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{
    public RawImage photoImage; //serverSide
    public RawImage capturedPhotoImage; //clientSide
    public RawImage capturedImageResult;
    public MeshRenderer targetMesh;

    private Texture2D cachedPhotoTexture;

    private WebCamTexture webCamTexture;
    private Quaternion deviceRotation;
    private Vector2 photoSize;

    private int pictureWidth;
    private int pictureHeight;

    private void Start()
    {
        EventManager.onUseCameraSelected += Open;
        EventManager.onPhotoCaptured += CaptureCamera;
        photoImage.transform.parent.gameObject.SetActive(false);
        capturedPhotoImage.gameObject.SetActive(false);
        capturedImageResult.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.onPhotoCaptured -= CaptureCamera;
        EventManager.onUseCameraSelected -= Open;
    }

    public void Open()
    {
        try
        {
            if (GameManager.Instance.controlType == GameManager.ControlType.PICTURE)
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
                capturedPhotoImage.gameObject.SetActive(true);
                string frontCameraDeviceName = "";
                foreach (WebCamDevice device in WebCamTexture.devices)
                {
                    if (device.isFrontFacing)
                    {
                        frontCameraDeviceName = device.name;
                        pictureWidth = device.availableResolutions[0].width;
                        pictureHeight = device.availableResolutions[0].height;
                        break;
                    }
                }
                photoSize = new Vector2(Screen.height, Screen.width);
                capturedPhotoImage.rectTransform.sizeDelta = new Vector2(photoSize.x, photoSize.y);
                webCamTexture = new WebCamTexture(frontCameraDeviceName, (int)photoSize.x, (int)photoSize.y, 30);
                capturedPhotoImage.texture = webCamTexture;
                webCamTexture.Play();
            }
        }
        catch (Exception exc)
        {
            WTI.NetCode.NetworkController.Instance.ipText.text = "err : " + exc.Message;
        }
    }


    private void CaptureCamera()
    {
        GameManager.Instance.HideCaptureButton();
        StartCoroutine(WaitCaptureCamera());
    }

    private IEnumerator WaitCaptureCamera()
    {
        yield return new WaitForEndOfFrame();
        cachedPhotoTexture = new Texture2D((int)photoSize.y, (int)photoSize.x, TextureFormat.RGB24, false);
        cachedPhotoTexture.ReadPixels(new Rect(0, 0, photoSize.y, photoSize.x), 0, 0);
        //cachedPhotoTexture.SetPixels(webCamTexture.GetPixels());
        cachedPhotoTexture.Apply();

        capturedImageResult.texture = cachedPhotoTexture;
        capturedImageResult.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        GameManager.Instance.ShowCaptureButton();
    }

    public byte[] GetCapturedPhoto()
    {
        return cachedPhotoTexture.EncodeToPNG();
    }

    public string GetEncodedCapturedPhoto()
    {
        return Convert.ToBase64String(cachedPhotoTexture.EncodeToPNG());
    }

    public Vector2 GetPhotoSize()
    {
        return photoSize;
    }

    public void SetPhoto(byte[] photo, int width, int height)
    {
        GameManager.Instance.ShowCameraTexture();
        Texture2D tex = new Texture2D(width, height);
        tex.LoadImage(photo);
        photoImage.texture = tex;
    }

    public void SetPhoto(byte[] photo)
    {
        GameManager.Instance.ShowCameraTexture();
        Texture2D tex = new Texture2D(500, 500);
        tex.LoadImage(photo);
        photoImage.texture = tex;
    }

    public void SetPhoto(string encodedPhoto, int width, int height)
    {
        GameManager.Instance.ShowCameraTexture();
        Texture2D tex = new Texture2D(width, height);
        tex.LoadImage(Convert.FromBase64String(encodedPhoto));
        photoImage.texture = tex;
        photoImage.rectTransform.sizeDelta = new Vector2(height / 1.8f, width / 1.8f);
    }

    public void SetPhoto(string encodedPhoto)
    {
        GameManager.Instance.ShowCameraTexture();
        Texture2D tex = new Texture2D(500, 500);
        tex.LoadImage(Convert.FromBase64String(encodedPhoto));
        photoImage.texture = tex;
    }
}
