using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// 剑相关技能实现
public class SwordSkillController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;   // 旋转检测范围
    private Player player;

    private bool canRotate = true;      // 是否旋转
    private bool isReturning;   // 是否正在返回
    private float swordReturnSpeed; // 返回速度
    private Vector2 launchSpeed; // 发射速度 设置特效方向

    private float enemyFreezeDuration;      // 敌人冻结时间
    private float enemyVulnerableDuration;          // 敌人脆弱事件

    [Header("弹跳剑")]
    private bool isBouncingSword;
    private int bounceAmount;
    private float bounceSpeed;
    private List<Transform> bounceTargets = new List<Transform>();
    private int bounceTargetIndex;

    [Header("穿刺剑属性")]
    private bool isPierceSword;
    private int pierceAmount;

    [Header("锯旋剑属性")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinSword;

    private float spinHitCooldown;
    private float spinHitTimer;

    private bool spinTimerHasBeenSetToSpinDuration = false;
    private float spinDirection;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
        player = PlayerManager.instance.player;
    }

    private void Update()
    {
        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, swordReturnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1.5)
            {
                player.CatchSword(); // 玩家拿到剑之后销毁剑
            }
        }

        BounceSwordLogic();

        SpinSwordLogic();

        DestroySwordIfTooFar(30);
    }


    private void StopAndSpin()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        if (!spinTimerHasBeenSetToSpinDuration)
        {
            spinTimer = spinDuration;
        }

        spinTimerHasBeenSetToSpinDuration = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 返回的时候不检测
        if (isReturning)
        {
            return;
        }

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
        }

        
        if (isSpinSword)
        {
            StopAndSpin();
            return;
        }
        else
        {
            DamageAndFreezeAndVulnerateEnemy(collision);
        }


        SetupBounceSwordTargets(collision);

        SwordStuckInto(collision);
    }


    private void DamageAndFreezeAndVulnerateEnemy(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            // 击退并造成伤害
            player.stats.DoDamge(enemy.GetComponent<CharacterStats>());
            // 如果解锁了timeStop 则冻结敌人
            if (SkillManager.instance.sword.timeStopUnlocked)
            {
                enemy.FreezeEnemyForTime(enemyFreezeDuration);

            }

            if (SkillManager.instance.sword.vulnerabilityUnlocked)
            {
                enemy.stats.BecomeVulnerableForTime(enemyVulnerableDuration);
            }

            //summon charm effect
            Inventory.instance.UseCharmEffect_ConsiderCooldown(enemy.transform);
            //ItemData_Equipment equippedCharm = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Charm);

            //if (equippedCharm != null)
            //{
            //    equippedCharm.ExecuteItemEffect(enemy.transform);
            //}
        }

    }

    private void SwordStuckInto(Collider2D collision)
    {
        // 穿透次数 
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinSword && collision.GetComponent<Enemy>() != null)
        {
            StopAndSpin();
            return;
        }
        //控制旋转和碰撞体
        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        //  将刚体设置为静态 (isKinematic = true) 并冻结所有物理行为 (FreezeAll)，以使剑完全停止移动。
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncingSword && bounceTargets.Count > 0)
        {
            return;
        }

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;

        // 尘土效果
        ParticleSystem dustFX = GetComponentInChildren<ParticleSystem>();
        if (dustFX != null)
        {
            if (launchSpeed.x < 0)
            {
                dustFX.transform.localScale = new Vector3(-1, 1, 1);
            }

            dustFX.Play();
        }
    }

    private void BounceSwordLogic()
    {
        if (isBouncingSword && bounceTargets.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, bounceTargets[bounceTargetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, bounceTargets[bounceTargetIndex].position) < 0.15f)
            {
                DamageAndFreezeAndVulnerateEnemy(bounceTargets[bounceTargetIndex].GetComponent<Collider2D>());
                bounceTargetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncingSword = false;
                    isReturning = true;
                }

                if (bounceTargetIndex >= bounceTargets.Count)
                {
                    bounceTargetIndex = 0;
                }
            }
        }
    }

    private void SetupBounceSwordTargets(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncingSword && bounceTargets.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        bounceTargets.Add(hit.transform);
                    }
                }
            }

            bounceTargets.Sort(new SortByDistanceToPlayer_BounceSwordTargets());
        }
    }

    private void SpinSwordLogic()
    {
        if (isSpinSword)
        {
            // 如果剑到了最远距离还没有停下来的话 则直接停下旋转
            if (Vector2.Distance(player.transform.position, transform.position) >= maxTravelDistance && !wasStopped)
            {
                StopAndSpin();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;
                spinHitTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinSword = false;
                }

                if (spinHitTimer < 0)
                {
                    spinHitTimer = spinHitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            DamageAndFreezeAndVulnerateEnemy(hit);
                        }
                    }
                }
            }
        }
    }

    public void SetupSword(Vector2 _launchSpeed, float _swordGravity, float _swordReturnSpeed, float _enemyFreezeDuration, float _enemyVulnerableDuration)
    {
        rb.velocity = _launchSpeed;
        rb.gravityScale = _swordGravity;
        swordReturnSpeed = _swordReturnSpeed;
        enemyFreezeDuration = _enemyFreezeDuration;
        enemyVulnerableDuration = _enemyVulnerableDuration;

        launchSpeed = _launchSpeed;

        if (!isPierceSword)
        {
            anim.SetBool("Rotation", true);
        }

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

    }

    public void SetupBounceSword(bool _isBounceSword, int _bounceAmount, float _bounceSpeed)
    {
        isBouncingSword = _isBounceSword;
        bounceAmount = _bounceAmount;
        bounceSpeed = _bounceSpeed;
    }

    public void SetupPierceSword(bool _isPierceSword, int _pierceAmount)
    {
        isPierceSword = _isPierceSword;
        pierceAmount = _pierceAmount;
    }

    public void SetupSpinSword(bool _isSpinSword, float _maxTravelDistance, float _spinDuration, float _spinHitCooldown)
    {
        isSpinSword = _isSpinSword;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        spinHitCooldown = _spinHitCooldown;
    }

    public void ReturnSword()
    {
        if (bounceTargets.Count > 0)
        {
            return;
        }

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;
    }

    private void DestroySwordIfTooFar(float _maxDistance)
    {
        if (Vector2.Distance(player.transform.position, transform.position) >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
