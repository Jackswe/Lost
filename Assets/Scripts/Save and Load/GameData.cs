using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

[System.Serializable]
public class GameData
{
    // 存档数据
    public int currecny;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equippedEquipmentIDs;

    public SerializableDictionary<string, bool> checkpointsDictionary;
    public string closestActivatedCheckpointID;
    public string lastActivatedCheckpointID;

    [Header("金币掉落")]
    public int droppedCurrencyAmount;
    public Vector2 deathPosition;

    [Header("地图元素信息")]
    public List<int> UsedMapElementIDList;

    public GameData()
    {
        // 基本数据
        this.currecny = 0;
        this.droppedCurrencyAmount = 0;
        this.deathPosition = Vector2.zero;

        // 设置在地图上的拾取的物品列表
        UsedMapElementIDList = new List<int>();
        //pickedUpItemInMapList = new List<ItemObject>();

        //skillTree<skillName, unlocked>
        skillTree = new SerializableDictionary<string, bool>();
        //inventory<itemID, stackSize>
        inventory = new SerializableDictionary<string, int>();

        equippedEquipmentIDs = new List<string>();

        //checkpoints<checkpointID, activated>
        checkpointsDictionary = new SerializableDictionary<string, bool>();

        closestActivatedCheckpointID = string.Empty;
        lastActivatedCheckpointID = string.Empty;
    }
}
