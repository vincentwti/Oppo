using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class ControlTypeInfo
{
    public string controlName;
    public Sprite controlSprite;
}

public class SelectableControlType : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform itemParent;
    public HighlightedControlType highlightedItem;
    public List<ControlTypeInfo> controlInfoList;

    public TweenFade tweenFade;

    private void Start()
    {
        tweenFade = GetComponent<TweenFade>();
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < controlInfoList.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, itemParent, false);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            item.transform.localEulerAngles = Vector3.zero;

            item.GetComponent<ControlTypeItem>()?.Init(this, controlInfoList[i]);
        }
    }

    public void SelectItem(Transform transform, ControlTypeInfo info)
    {
        highlightedItem.gameObject.SetActive(true);
        highlightedItem.Show(this, transform, info);
        tweenFade.Play();
    }
}
