using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [Header("Audio����")]
    public SerializableDictionary<string, float> volumeSettingsDictionary;

    [Header("����������")]
    public SerializableDictionary<string, KeyCode> keybindsDictionary;
    //public SerializableDictionary<string, KeyCode> keybindsDictionary_Chinese; 

    [Header("Gameplay����")]
    public SerializableDictionary<string, bool> gameplayToggleSettingsDictionary;

    [Header("�������� 1���� 0 Ӣ��")]
    public int localeID; //0 for english, 1 for chinese

    public SettingsData()
    {
        localeID = 1;
        // ��ʼ������
        //volumeSettingsDictionary<exposedParameter, value>
        volumeSettingsDictionary = new SerializableDictionary<string, float>();

        keybindsDictionary = new SerializableDictionary<string, KeyCode>();
        SetupDefaultKeybinds();

        gameplayToggleSettingsDictionary = new SerializableDictionary<string, bool>();
    }

    private void SetupDefaultKeybinds()
    {
        keybindsDictionary.Add("Attack", KeyCode.Mouse0);
        keybindsDictionary.Add("Aim", KeyCode.Mouse1);
        keybindsDictionary.Add("Flask", KeyCode.Alpha1);
        keybindsDictionary.Add("Dash", KeyCode.LeftShift);
        keybindsDictionary.Add("Parry", KeyCode.Q);
        keybindsDictionary.Add("Crystal", KeyCode.F);
        keybindsDictionary.Add("Blackhole", KeyCode.R);
        keybindsDictionary.Add("Character", KeyCode.C);
        keybindsDictionary.Add("Craft", KeyCode.B);
        keybindsDictionary.Add("Skill", KeyCode.K);
    }
}
