using System.Collections.Generic;
using UnityEngine;

public class KeybindConflictPromptWindowController : MonoBehaviour
{
    [SerializeField] private GameObject[] keybindConflicts;

    public void SetupKeybindConflictPromptWindow(KeyCode _keyCode)
    {
        List<string> _behaveNames = new List<string>();

        foreach (var search in KeyBindManager.instance.keybindsDictionary)
        {
            if (search.Value == _keyCode)
            {
                _behaveNames.Add(search.Key);
            }
        }


        if (_behaveNames.Count >= 2)
        {
            //KeybindConflictController[] controllers = GetComponentsInChildren<KeybindConflictController>();

            int k = 0;

            for (int i = 0; i < keybindConflicts.Length; i++)
            {
                keybindConflicts[i].GetComponent<KeybindConflictController>()?.SetupKeybindConflict(_behaveNames[k], _keyCode.ToString());
                k++;

                if (k == 2)
                {
                    return;
                }
            }

        }
    }

    public void ClosePromptWindow()
    {
        Destroy(gameObject);
    }
}
