using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Player player;


    [Header("上浮跳字")]
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("受伤闪烁特效")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMaterial;
    private Material originalMaterial;

    [Header("异常状态颜色")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color chillColor;
    [SerializeField] private Color[] shockColor;

    private bool canApplyAilmentColor;

    [Header("异常状态 粒子")]
    [SerializeField] private ParticleSystem igniteFX;
    [SerializeField] private ParticleSystem chillFX;
    [SerializeField] private ParticleSystem shockFX;

    [Header("击中效果")]
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField] private GameObject critHitFXPrefab;

    protected GameObject HPBar;


    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        HPBar = GetComponentInChildren<HPBar_UI>()?.gameObject;
    }

    protected virtual void Start()
    {
        originalMaterial = sr.material;
        player = PlayerManager.instance.player;
    }

    protected virtual void Update()
    {

    }



    public GameObject CreatePopUpText(string _text)
    {
        float xOffset = Random.Range(-1, 1);
        float yOffset = Random.Range(1.5f, 3);

        Vector3 postionOffset = new Vector3(xOffset, yOffset, 0);

        GameObject newText = Instantiate(popUpTextPrefab, transform.position + postionOffset, Quaternion.identity);
        newText.GetComponent<TextMeshPro>().text = _text;

        return newText;
    }

    private IEnumerator FlashFX()
    {
        canApplyAilmentColor = false;

        sr.material = hitMaterial;

        
        //Color originalColor = sr.color;
        //sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        //sr.color = Color.white;
        sr.material = originalMaterial;

        canApplyAilmentColor = true;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;
        }
    }

    private void CancelColorChange()
    {
        CancelInvoke();

        sr.color = Color.white;

        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }

    public void MakeEntityTransparent(bool _transparent)
    {
        if (_transparent)
        {
            sr.color = Color.clear;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    public void MakeEntityTransparent_IncludingHPBar(bool _transparent)
    {
        if (_transparent)
        {
            if (HPBar != null)
            {
                HPBar.SetActive(false);
            }
            sr.color = Color.clear;
        }
        else
        {
            if (HPBar != null)
            {
                HPBar.SetActive(true);
            }
            sr.color = Color.white;
        }
    }

    #region Ailment FX
    public void EnableIgniteFXForTime(float _seconds)
    {
        igniteFX.Play();
        InvokeRepeating("IgniteColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
        //Invoke("CancelColorChange", _seconds + 0.1f);
    }

    private void IgniteColorFX()
    {
        if (sr.color != igniteColor[0])
        {
            sr.color = igniteColor[0];
            Debug.Log("Set to ignite color 0");
        }
        else
        {
            sr.color = igniteColor[1];
            Debug.Log("Set to ignite color 1");
        }
    }

    public void EnableChillFXForTime(float _seconds)
    {
        chillFX.Play();
        ChillColorFX();
        Invoke("CancelColorChange", _seconds);
    }

    private void ChillColorFX()
    {
        if (sr.color != chillColor)
        {
            sr.color = chillColor;
        }
    }

    public void EnableShockFXForTime(float _seconds)
    {
        shockFX.Play();
        InvokeRepeating("ShockColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void ShockColorFX()
    {
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    #region AilmentColorFX_Coroutine
    public IEnumerator EnableIgniteFXForTime_Coroutine(float _seconds)
    {
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableIgniteFXForTime(_seconds);

    }

    public IEnumerator EnableChillFXForTime_Coroutine(float _seconds)
    {
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableChillFXForTime(_seconds);

    }

    public IEnumerator EnableShockFXForTime_Coroutine(float _seconds)
    {
        yield return new WaitUntil(() => canApplyAilmentColor == true);
        EnableShockFXForTime(_seconds);

    }
    #endregion

    #endregion

    public void CreateHitFX(Transform _targetTransform, bool _canCrit)
    {
        float zRotation = Random.Range(-90, 90);
        float xOffset = Random.Range(-0.5f, 0.5f);
        float yOffset = Random.Range(-0.5f, 0.5f);

        Vector3 generationPosition = new Vector3(_targetTransform.position.x + xOffset, _targetTransform.position.y + yOffset);
        Vector3 generationRotation = new Vector3(0, 0, zRotation);

        GameObject prefab = hitFXPrefab;

        if (_canCrit)
        {
            prefab = critHitFXPrefab;

            zRotation = Random.Range(-30, 30);

            float yRotation = 0;
            if (GetComponent<Entity>().facingDirection == -1)
            {
                yRotation = 180;
            }

            generationRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newHitFX = Instantiate(prefab, generationPosition, Quaternion.identity);

        newHitFX.transform.Rotate(generationRotation);

        Destroy(newHitFX, 0.5f);
    }




}
