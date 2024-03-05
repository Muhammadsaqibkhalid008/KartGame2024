using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        AudioListener.volume = slider.value;
    }

    public void OnValueChangedSlider(float newValue)
    {
        AudioListener.volume = newValue;
    }



}
