using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;
using TMPro;
using PowerslideKartPhysics;

public class CinemachineStart : MonoBehaviour
{
    [SerializeField] float eachVcamTime = 1f;
    [SerializeField] TMP_Text startScoreText;
    [SerializeField] Kart targetKart;
    private void Start()
    {
        startScoreText.gameObject.SetActive(false);
        StartCoroutine(CinemachineTrackCoroutine());
    }

    void SetScore(string score)
    {
        startScoreText.gameObject.SetActive(false);
        startScoreText.gameObject.SetActive(true);
        startScoreText.text = score;
    }

    IEnumerator CinemachineTrackCoroutine()
    {
        SetScore("3");
        yield return new WaitForSeconds(eachVcamTime);
        SetScore("2");
        yield return new WaitForSeconds(eachVcamTime);
        SetScore("1");
        yield return new WaitForSeconds(eachVcamTime);
        SetScore("START");


    }
}
