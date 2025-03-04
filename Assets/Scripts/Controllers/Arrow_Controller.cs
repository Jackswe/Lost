using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    [SerializeField] private string targetLayerName = "Player";
    //[SerializeField] private int damage;

    //private float xVelocity;
    private Vector2 flySpeed;
    private Rigidbody2D rb;
    private CharacterStats archerStats;

    [SerializeField] private bool canMove = true;
    [SerializeField] private bool flipped = false;

    private bool isStuck = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove)
        {
            rb.velocity = flySpeed;
            transform.right = rb.velocity;
        }

        // 3-5s�������ټ�
        if (isStuck)
        {
            Invoke("BecomeTransparentAndDestroyArrow", Random.Range(3, 5));
        }
        // �����û�п�ס ��10s֮������
        Invoke("BecomeTransparentAndDestroyArrow", 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            if (collision.GetComponent<CharacterStats>() != null)
            {
                archerStats.DoDamge(collision.GetComponent<CharacterStats>());
                StuckIntoCollidedObject(collision);
            }

        }
        // �����е���
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StuckIntoCollidedObject(collision);
        }
    }

    public void SetupArrow(Vector2 _speed, CharacterStats _archerStats)
    {
        flySpeed = _speed;
        // ��һ���ж��Ƿ���Ҫ��ת��ʸ
        if (flySpeed.x < 0)
        {
            transform.Rotate(0, 180, 0);
        }

        archerStats = _archerStats;
    }

    private void StuckIntoCollidedObject(Collider2D collision)
    {
        // �ر�����Ч��
        GetComponentInChildren<ParticleSystem>()?.Stop();
        // ��ֹ������˺����
        GetComponent<CapsuleCollider2D>().enabled = false;

        canMove = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = collision.transform;

        isStuck = true;
    }

    private void BecomeTransparentAndDestroyArrow()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - (5 * Time.deltaTime));

        if (sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void FlipArrow()
    {
        if (flipped)
        {
            return;
        }

        flySpeed.x *= -1;
        flySpeed.y *= -1;
        transform.Rotate(0, 180, 0);
        flipped = true;

        targetLayerName = "Enemy";
    }
}
