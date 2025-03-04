using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int maxItemDropAmount;  // ����������
    [SerializeField] private ItemData[] possibleDropItemList;  // ���ܵ������Ʒ�б�
    private List<ItemData> actualDropList = new List<ItemData>(); //    �������ʵ�ʵ������Ʒ
    //һ���յ�Ԥ���壬����ͨ��ItemObject�е�SetupItemDrop������������Ϊ�������Ʒ
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
        //  ��ʾ��Ʒ����������
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
