using System;
using UnityEngine;

public class Options_SubUI : MonoBehaviour
{
    [SerializeField] private GameObject GameplayOptions;
    [SerializeField] private GameObject KeybindOptions;
    [SerializeField] private GameObject SoundOptions;
    [SerializeField] private GameObject LanguageOptions;


    private void Start()
    {
        SwitchToOptions(GameplayOptions);
    }

    public void SwitchToOptions(GameObject _optionsMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_optionsMenu != null)
        {
            _optionsMenu.SetActive(true);
            AudioManager.instance.PlaySFX(7, null);
        }
    }
}
