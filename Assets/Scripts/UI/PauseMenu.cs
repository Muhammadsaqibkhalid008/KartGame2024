using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts;
using Unity.VisualScripting;
using System;
using PowerslideKartPhysics;

public class PauseMenu : MonoBehaviour
{
    // get all the pause buttons
    [SerializeField] Transform restartBtn;
    [SerializeField] Transform resumeBtn;
    [SerializeField] Transform mainMenuBtn;
    [SerializeField] Transform pauseBtn;
    [SerializeField] Transform settingsBtn;
    [SerializeField] Transform pauseMenuPanel;
    [SerializeField] float buttonsEnablingDuration = .25f;

    // part 2
    [SerializeField] Button mainMenuButton_2;
    [SerializeField] Button restartButton_2;

    // settings area
    [SerializeField] Transform checkMarkForArrowControls;
    [SerializeField] Transform checkMarkForJoystickControls;
    [SerializeField] Transform settingsPanel;
    [SerializeField] Button kartControlsBtn;
    [SerializeField] Transform kartControlSelectionPanel;
    [SerializeField] Button soundMuteBtn;
    [SerializeField] Transform soundMuteCheckMark;
    [SerializeField] Button checkButtonArrow;
    [SerializeField] Button checkButtonJoystick;
    // check these values on the basis of ui_data property defined 



    [SerializeField] UI_Data ui_data;


    // private members
    private CustomInputManager customInputManager;

    enum CurrentControl
    {
        arrows, joystick
    }
    CurrentControl currentControl;


    private void Start()
    {
        // get all the button components first
        var restartBtnComponent = restartBtn.GetComponent<Button>();
        var resumeBtnComponent = resumeBtn.GetComponent<Button>();
        var mainMenuBtnComponent = mainMenuBtn.GetComponent<Button>();
        var pauseBtnComponent = pauseBtn.GetComponent<Button>();
        var settingBtnComponent = settingsBtn.GetComponent<Button>();

        // assign callback functions to their button components
        restartBtnComponent.onClick.AddListener(OnPressRestart);
        resumeBtnComponent.onClick.AddListener(OnPressResume);
        mainMenuBtnComponent.onClick.AddListener(OnPressMainMenu);
        pauseBtnComponent.onClick.AddListener(OnPressPause);
        settingBtnComponent.onClick.AddListener(OnPressSettings);
        kartControlsBtn.onClick.AddListener(OnPressKartControls);
        soundMuteBtn.onClick.AddListener(OnPressSoundMute);
        checkButtonArrow.onClick.AddListener(OnClickArrowControl);
        checkButtonJoystick.onClick.AddListener(OnClickJoystickControl);

        // for part 2
        mainMenuButton_2.onClick.AddListener(OnPressMainMenu);
        restartButton_2.onClick.AddListener(OnPressRestart);


        // turning everything off
        pauseMenuPanel.gameObject.SetActive(false);
        foreach (var child in pauseMenuPanel.GetComponentsInChildren<Button>(includeInactive: true))
        {
            child.gameObject.SetActive(false);
        }

        customInputManager = GameObject.FindObjectOfType<CustomInputManager>();


        // intital check from the scriptable database
        CheckForPlayerControls();
        settingsPanel.gameObject.SetActive(false);
        kartControlSelectionPanel.gameObject.SetActive(false);

        currentControl = new();

        // checking whether the audio is mute from the start or not
        if (ui_data.audioIsMute)
        {
            soundMuteCheckMark.gameObject.SetActive(false);
            SetCameraAudioListenerState(false);
        }
        else
        {
            soundMuteCheckMark.gameObject.SetActive(true);
            SetCameraAudioListenerState(true);
        }
    }



    void OnClickArrowControl()
    {
        currentControl = CurrentControl.arrows;
        checkMarkForArrowControls.gameObject.SetActive(true);
        checkMarkForJoystickControls.gameObject.SetActive(false);
        ui_data.isUsingArrows = true;
        customInputManager.CheckForControls();
    }
    void OnClickJoystickControl()
    {
        currentControl = CurrentControl.joystick;
        checkMarkForArrowControls.gameObject.SetActive(false);
        checkMarkForJoystickControls.gameObject.SetActive(true);
        ui_data.isUsingArrows = false;
        customInputManager.CheckForControls();
    }

    void SetCameraAudioListenerState(bool state)
    {
        Camera.main.GetComponent<AudioListener>().enabled = state;
    }


    void OnPressSoundMute()
    {
        // same logic as mute button, you did in the selection screen
        if (ui_data.audioIsMute)
        {
            soundMuteCheckMark.gameObject.SetActive(true);
            ui_data.audioIsMute = false;
            SetCameraAudioListenerState(true);
        }
        else
        {
            soundMuteCheckMark.gameObject.SetActive(false);
            ui_data.audioIsMute = true;
            SetCameraAudioListenerState(false);
        }
    }
    void OnPressKartControls()
    {
        // we need to open the kart controls selection panel from where the user can 
        // select either the arrow controls or the joystick controls
        kartControlSelectionPanel.gameObject.SetActive(true);
    }
    void CheckForPlayerControls()
    {
        if (ui_data.isUsingArrows)
        {
            // you know the logics....
            checkMarkForArrowControls.gameObject.SetActive(true);
            checkMarkForJoystickControls.gameObject.SetActive(false);

            // update the sctipable database
            ui_data.isUsingArrows = true;
            currentControl = CurrentControl.arrows;
        }
        else
        {
            // you know the logics....
            checkMarkForArrowControls.gameObject.SetActive(false);
            checkMarkForJoystickControls.gameObject.SetActive(true);

            // update the scriptbale database
            ui_data.isUsingArrows = false;
            currentControl = CurrentControl.joystick;
        }


        // updating and calling the method from the custom input manager
        customInputManager.CheckForControls();
    }

    void OnPressRestart()
    {
        // restart the current level
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void OnPressResume()
    {
        // we need to cut off the pause menu panel and disable all the buttons as well
        // so next time the user pauses the game again, they will show animations again
        pauseMenuPanel.gameObject.SetActive(false);
        foreach (var child in pauseMenuPanel.GetComponentsInChildren<Button>(includeInactive: true))
        {
            child.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
        TurnEnemyValue(true);
    }
    void OnPressMainMenu()
    {
        // get back to the main scene of index 0
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
    void OnPressPause()
    {
        // now we need to enable the pause menu panel and its all children as well
        StartCoroutine(ButtonEnablingCoroutine(pauseMenuPanel));
        //Time.timeScale = 0f;
        TurnEnemyValue(false);
    }

    void TurnEnemyValue(bool value)
    {
        foreach (var item in this.enemySpawner.GetAllEnemies())
        {
            item.GetComponent<Kart>().enabled = value;
        }
    }
    void OnPressSettings()
    {
        settingsPanel.gameObject.SetActive(true);
    }

    IEnumerator ButtonEnablingCoroutine(Transform parent)
    {
        Button[] children = parent.GetComponentsInChildren<Button>(includeInactive: true);
        Debug.Log("total child count is " + children.Length + "name is " + parent.name);

        parent.gameObject.SetActive(true);

        for (int i = 0; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(this.buttonsEnablingDuration);
        }
    }

    private EnemySpawner enemySpawner;


    private void OnEnable()
    {
        Debug.Log("timeScale value is " + Time.timeScale);

        try
        {
            enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
            if (enemySpawner != null)
            {
                Debug.Log("got the enemy spawner");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"error getting enemy ai karts {e.Message}");
        }
    }
}
