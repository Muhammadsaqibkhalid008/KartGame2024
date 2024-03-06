using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PowerslideKartPhysics;
using UnityEngine.UI;
//using TMPro;

public class GameStarter : MonoBehaviour
{
   // [SerializeField] SaveAndLoadManager saveAndLoadManager;
    [SerializeField] Transform[] allkarts = new Transform[6];
    [SerializeField] UI_Data ui_data;
    [SerializeField] CinemachineVirtualCamera mainVcam;
   // [SerializeField] TMP_Text roundTimeText;
    public Text RoundText;
    [SerializeField] GameObject timePanel;
    [SerializeField] GameObject gameOverPanel;
    public Text TotalCoinsCollect;
    public Text TotalEnemeisKilled;
   // [SerializeField] TMP_Text totalCoinsCollectedText;
   // [SerializeField] TMP_Text totalEnemiesKilledText;

    [Header("this will be the kart starting position")]
    [SerializeField] Transform kartStartingPosition;

    [Tooltip("the Ui text mesh pro for total kills count")]
  //  [SerializeField] TMP_Text total_kills_ui;
    public Text Kills_UI_text;

    public Camera mainCamera;
    private bool startGame = false;
    float timer = 0;
    float maxTime = 0;
    string minConstant = "";
    [Tooltip("That kart which is active and playable, its playerUI component will be used")]
    public PlayerUI playerUI { get; set; }

    // public properites to be used for total coins and total enemies killed
    public int totalCoinsCollected { get; set; } = 0;
    public int totalEnemiesKilled { get; set; } = 0;

    private void Awake()
    {

       // this.roundTimeText.gameObject.SetActive(false);
        this.RoundText.gameObject.SetActive(false);
        this.gameOverPanel.SetActive(false);
        Time.timeScale = 0;

        int currentSelectableKartIndex = 0;
        for (int index = 0; index < ui_data.allSelectableKarts.Length; index++)
        {
            // since there is only one index that is true
            // gotta switch to that kart index
            if (ui_data.allSelectableKarts[index])
            {
                currentSelectableKartIndex = index;
            }
        }

        // not match this index with the available karts to get the game started
        foreach (var item in this.allkarts)
        {
            item.gameObject.SetActive(false);
        }
        allkarts[currentSelectableKartIndex].gameObject.SetActive(true);
        playerUI = allkarts[currentSelectableKartIndex].gameObject.GetComponent<PlayerUI>();

        mainVcam.LookAt = allkarts[currentSelectableKartIndex];
        mainVcam.Follow = allkarts[currentSelectableKartIndex];

        mainCamera = Camera.main;
        mainCamera.gameObject.GetComponent<KartCamera>().targetKart = allkarts[currentSelectableKartIndex].gameObject.GetComponent<Kart>();


        this.SpawnAtStartingPosition(this.kartStartingPosition, allkarts[currentSelectableKartIndex]);
    }

    void SpawnAtStartingPosition(Transform pos, Transform playerKart)
    {
        // we just need to spawn the kart at the specified position
        playerKart.position = pos.position;

        // we need to set the local transforms
        playerKart.transform.forward = pos.forward;
    }

    private void Update()
    {
        if (this.startGame)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                minConstant = (int.Parse(minConstant) - 1).ToString();
                timer = 60f;
            }
            //  roundTimeText.text = $"{minConstant}:{(int)timer}";
            RoundText.text = $"{minConstant}:{(int)timer}";
        }

        if (minConstant == "0" && timer <= 0.10f)
        {
            this.DisplayGameOverPanel();
            // since this is the logic for the game over, we need to update the totalCoinsCollected and the totalEnemiesKilled
          //  this.totalCoinsCollectedText.text = "Total coins collected " + this.playerUI.totalCoins.ToString();
            this.TotalCoinsCollect.text = "Total coins collected " + this.playerUI.totalCoins.ToString();
           // this.totalEnemiesKilledText.text = "Total enemies killed " + ui_data.totalEnemiesKilled.ToString();
            this.TotalEnemeisKilled.text = "Total enemies killed " + ui_data.totalEnemiesKilled.ToString();
        }


        // controlling the ui text mesh pro (total enemy kills)
        //  total_kills_ui.text = $"Kills: {ui_data.totalEnemiesKilled.ToString()}";
        Kills_UI_text.text = $"Kills: {ui_data.totalEnemiesKilled.ToString()}";
    }
    public void OnClickTimeButton(int timeIndex = 1)
    {
        // by default total time for the round is 1 min
        this.startGame = true;
        this.maxTime = timeIndex * 60;  // as converting for minutes
        this.timer = 60.10f;
        Time.timeScale = 1;
        minConstant = timeIndex.ToString();
        this.timePanel.SetActive(false);
       // this.roundTimeText.gameObject.SetActive(true);
        this.RoundText.gameObject.SetActive(true);

        // making an extra logic for one minute
        minConstant = (timeIndex - 1).ToString();
    }

    public void DisplayGameOverPanel()
    {
        Time.timeScale = 0;
        this.startGame = false;
        this.gameOverPanel.SetActive(true);
        //saveAndLoadManager.SaveData();
    }

}
