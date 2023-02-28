using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class InfinityScroll : ScrollRect
{
    public List<GameObject> itemList;

    public enum ScrollDirection
    {
        HORIZONTAL,
        VERTICAL
    }
    public ScrollDirection scrollDirection;

    private ScrollRect scrollRect;
    private List<GameObject> originalItemList;

    protected override void Start()  
    {
        base.Start();
        //new reference copy
        originalItemList = new List<GameObject>(itemList);

        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnItemScrolled);
        //InstantiateLastItem();
        //InstantiateFirstItem();
        if (scrollDirection == ScrollDirection.HORIZONTAL)
        {
            if (((RectTransform)transform.root).sizeDelta.x > GetItemSize().x)
            {
                CloneAllList();
            }
        }
        else
        {
            if (((RectTransform)transform.root).sizeDelta.y > GetItemSize().y)
            {
                CloneAllList();
            }
        }
    }

    private Vector2 GetItemSize()
    {
        if (itemList.Count > 0)
        {
            try
            {
                RectTransform rectTransform = (RectTransform)itemList[0].transform;
                return rectTransform.sizeDelta;
            }
            catch (Exception exc)
            {
                Debug.LogError("err : " + exc.Message);
            }
        }
        return Vector2.zero;
    }

    private void InstantiateFirstItem()
    {
        if (itemList.Count > 0)
        {
            GameObject item = Instantiate(itemList[0], itemList[0].transform.parent);
            itemList.Add(item);
        }
    }

    private void InstantiateLastItem()
    {
        if (itemList.Count > 0)
        {
            GameObject item = Instantiate(itemList[itemList.Count - 1], itemList[itemList.Count - 1].transform.parent);
            itemList.Add(item);
            item.transform.SetAsFirstSibling();
        }
    }

    private void CloneAllList()
    {
        List<GameObject> newItemList = new List<GameObject>();
        for (int i = 0; i < itemList.Count; i++)
        {
            GameObject item = Instantiate(itemList[i], itemList[i].transform.parent);
            newItemList.Add(item);
        }
        itemList.AddRange(newItemList);
    }

    private void OnItemScrolled(Vector2 position)
    {
        
    }
}
