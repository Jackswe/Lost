using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{
    [Space]
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalExistenceDuration;
    private GameObject currentCrystal;

    [Header("水晶技能")]
    [SerializeField] private SkillTreeSlot_UI crystalUnlockButton;
    public bool crystalUnlocked { get; private set; }

    [Header("魔法飞弹")]  
    [SerializeField] private SkillTreeSlot_UI mirageBlinkUnlockButton;
    public bool mirageBlinkUnlocked { get; private set; }

    [Header("爆裂水晶")]
    [SerializeField] private SkillTreeSlot_UI explosiveCrystalUnlockButton;
    public bool explosiveCrystalUnlocked { get; private set; }

    [Header("水晶冲击")]
    [SerializeField] private SkillTreeSlot_UI movingCrystalUnlockButton;
    public bool movingCrystalUnlocked { get; private set; }
    [SerializeField] private float moveSpeed;

    [Header("水晶枪")]
    [SerializeField] private SkillTreeSlot_UI crystalGunUnlockButton;
    public bool crystalGunUnlocked { get; private set; }
    [SerializeField] private int magSize;
    [SerializeField] private float shootCooldown; 
    [SerializeField] private float reloadTime;
    [SerializeField] private float shootWindow;
    private float shootWindowTimer;
    [SerializeField] private List<GameObject> crystalMag = new List<GameObject>();
    private bool reloading = false;


    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystal);
        mirageBlinkUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalMirage);
        explosiveCrystalUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockExplosiveCrystal);
        movingCrystalUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMovingCrystal);
        crystalGunUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalGun);
    }

    protected override void Update()
    {
        base.Update();

        shootWindowTimer -= Time.deltaTime;

        if (shootWindowTimer <= 0 && crystalMag.Count > 0 && crystalMag.Count < magSize && !reloading)
        {
            ReloadCrystalMag();
        }
    }

    protected override void CheckUnlockFromSave()
    {
        UnlockCrystal();
        UnlockCrystalGun();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
    }

    public override bool UseSkillIfAvailable()
    {
        if (crystalGunUnlocked)
        {
            UseSkill();
            return true;
        }
        else
        {
            if (cooldownTimer < 0)
            {
                UseSkill();
                return true;
            }

            //english
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");

            }
            //chinese
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("技能冷却中！");
            }

            return false;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (crystalGunUnlocked)
        {
            if (ShootCrystalGunIfAvailable())
            {
                EnterCooldown();
                return;
            }

            //english
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");

            }
            //chinese
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("技能冷却中！");
            }

            return;
        }

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (movingCrystalUnlocked)
            {
                return;
            }

            
            if (mirageBlinkUnlocked)
            {
                SkillManager.instance.clone.CreateClone(player.transform.position);
                //Destroy(currentCrystal);
                currentCrystal.GetComponent<CrystalSkillController>()?.crystalSelfDestroy();
            }

            Vector2 playerPosition = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPosition;

            currentCrystal.GetComponent<CrystalSkillController>()?.EndCrystal_ExplodeIfAvailable();

            EnterCooldown();
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity); ;
        CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();

        currentCrystalScript.SetupCrystal(crystalExistenceDuration, explosiveCrystalUnlocked, movingCrystalUnlocked, moveSpeed, FindClosestEnemy(currentCrystal.transform));
    }

    private void ReloadCrystalMag()
    {
        if (reloading)
        {
            return;
        }

        StartCoroutine(ReloadCrystalMag_Coroutine());
    }

    private IEnumerator ReloadCrystalMag_Coroutine()
    {
        reloading = true;
        EnterCooldown();

        yield return new WaitForSeconds(reloadTime);

        Reload();
        reloading = false;
    }

    private void Reload()
    {
        if (!reloading)
        {
            return;
        }

        int ammoToAdd = magSize - crystalMag.Count;

        for (int i = 0; i < ammoToAdd; i++)
        {
            crystalMag.Add(crystalPrefab);
        }
    }

    private bool ShootCrystalGunIfAvailable()
    {
        if (crystalGunUnlocked && !reloading)
        {
            if (crystalMag.Count > 0)
            {
                GameObject crystalToSpawn = crystalMag[crystalMag.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity); ;

                crystalMag.Remove(crystalToSpawn);

                newCrystal.GetComponent<CrystalSkillController>()?.
                    SetupCrystal(crystalExistenceDuration, explosiveCrystalUnlocked, movingCrystalUnlocked, moveSpeed, FindClosestEnemy(newCrystal.transform));


                shootWindowTimer = shootWindow;

                if (crystalMag.Count <= 0)
                {
                    ReloadCrystalMag();
                }
            }

            return true;
        }

        return false;
    }

    public void DestroyCurrentCrystal_InCrystalMirageOnly()
    {
        if (currentCrystal != null)
        {
            Destroy(currentCrystal);
        }
    }

    public void CurrentCrystalSpecifyEnemy(Transform _enemy)
    {
        if (currentCrystal != null)
        {
            currentCrystal.GetComponent<CrystalSkillController>()?.SpecifyEnemyTarget(_enemy);
        }
    }

    public void EnterCooldown()
    {
        if (cooldownTimer < 0 && !crystalGunUnlocked)
        {
            InGame_UI.instance.SetCrystalCooldownImage();
            cooldownTimer = cooldown;
        }
        else if (cooldownTimer < 0 && crystalGunUnlocked)
        {
            InGame_UI.instance.SetCrystalCooldownImage();
            cooldownTimer = GetCrystalCooldown();
        }

        skillLastUseTime = Time.time;
    }

    public float GetCrystalCooldown()
    {
        float crystalCooldown = cooldown;

        if (crystalGunUnlocked)
        {
            if (shootWindowTimer > 0)
            {
                crystalCooldown = shootCooldown;
            }

            if (reloading)
            {
                crystalCooldown = reloadTime;
            }
        }

        return crystalCooldown;
    }

    public override bool SkillIsReadyToUse()
    {
        if (crystalGunUnlocked)
        {
            if (!reloading)
            {
                return true;
            }
            return false;
        }
        else
        {
            if (cooldownTimer < 0)
            {
                return true;
            }
            return false;
        }
    }

    //public Transform GetCurrentCrystalTransform()
    //{
    //    return currentCrystal?.transform;
    //}


    //public void CurrentCrystalChooseRandomEnemy(float _searchRadius)
    //{
    //    if (currentCrystal != null)
    //    {
    //        currentCrystal.GetComponent<CrystalSkillController>()?.CrystalChooseRandomEnemy(_searchRadius);
    //    }
    //}

    #region Unlock Crystal Skills
    private void UnlockCrystal()
    {
        if (crystalUnlocked)
        {
            return;
        }

        if (crystalUnlockButton.unlocked)
        {
            crystalUnlocked = true;
        }
    }

    private void UnlockCrystalMirage()
    {
        if (mirageBlinkUnlocked)
        {
            return;
        }

        if (mirageBlinkUnlockButton.unlocked)
        {
            mirageBlinkUnlocked = true;
        }
    }

    private void UnlockExplosiveCrystal()
    {
        if (explosiveCrystalUnlocked)
        {
            return;
        }

        if (explosiveCrystalUnlockButton.unlocked)
        {
            explosiveCrystalUnlocked = true;
        }
    }

    private void UnlockMovingCrystal()
    {
        if (movingCrystalUnlocked)
        {
            return;
        }

        if (movingCrystalUnlockButton.unlocked)
        {
            movingCrystalUnlocked = true;
        }
    }

    private void UnlockCrystalGun()
    {
        if (crystalGunUnlocked)
        {
            return;
        }

        if (crystalGunUnlockButton.unlocked)
        {
            crystalGunUnlocked = true;
        }
    }
    #endregion
}
