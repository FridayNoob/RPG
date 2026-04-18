using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float speed;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private float colorDisappearSpeed;

    [SerializeField] private float lifeTime;
    private float textTimer;
    void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }


    void Update()
    {
        textTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y + 1), speed * Time.deltaTime);

        if(textTimer < 0)
        {
            //逐渐变透明
            float alpha = myText.color.a - Time.deltaTime * colorDisappearSpeed;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);
            //透明度达到界限后
            if (myText.color.a < 0.5)
                speed = disappearSpeed;

            if (myText.color.a <= 0)
                Destroy(gameObject);
        }
    }
}
