using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private float colorLooseRate;

    public void SetAfterImage(float _loosingSpeed, Sprite _spriteImage)
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = _spriteImage;

        colorLooseRate = _loosingSpeed;
    }

    private void Update()
    {
        //随时间逐渐变得透明
        float alpha = sr.color.a - colorLooseRate * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (sr.color.a <= 0)
            Destroy(gameObject);
    }
}
