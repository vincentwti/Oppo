using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class ImageData
{
    public string img;
}

[Serializable]
public class ImageTestData
{
    public string image;
}

public class Test : MonoBehaviour
{
    private const string uploadPhotoUrl = "https://dickyri.net/flserver/serve_file.php";
    private const string anotherUploadUrl = "http://185.201.8.46/upload";
    private const string url = "http://185.201.8.46:3000/v1/faceShape";
    public Texture tex;
    public Texture2D tex2D;

    public RawImage rawImage;

    [TextArea(1, 20)]
    public string encodedResult = "";

    private IEnumerator Start()
    {
        yield return UploadImageToServer();
    }



    private IEnumerator UploadImageToServer()
    {
        tex2D = (Texture2D)tex;
        yield return null;
        byte[] bytes = tex2D.EncodeToPNG();

        string encoded = Convert.ToBase64String(bytes);
        ImageData data = new ImageData { img = encoded };
        string body = JsonUtility.ToJson(data);
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.SetRequestHeader("content-type", "application/json; charset=UTF-8");
        encodedResult = encoded;
        req.uploadHandler = new UploadHandlerRaw(bodyBytes);
        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.SendWebRequest();
        Debug.Log(req.result);
        
        Debug.Log("status code : " + req.responseCode);
        if (req.result == UnityWebRequest.Result.Success)
        {
            string result = req.downloadHandler.text;
            Debug.Log("result : " + result);
        }
        else
        {
            OnUploadFailed();
        }
    }

    private void OnUploadFailed()
    {
        
    }
}
