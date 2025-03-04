using System.Text;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,   // ����
    Equipment   // װ��
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
    public float dropChance;   // ���伸��

    protected StringBuilder sb = new StringBuilder();

    /*
     OnValidate �� ScriptableObject ��һ���������ڷ�������������Դ���κ��ֶη�������ʱ�����á�
        AssetDatabase.GetAssetPath(this) ��ȡ��ǰ ItemData �����·����
        AssetDatabase.AssetPathToGUID(path) ��ȡ��·����Ӧ��ȫ��Ψһ��ʶ����GUID����
    �����丳ֵ�� itemID�����ڱ༭����ȷ��ÿ����Ʒ����Ψһ��ID��
     */

    private void OnValidate()
    {
#if UNITY_EDITOR  
        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);  
#endif
    }


    // ���ص�����Ϣ��Ч������
    public virtual string GetItemStatInfoAndEffectDescription()
    {
        return "";
    }
}
