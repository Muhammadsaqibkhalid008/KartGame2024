using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScreenshotManager : MonoBehaviour
{

    UI_Manager uiManager;
    public GameObject[] Boxes;
    private void Start()
    {
        uiManager = GameObject.FindObjectOfType<UI_Manager>();

    }
    public void SetImageIndex(int index)
    {
        // whenever the index is changed make a callback and report function to
        // let the UIManager container know something
        uiManager.LevelIsChanged(index);
        if (index == 0)
        {
        Boxes[0].SetActive(true);
        Boxes[1].SetActive(false);
        }
        if (index == 1)
        {
            Boxes[1].SetActive(true);
            Boxes[0].SetActive(false);
        }
    }
}
