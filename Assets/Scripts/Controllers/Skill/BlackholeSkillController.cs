using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Rendering;
using UnityEngine;

public class BlackholeSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> hotkeyList;  

    private float maxSize;
    private bool canGrow = true;
    private float growSpeed;
    private bool canShrink;
    private float shrinkSpeed;

    private int cloneAttackAmount;
    private float cloneAttackCooldown;
    private float cloneAttackTimer;
    private bool canCloneAttack;

    private bool canCreateHotkey = true;

    private bool playerIsTransparent;
    private bool canExitBlackHoleSkill;

    private float QTEInputTimer;

    private List<Transform> enemyTargets = new List<Transform>();
    private List<GameObject> createdHotkey = new List<GameObject>();


    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        QTEInputTimer -= Time.deltaTime;


        if (QTEInputTimer >= 0)
        {
            
            if (enemyTargets.Count > 0 && enemyTargets.Count == createdHotkey.Count)
            {
                
                QTEInputTimer = Mathf.Infinity;
                ReadyToReleaseBlackholeCloneAttack();
                BlackholeCloneAttack();
            }
        }
        else if (QTEInputTimer < 0)  
        {
            
            if (enemyTargets.Count > 0)
            {
                ReadyToReleaseBlackholeCloneAttack();
                BlackholeCloneAttack();
            }
            else  
            {
                EndCloneAttack();
            }

        }

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeEnemy(true);

            CreateHotkey(collision);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeEnemy(false);
        }
    }

    public void SetupBlackholeSkill(float _maxSize, float _growSpeed, float _shrinkSpeed, int _cloneAttackAmount, float _cloneAttackCooldown, float _QTEInputWindow)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        cloneAttackAmount = _cloneAttackAmount;
        cloneAttackCooldown = _cloneAttackCooldown;
        QTEInputTimer = _QTEInputWindow;

        if (SkillManager.instance.clone.crystalMirageUnlocked)
        {
            playerIsTransparent = true;
        }
    }

    private void ReadyToReleaseBlackholeCloneAttack()
    {
        DestroyHotkeys();
        canCloneAttack = true;
        canCreateHotkey = false;  

        if (!playerIsTransparent)
        {
            PlayerManager.instance.player.fx.MakeEntityTransparent(true);
            playerIsTransparent = true;
        }
    }

    private void BlackholeCloneAttack()
    {
        if (cloneAttackTimer < 0 && canCloneAttack && cloneAttackAmount > 0 && enemyTargets.Count > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, enemyTargets.Count);

            Vector3 offset;
            if (Random.Range(0, 100) > 50)
            {
                offset = new Vector3(1, 0);
            }
            else
            {
                offset = new Vector3(-1, 0);
            }

            if (SkillManager.instance.clone.crystalMirageUnlocked)
            {
                SkillManager.instance.crystal.CreateCrystal();

                SkillManager.instance.crystal.CurrentCrystalSpecifyEnemy(enemyTargets[randomIndex]);
            }
            else
            {
                SkillManager.instance.clone.CreateClone(enemyTargets[randomIndex].position + offset);
            }

            cloneAttackAmount--;

            if (cloneAttackAmount <= 0)
            {
                Invoke("EndCloneAttack", 0.5f);
            }
        }
    }

    private void EndCloneAttack()
    {
        DestroyHotkeys();
        canExitBlackHoleSkill = true;
        canShrink = true;
        canCloneAttack = false;
    }

    private void CreateHotkey(Collider2D collision)
    {
        if (hotkeyList.Count <= 0)
        {
            Debug.Log("No enough available hotkeys in list");
            return;
        }

        if (!canCreateHotkey)
        {
            return;
        }

        GameObject newHotkey = Instantiate(hotkeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity); ;
        createdHotkey.Add(newHotkey);

        Blackhole_HotkeyController newHotkeyScript = newHotkey.GetComponent<Blackhole_HotkeyController>();

        KeyCode chosenKey = hotkeyList[Random.Range(0, hotkeyList.Count)];
        hotkeyList.Remove(chosenKey);

        newHotkeyScript.SetupHotkey(chosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform)
    {
        enemyTargets.Add(_enemyTransform);
    }

    private void DestroyHotkeys()
    {
        if (createdHotkey.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < createdHotkey.Count; i++)
        {
            Destroy(createdHotkey[i]);
        }
    }

    public bool CloneAttackHasFinished()
    {
        if (canExitBlackHoleSkill)
        {
            return true;
        }

        return false;
    }
}
