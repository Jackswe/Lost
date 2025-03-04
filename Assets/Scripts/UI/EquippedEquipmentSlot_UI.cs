using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedEquipmentSlot_UI : InventorySlot_UI
{
    public EquipmentType equipmentType;

    private void OnValidate()
    {
        gameObject.name = equipmentType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
        if (inventorySlot == null || inventorySlot.item == null)
        {
            return;
        }

        Inventory.instance.UnequipEquipmentWithoutAddingBackToInventory(inventorySlot.item as ItemData_Equipment);
        Inventory.instance.AddItem(inventorySlot.item as ItemData_Equipment);
        CleanUpInventorySlotUI();  

        ui.itemToolTip.HideToolTip();
    }
}
