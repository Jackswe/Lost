using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int maxItemDropAmount;  // 最大掉落数量
    [SerializeField] private ItemData[] possibleDropItemList;  // 可能掉落的物品列表
    private List<ItemData> actualDropList = new List<ItemData>(); //    这个敌人实际掉落的物品
    //一个空的预制体，可以通过ItemObject中的SetupItemDrop（）将其设置为掉落的物品
    [SerializeField] private GameObject dropItemPrefab; 


    public virtual void GenrateDrop()
    {
        for (int i = 0; i < possibleDropItemList.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDropItemList[i].dropChance)
            {
                actualDropList.Add(possibleDropItemList[i]);
            }
        }
        //  显示物品最大掉落数量
        for (int i = 0; i < maxItemDropAmount && actualDropList.Count > 0; i++)
        {
            ItemData itemToDrop = actualDropList[Random.Range(0, actualDropList.Count - 1)];

            actualDropList.Remove(itemToDrop);
            DropItem(itemToDrop);
        }

    }


    protected void DropItem(ItemData _itemToDrop)
    {
        GameObject newDropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);

        Vector2 dropVelocity = new Vector2(Random.Range(-5, 5), Random.Range(12, 15));

        newDropItem.GetComponent<ItemObject>()?.SetupItemDrop(_itemToDrop, dropVelocity);
    }
}
