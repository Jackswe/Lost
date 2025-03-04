using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGame_UI : MonoBehaviour
{
    public static InGame_UI instance;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image throwSwordImage;
    [SerializeField] private Image blackholeImage;

    //[SerializeField] private Image flaskImage;

    [Header("Souls")]
    [SerializeField] private TextMeshProUGUI currentCurrency;
    [SerializeField] private float currencyAmount;
    [SerializeField] private float increaseRate = 100;
    [SerializeField] private float defaultcurrencyFontSize;
    [SerializeField] private float currencyFontSizeWhenIncreasing;

    private SkillManager skill;
    private Player player;

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
        if (playerStats != null)
        {
            playerStats.onHealthChanged += UpdateHPUI;
        }

        skill = SkillManager.instance;
        player = PlayerManager.instance.player;
        UpdateHPUI();
    }

    private void Update()
    {
        UpdateCurrencyUI();

        if (Input.GetKeyDown(/*KeyCode.LeftShift*/ KeyBindManager.instance.keybindsDictionary["Dash"]) && skill.dash.dashUnlocked)
        {
            SetSkillCooldownImage(dashImage);
        }

        if (Input.GetKeyDown(/*KeyCode.Q*/ KeyBindManager.instance.keybindsDictionary["Parry"]) && skill.parry.parryUnlocked)
        {
            SetSkillCooldownImage(parryImage);
        }


       

        if (Input.GetKeyDown(/*KeyCode.Mouse1*/ KeyBindManager.instance.keybindsDictionary["Aim"]) && skill.sword.throwSwordSkillUnlocked)
        {
            SetSkillCooldownImage(throwSwordImage);
        }

        if (Input.GetKeyDown(/*KeyCode.R*/ KeyBindManager.instance.keybindsDictionary["Blackhole"]) && skill.blackhole.blackholeUnlocked)
        {
            SetSkillCooldownImage(blackholeImage);
        }

       
        FillSkillCooldownImage(dashImage, skill.dash.cooldown);
        FillSkillCooldownImage(parryImage, skill.parry.cooldown);
        FillSkillCooldownImage(crystalImage, skill.crystal.GetCrystalCooldown());
        FillSkillCooldownImage(throwSwordImage, skill.sword.cooldown);
        FillSkillCooldownImage(blackholeImage, skill.blackhole.cooldown);
        FillFlaskCooldownImage();
    }

    private void UpdateCurrencyUI()
    {
        if (currencyAmount < PlayerManager.instance.GetCurrentCurrency())
        {
            IncraseCurrencyFontSize();
            currencyAmount += Time.deltaTime * increaseRate;
        }
        else
        {
            currencyAmount = PlayerManager.instance.GetCurrentCurrency();
            DecreaseCurrencyFontSizeToDefault();
        }


        if (currencyAmount == 0)
        {
            currentCurrency.text = currencyAmount.ToString();
        }
        else
        {
            currentCurrency.text = currencyAmount.ToString("#,#");
        }
    }

    private void IncraseCurrencyFontSize()
    {
        currentCurrency.fontSize = Mathf.Lerp(currentCurrency.fontSize, currencyFontSizeWhenIncreasing, 100 * Time.deltaTime);
    }

    private void DecreaseCurrencyFontSizeToDefault()
    {
        currentCurrency.fontSize = Mathf.Lerp(currentCurrency.fontSize, defaultcurrencyFontSize, 5 * Time.deltaTime);
    }

    private void UpdateHPUI()
    {
        //total_maxHP = maxHP + vitality * 5
        slider.maxValue = playerStats.getMaxHP();
        slider.value = playerStats.currentHP;
    }

    private void SetSkillCooldownImage(Image _skillImage)
    {
        if (_skillImage.fillAmount <= 0)
        {
            _skillImage.fillAmount = 1;
        }
    }

    public void SetCrystalCooldownImage()
    {
        SetSkillCooldownImage(crystalImage);
    }

    private void FillSkillCooldownImage(Image _skillImage, float _cooldown)
    {
        if (_skillImage == null)
        {
            return;
        }

        if (_skillImage.fillAmount > 0)
        {
            _skillImage.fillAmount -= (1 / _cooldown) * Time.deltaTime;
        }
    }

    private void FillFlaskCooldownImage()
    {
        Image _flaskImage = Flask_UI.instance.flaskCooldownImage;

        if (_flaskImage == null)
        {
            return;
        }

        ItemData_Equipment flask = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Flask);

        if (flask == null)
        {
            return;
        }

        if (!flask.itemEffects[0].effectUsed)
        {
            _flaskImage.fillAmount = 0;
            return;
        }

        if (_flaskImage.fillAmount >= 0)
        {
            float timer = Time.time - flask.itemEffects[0].effectLastUseTime;
            _flaskImage.fillAmount = 1 - ((1 / flask.itemEffects[0].effectCooldown) * timer);
        }
    }
}
