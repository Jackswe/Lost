using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_UI : MonoBehaviour, ISettingsSaveManager
{
    public static MainMenu_UI instance;

    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private FadeScreen_UI fadeScreen;

    [Header("新游戏")]
    [SerializeField] private GameObject newGameConfirmWindow;

    [Header("设置")]
    [SerializeField] private GameObject optionsUI;

    [Header("退出确认窗口")]
    [SerializeField] private GameObject exitConfirmWindow;

    [Header("音效设置")]
    [SerializeField] private VolumeSlider_UI[] volumeSettings;

    [Header("GamePlay设置窗口")]
    [SerializeField] private GameplayOptionToggle_UI[] gameplayToggleSettings;

    private bool UIKeyFunctioning = true;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!SaveManager.instance.HasGameSaveData())
        {
            continueButton.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        //SceneManager.LoadScene(sceneName);
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void ShowNewGameConfirmWindow()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            newGameConfirmWindow.SetActive(true);
        }
    }

    public void CloseAllConfirmWindow()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            newGameConfirmWindow.SetActive(false);
            exitConfirmWindow.SetActive(false);
            optionsUI.SetActive(false);
        }
    }

    public void NewGame_DetectSaveFile()
    {
        if (SaveManager.instance.HasGameSaveData())
        {
            ShowNewGameConfirmWindow();
        }
        else
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteGameProgressionSavedData();
        //SceneManager.LoadScene(sceneName);
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void Exit()
    {
        Debug.Log("Game exited");
        Application.Quit();
    }

    private IEnumerator LoadSceneWithFadeEffect(float _delayTime)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delayTime);

        SceneManager.LoadScene(sceneName);
    }

    public void SwitchToOptionsUI()
    {
        if (optionsUI != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            optionsUI.SetActive(true);
        }
    }

    public void ShowExitConfirmWindow()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            exitConfirmWindow.SetActive(true);
        }
    }


    public void EnableKeyInput(bool _value)
    {
        UIKeyFunctioning = _value;
    }

    public void LoadData(SettingsData _data)
    {
        Debug.Log("Loading option data");
        foreach (var search in _data.volumeSettingsDictionary)
        {
            foreach (var volume in volumeSettings)
            {
                if (volume.parameter == search.Key)
                {
                    volume.LoadVolumeSlider(search.Value);
                }
            }
        }

        foreach (var search in _data.gameplayToggleSettingsDictionary)
        {
            foreach (var toggle in gameplayToggleSettings)
            {
                if (toggle.optionName == search.Key)
                {
                    toggle.SetToggleValue(search.Value);
                }
            }
        }
    }

    public void SaveData(ref SettingsData _data)
    {
        Debug.Log("Saving option data");
        _data.volumeSettingsDictionary.Clear();

        foreach (var volume in volumeSettings)
        {
            _data.volumeSettingsDictionary.Add(volume.parameter, volume.slider.value);
        }

        _data.gameplayToggleSettingsDictionary.Clear();

        foreach (var toggle in gameplayToggleSettings)
        {
            _data.gameplayToggleSettingsDictionary.Add(toggle.optionName, toggle.GetToggleValue());
        }
    }
}
