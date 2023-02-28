using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupMessageController : MonoBehaviour
{
    public Transform popupMessageParent; 
    public GameObject popupPrefab;
    public static PopupMessageController Instance { get; private set; }

    private Dictionary<string, GameObject> popupMessageDict = new Dictionary<string, GameObject>();

    private void Start()
    {
        Instance = this;
    }

    public void ShowMessage(string id, string title = "", string content = "", Action onButtonClicked = null)
    {
        if (popupMessageDict.ContainsKey(id))
        {
            return;
        }
        GameObject popup = Instantiate(popupPrefab);
        popup.transform.SetParent(popupMessageParent, false);
        popup.transform.localScale = Vector3.one;
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localEulerAngles = Vector3.zero;

        PopupMessage popupMessage = popup.GetComponent<PopupMessage>();
        popupMessage?.ShoweMessage(title, content, onButtonClicked);
        popupMessageDict.Add(id, popup);
    }

    public void ShowConfirmationMessage(string id, string title = "", string content = "", Action onAcceptButtonClicked = null, Action onRejectButtonClicked = null)
    {
        if (popupMessageDict.ContainsKey(id))
        {
            return;
        }
        GameObject popup = Instantiate(popupPrefab);
        popup.transform.SetParent(popupMessageParent, false);
        popup.transform.localScale = Vector3.one;
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localEulerAngles = Vector3.zero;

        PopupMessage popupMessage = popup.GetComponent<PopupMessage>();
        popupMessage?.ShowConfirmationMessage(title, content, onAcceptButtonClicked, onRejectButtonClicked);
        popupMessageDict.Add(id, popup);
    }

    public void HideMessage(string id)
    {
        if (popupMessageDict.ContainsKey(id))
        {
            Destroy(popupMessageDict[id]);
        }
    }
}
