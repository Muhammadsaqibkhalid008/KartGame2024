using PowerslideKartPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource kartEngineSound;

    private bool soundStartedPlaying = false;

    [SerializeField] private float desiredTime = 1f;
    private float timer = 0f;
    [SerializeField] private float maxVolume = .5f;

    private void Start()
    {
        // kartEngineSound = GetComponent<AudioSource>();
    }
    public void PlayKartEngineSound()
    {
        if (kartEngineSound.isPlaying) return;
        kartEngineSound.Play();
    }
    public void StopKartEngineSound()
    {
        kartEngineSound.Stop();
    }

    private void Update()
    {
        //  Debug.Log($"{KartInputMobile.}")
        Debug.Log($"mobile accel {InputManager.GetMobileAccel()}, mobile brake {InputManager.GetMobileBrake()}");

        if (InputManager.GetMobileAccel() == 1)
        {
            soundStartedPlaying = true;
            if (soundStartedPlaying) PlayKartEngineSound();
        }
        else
        {
            soundStartedPlaying = false;
            StopKartEngineSound();
        }




        //
        if (soundStartedPlaying)
        {
            timer += Time.deltaTime;
            float per = timer / desiredTime;
            kartEngineSound.volume = Mathf.Lerp(0, maxVolume, Mathf.SmoothStep(0, 1, per));
        }
        else
        {
            timer = 0f;
        }
    }

}
