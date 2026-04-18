using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    //控制血条
    private Entity entity => GetComponentInParent<Entity>();
    private RectTransform myTransform;

    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private Slider slider;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();


        UpdateHealthUI();
    }

    private void OnEnable()
    {
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;
    }
    private void OnDisable()
    {
        if(entity != null)
            entity.onFlipped -= FlipUI;
        if(myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;
    }

    //显示当前血量和最大血量
    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void FlipUI()
    {
        myTransform.Rotate(0, 180, 0);
    }
}
