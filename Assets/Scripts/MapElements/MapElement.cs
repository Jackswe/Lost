using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapElement : MonoBehaviour
{
    public bool isMapElementThatCannotReuse;
    public int mapElementID;

    protected virtual void Start()
    {
        DestroySelfIfHasBeenUsed();
    }

    private void DestroySelfIfHasBeenUsed()
    {
        if (isMapElementThatCannotReuse)
        {
            foreach (var id in GameManager.instance.UsedMapElementIDList)
            {
                if (mapElementID == id)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
