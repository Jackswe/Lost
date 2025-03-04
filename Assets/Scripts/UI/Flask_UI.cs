using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flask_UI : MonoBehaviour
{
    public static Flask_UI instance;

    public Image flaskImage;
    public Image flaskCooldownImage;

    //private ItemData_Equipment flask;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        SetFlaskImage(Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Flask));
    }

    public void SetFlaskImage(ItemData_Equipment _flask)
    {
        if (_flask == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            flaskImage.sprite = _flask.icon;
            flaskCooldownImage.sprite = _flask.icon;
        }
    }
}
