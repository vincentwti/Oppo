using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ControlTypeItem : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemText;

    private Button itemButton;
    private ControlTypeInfo info;

    private SelectableControlType controller;

    private void Awake()
    {
        itemButton = itemImage.GetComponent<Button>();
        itemButton.onClick.AddListener(OnItemClicked);
    }

    public void Init(SelectableControlType controller, ControlTypeInfo info)
    {
        this.controller = controller;
        this.info = info;
        itemImage.sprite = info.controlSprite;
        itemImage.SetNativeSize();
        itemText.text = info.controlName;
    }

    public void OnItemClicked()
    {
        controller.SelectItem(transform, info);
    }
}
