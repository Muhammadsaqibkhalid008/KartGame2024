using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float threshold = 10f;   // a minute of 10 seconds
    private float startTimer = 0f;

    private void Update()
    {
        startTimer += Time.deltaTime; ;
    }

}
