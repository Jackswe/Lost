using UnityEngine;


/// <summary>
/// 处理物品捡起 掉落等
/// </summary>
public class ItemObject : MapElement
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData item;

    protected override void Start()
    {
        SetupItemIconAndName();
        base.Start();
    }

    public void SetupItemDrop(ItemData _item, Vector2 _dropVelocity)
    {
        item = _item;
        rb.velocity = _dropVelocity;

        SetupItemIconAndName();
    }

    public void PickupItem()
    {
        if (!Inventory.instance.CanAddEquipmentToInventory() && item.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 5);

            //english
            if (LanguageManager.instance.localeID == 0)
            {
                PlayerManager.instance.player.fx.CreatePopUpText("No more space in inventory!");
            }
            //chinese
            else if (LanguageManager.instance.localeID == 1)
            {
                PlayerManager.instance.player.fx.CreatePopUpText("背包空间不足!");
            }
            return;
        }

        Inventory.instance.AddItem(item);
        AudioManager.instance.PlaySFX(18, transform);

        GameManager.instance.UsedMapElementIDList.Add(mapElementID);

        Destroy(gameObject);
    }

    private void SetupItemIconAndName()
    {
        if (item == null)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = item.icon;
        gameObject.name = $"Item Object - {item.name}";
    }
}
