using System.Collections;
using UnityEngine;
using Cinemachine;
using TMPro;

public class EntityFX : MonoBehaviour
{
    protected Player player;
    private SpriteRenderer sr;

    [Header("Pop up text")]
    [SerializeField] private GameObject popUpTextPrefab;



    //冲刺时的残影特效
    [Header("After image fx")]
    [SerializeField] private float afterImageCooldown;
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    private float afterImageCooldownTimer;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private float flashDuration;
    private Material originalMat;

    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailments particles")]
    [SerializeField] private ParticleSystem ignitFX;
    [SerializeField] private ParticleSystem chillFX;
    [SerializeField] private ParticleSystem shockFX;

    [Header("Hit fx")]
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField] private GameObject criticalHitFXPrefab;


    private GameObject myHealthBar;

    protected virtual void Start()
    {
        myHealthBar = GetComponentInChildren<UI_HealthBar>().gameObject;
        sr = GetComponentInChildren<SpriteRenderer>();
        player = PlayerManager.instance.player;
        
        originalMat = sr.material;
    }

    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    public void CreatePopUpText(string _text)
    {
        //生成文字提示
        float xOffset = Random.Range(-1, 1);
        float yOffset = Random.Range(1.5f, 5);

        Vector3 positionOffset = new Vector3(xOffset, yOffset, 0);

        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);
        newText.GetComponent<TextMeshPro>().text = _text;
    }

    public void CreateAfterImage()
    {
        if(afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetAfterImage(colorLooseRate, sr.sprite);
        }
    }
    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.material = originalMat;
        sr.color = currentColor;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        ignitFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }
    public void IgniteFxFor(float _seconds)
    {
        ignitFX.Play();

        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }
    public void ChillFxFor(float _seconds)
    {
        chillFX.Play();

        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        shockFX.Play();

        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
        {
            myHealthBar.SetActive(false);
            sr.color = Color.clear;
        }
        else
        {
            myHealthBar.SetActive (true);
            sr.color = Color.white;
        }
    }

    public void CreateHitFX(Transform _target, bool _isCritical)
    {
        float zRotation = Random.Range(-90, 90);
        float xOffset = Random.Range(-0.5f, 0.5f);
        float yOffset = Random.Range(-0.5f, 0.5f);

        Vector3 hitFXRotation = new Vector3(0, 0, zRotation);
        //普通打击特效 or 暴击特效
        GameObject hitPrefab = hitFXPrefab;
        if (_isCritical)
        {
            hitPrefab = criticalHitFXPrefab;

            float yRotation = 0;
            if (GetComponent<Entity>().facingDir == -1)
                yRotation = 180;
            zRotation = Random.Range(-45, 45);
            hitFXRotation = new Vector3(0, yRotation, zRotation);
        }

        //每次生成的特效位置和角度都会有点不同
        GameObject newHitFX = Instantiate(hitPrefab, _target.position + new Vector3(xOffset, yOffset), Quaternion.identity, _target.transform);
        newHitFX.transform.Rotate(hitFXRotation);

        //特效完毕后进行销毁
        Destroy(newHitFX, .5f);
    }



}
