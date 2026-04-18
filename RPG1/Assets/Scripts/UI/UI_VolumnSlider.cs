using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumnSlider : MonoBehaviour
{
    public Slider slider;
    public string parameter;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier;

    public void SliderValue(float _value)
    {
        //设置背景音乐或者音效声音大小
        audioMixer.SetFloat(parameter, Mathf.Log10(_value) * multiplier);
    }

    public void LoadSlider(float _value)
    {
        if (_value >= 0.0001f)
            slider.value = _value;
    }
}
