using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// 制作窗口
public class CraftWindow_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemStatInfo;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Transform referenceRequiredMaterialList;
    private Image[] materialImage;

    private ItemData_Equipment equipmentToCraft;

    private void Awake()
    {
        materialImage = new Image[referenceRequiredMaterialList.childCount];

        for (int i = 0; i < referenceRequiredMaterialList.childCount; i++)
        {
            materialImage[i] = referenceRequiredMaterialList.GetChild(i).GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (equipmentToCraft != null)
        {
            UpdateItemTextLanguage(equipmentToCraft);
        }
    }

    public void SetupCraftWindow(ItemData_Equipment _itemToCraft)
    {
        equipmentToCraft = _itemToCraft;

        craftButton.onClick.RemoveAllListeners();

        if (_itemToCraft.requiredCraftMaterials.Count > materialImage.Length)
        {
            Debug.Log("所需材料槽不够");  // TODO
        }

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0; i < _itemToCraft.requiredCraftMaterials.Count; i++)
        {
            materialImage[i].sprite = _itemToCraft.requiredCraftMaterials[i].item.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI requiredMaterialText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            requiredMaterialText.text = _itemToCraft.requiredCraftMaterials[i].stackSize.ToString();
            requiredMaterialText.color = Color.white;
        }

        itemIcon.sprite = _itemToCraft.icon;
        itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();

        UpdateItemTextLanguage(_itemToCraft);

        craftButton.onClick.AddListener(() => Inventory.instance.CraftIfAvailable(_itemToCraft, _itemToCraft.requiredCraftMaterials));
    }

    private void UpdateItemTextLanguage(ItemData_Equipment _itemToCraft)
    {
        //english
        if (LanguageManager.instance.localeID == 0)
        {
            itemName.text = _itemToCraft.itemName;
            itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();
        }
        //chinese
        else if (LanguageManager.instance.localeID == 1)
        {
            itemName.text = _itemToCraft.itemName_Chinese;
            itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();
            itemStatInfo.text = LanguageManager.instance.TranslateItemStatInfoFromEnglishToChinese(itemStatInfo.text);
        }
    }
}
