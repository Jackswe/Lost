using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftList_UI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform referenceCraftList;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> equipmentCraftList;

    private void Start()
    {
        transform.parent.GetChild(0)?.GetComponent<CraftList_UI>()?.SetupCraftList();
        transform.parent.GetChild(0)?.GetComponent<CraftList_UI>()?.SetupDefaultCraftWindow();
    }

    public void SetupCraftList()
    {
        for (int i = 0; i < referenceCraftList.childCount; i++)
        {
            Destroy(referenceCraftList.GetChild(i).gameObject);
        }

        for (int i = 0; i < equipmentCraftList.Count; i++)
        {
            GameObject newCraftSlot = Instantiate(craftSlotPrefab, referenceCraftList);
            newCraftSlot.GetComponent<CraftSlot_UI>()?.SetupCraftSlot(equipmentCraftList[i]);
        }
    }

    public void SetupDefaultCraftWindow()
    {
        if (equipmentCraftList[0] != null)
        {
            GetComponentInParent<UI>()?.craftWindow.SetupCraftWindow(equipmentCraftList[0]);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }
}
