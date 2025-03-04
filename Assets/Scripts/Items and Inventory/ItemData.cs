using System.Text;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,   // 材料
    Equipment   // 装备
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string itemName_Chinese;
    public Sprite icon;
    public string itemID;

    [Range(0, 100)]
    public float dropChance;   // 掉落几率

    protected StringBuilder sb = new StringBuilder();

    /*
     OnValidate 是 ScriptableObject 的一个生命周期方法，它会在资源的任何字段发生更改时被调用。
        AssetDatabase.GetAssetPath(this) 获取当前 ItemData 对象的路径。
        AssetDatabase.AssetPathToGUID(path) 获取该路径对应的全局唯一标识符（GUID），
    并将其赋值给 itemID。这在编辑器中确保每个物品都有唯一的ID。
     */

    private void OnValidate()
    {
#if UNITY_EDITOR  
        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);  
#endif
    }


    // 返回道具信息和效果描述
    public virtual string GetItemStatInfoAndEffectDescription()
    {
        return "";
    }
}
