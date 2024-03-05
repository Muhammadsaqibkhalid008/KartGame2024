using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.Mathematics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using TMPro;
using Assets.Scripts;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] Button playBtn;
    [SerializeField] Button cancelBtn;
    [SerializeField] Button startBtn;
    [SerializeField] Button muteBtn;
    [SerializeField] Button characterSelectionCancelBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button exitButton;
    [Tooltip("check the build index of the scene you want to be played when clicked on the play button")]
    [SerializeField] int gameplaySceneIndex;
    [SerializeField] GameObject loadingScreenPanel;
    [SerializeField] Image fillImage;
    [SerializeField] float loadingDecliner;

    [SerializeField] string trainingAreaScneName;
    [SerializeField] string desertAreaSceneName;

    [SerializeField] GameObject levelSelectionPanel;
    [SerializeField] Image muteLine;

    // coins things
   // [SerializeField] TMP_Text totalCoins;
    public Text TotalCoins;

    // characters selection things
    [SerializeField] Image[] characterSelectionImages = new Image[0];
    [SerializeField] GameObject characterSelectionPanel;
    [SerializeField] Button customizeBtn;
    [SerializeField] float durationForCharacterSelectionImages = .2f;


    // will be used to rotate karts
    // be careful about the character kart indices
    [SerializeField] KartRotator[] kartRotator = new KartRotator[6];
    [SerializeField] ParticleSystem kartChangeEffect;
    [SerializeField] Transform transformToMoveTowards;
    [SerializeField] float lerpDuration;
    [SerializeField] AnimationCurve animationCurve;


    // settings options
    [Header("Settings options")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] Button cancelButtonsForSettings;
    public GameObject cannotPurchasePanel;
    public GameObject purchasedSuccessfully;

    [Header("Save/Load option")]
    [SerializeField] SaveAndLoadManager saveLoadManager;


    private float timer = 0f;
    private Vector3 defaultPosition;

    public Image[] levelScreenshots;
    private Image currentSelectedImage;
    public int selectedLevelImageIndex { get; set; } = -1;
    private string currentScene = "";
    private bool isMute = false;  // so the game is not mute from the start
    public GameObject currentSelectedPlayer { get; set; }
    public GameObject kartsHolder;
    private bool canThisKartMove = false;

    [Header("Scriptable data for the UI part")]
    [SerializeField] UI_Data ui_data;


    float defaultVolume;

    private void Awake()
    {
        ui_data.totalEnemiesKilled = 0; //for staying at safe side
        cannotPurchasePanel.SetActive(false);
        purchasedSuccessfully.SetActive(false);
        Time.timeScale = 1;
        defaultVolume = AudioListener.volume;

        if (PlayerPrefs.HasKey("initial"))
        {
            saveLoadManager.LoadData();
            Debug.Log("key found");
        }
        else
        {
            PlayerPrefs.SetString("initial", "set");
            saveLoadManager.SaveInitial();
           // saveLoadManager.LoadData();
            Debug.Log("key not found");
        }
    }

    private void Start()
    {
        this.canThisKartMove = false;
        this.settingsPanel.SetActive(false);

        playBtn.onClick.AddListener(OnButtonClick);
        cancelBtn.onClick.AddListener(OnCancelButtonClick);
        startBtn.onClick.AddListener(OnStartButtonClick);
        muteBtn.onClick.AddListener(OnMuteButtonClick);
        customizeBtn.onClick.AddListener(OnCustomizeClickButton);
        characterSelectionCancelBtn.onClick.AddListener(OnCharacterSelectCancelButton);
        settingsBtn.onClick.AddListener(OnSettingsButtonClick);
        cancelButtonsForSettings.onClick.AddListener(OnCancelButtonForSettingsClick);
        cancelButtonsForSettings.onClick.AddListener(OnCancelButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);

        // extra settings for when user clicks on any button, all other panels should disappear
        settingsBtn.onClick.AddListener(OnCharacterSelectCancelButton);
        settingsBtn.onClick.AddListener(OnCancelButtonClick);
        customizeBtn.onClick.AddListener(OnCancelButtonForSettingsClick);
        loadingScreenPanel.SetActive(false);

        characterSelectionPanel.SetActive(false);
        Array.ForEach(characterSelectionImages, image => image.gameObject.SetActive(false));



        int childIndex = 0;
        int totalChildren = 0;


        // getting total children first
        foreach (var child in levelSelectionPanel.GetComponentsInChildren<RectTransform>(includeInactive: true))
        {
            // get all the childrens
            if (child.gameObject.CompareTag("levelScreenshot"))
            {
                totalChildren++;
            }
        }
        levelScreenshots = new Image[totalChildren];

        // assigning images
        foreach (var child in levelSelectionPanel.GetComponentsInChildren<RectTransform>(includeInactive: true))
        {
            // get all the childrens
            if (child.gameObject.CompareTag("levelScreenshot"))
            {
                levelScreenshots[childIndex] = child.GetComponent<Image>();
                childIndex++;
            }
        }

        levelSelectionPanel.SetActive(false);

        // by default at start, trainign area level at index 0 is selected
        LevelIsChanged(0);

        // whenever the game is started, it will get the data from this ui_data scriptableObject
        this.isMute = ui_data.audioIsMute;
        // call the audio function now to make changed
        if (this.isMute)
        {
            muteLine.gameObject.SetActive(true);
            Camera.main.GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            muteLine.gameObject.SetActive(false);
            Camera.main.GetComponent<AudioListener>().enabled = true;
        }

        // setting up coins
      //  this.totalCoins.text = ui_data.totalCoins.ToString();
        this.TotalCoins.text = ui_data.totalCoins.ToString();

        int selectable = 0;
        for (int i = 0; i < ui_data.allSelectableKarts.Length; i++)
        {
            if (ui_data.allSelectableKarts[i])
            {
                selectable = i;
            }
        }

        foreach (var kart in kartRotator)
        {
            kart.gameObject.SetActive(false);
        }
        kartRotator[selectable].gameObject.SetActive(true);


        this.defaultPosition = this.kartsHolder.transform.position;



        // some extra button logics
        playBtn.onClick.AddListener(() =>
        {
            characterSelectionPanel.gameObject.SetActive(false);
            this.canThisKartMove = false;
        });

        customizeBtn.onClick.AddListener(() =>
        {
            levelSelectionPanel.gameObject.SetActive(false);
            this.canThisKartMove = true;
        });


    }

    public void TriedToPurchase(GameObject _obj, float ttl)
    {
        StartCoroutine(PurchasingCoroutine(_obj, ttl));
    }
    IEnumerator PurchasingCoroutine(GameObject _obj, float ttl)
    {
        _obj.SetActive(true);
        yield return new WaitForSeconds(ttl);
        _obj.SetActive(false);
    }
    private void Update()
    {
        // not a very big deal of performance
       // this.totalCoins.text = ui_data.totalCoins.ToString();
        this.TotalCoins.text = ui_data.totalCoins.ToString();


        //if (this.canThisKartMove)
        //{
        //    timer += Time.deltaTime;
        //    float percentage = this.timer / this.lerpDuration;
        //    kartsHolder.transform.position = Vector3.Lerp(kartsHolder.transform.position, this.transformToMoveTowards.position, this.animationCurve.Evaluate(percentage));
        //}
        //else
        //{
        //    // we need to move the kartHolder back to its original position
        //    timer += Time.deltaTime;
        //    float percentage = this.timer / this.lerpDuration;
        //    kartsHolder.transform.position = Vector3.Lerp(this.transformToMoveTowards.position, this.defaultPosition, this.animationCurve.Evaluate(percentage));
        //}

    }

    public void ChangeKartOnUserTouch(int kartCurrentIndex)
    {
        for (int i = 0; i < kartRotator.Length; i++)
        {
            kartRotator[i].gameObject.SetActive(false);
        }
        kartRotator[kartCurrentIndex].gameObject.SetActive(true);
        var _obj = kartRotator[kartCurrentIndex].gameObject;
        var clone = Instantiate(this.kartChangeEffect, _obj.transform.position, _obj.transform.rotation);
        var script = clone.AddComponent<DestroyerScript>();
        script.CallDestroyMethod(1f);
    }
    void OnButtonClick()
    {
        // we need to setActive the levelSelectionPanel
        levelSelectionPanel.SetActive(true);
    }
    void OnCancelButtonClick()
    {
        // when the cancel button is clicked
        // level selection panel should be turned off
        levelSelectionPanel.SetActive(false);

        // just for being on the safe side
        this.canThisKartMove = false;
        this.timer = 0f;
    }
    void OnStartButtonClick()
    {
        // be default the training area level is selected
        LoadScene(this.currentScene);
    }

    void OnExitButtonClick()
    {
        saveLoadManager.SaveData();
        Application.Quit();
    }

    public void LevelIsChanged(int changedIndex)
    {
        this.selectedLevelImageIndex = changedIndex;

        // now make a current selected level image
        currentSelectedImage = levelScreenshots[selectedLevelImageIndex];
        switch (selectedLevelImageIndex)
        {
            case 1:
                currentScene = trainingAreaScneName;
                break;
            case 0:
                currentScene = desertAreaSceneName;
                break;
        }

        Debug.Log($"Current screenshot is {currentSelectedImage.name}");
    }


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreenPanel.SetActive(true);

        while (!operation.isDone)
        {
            float percentage = Mathf.Clamp01(operation.progress / loadingDecliner);
            fillImage.fillAmount = percentage;
            yield return null;
        }
    }

    public void OnMuteButtonClick()
    {
        this.isMute = !this.isMute;
        if (this.isMute)
        {
            muteLine.gameObject.SetActive(true);
            this.isMute = true;

            // setting the audio to mute
            //Camera.main.GetComponent<AudioListener>().enabled = false;
            AudioListener.volume = 0f;
        }
        else
        {
            muteLine.gameObject.SetActive(false);
            this.isMute = false;

            // setting the audio to unmute
            //Camera.main.GetComponent<AudioListener>().enabled = true;
            AudioListener.volume = this.defaultVolume;
        }

        // setting the main data source of the audio
        ui_data.audioIsMute = this.isMute;
    }

    void OnCustomizeClickButton()
    {
        // when player clicks on the button, show the images and the panel
        characterSelectionPanel.gameObject.SetActive(true);

        // we need to move the kartsHolde a little bit to the right
        this.canThisKartMove = true;
        this.timer = 0f;

        StartCoroutine(CharacterSelectionCoroutine(this.characterSelectionImages));
    }
    void OnCharacterSelectCancelButton()
    {
        characterSelectionPanel.gameObject.SetActive(false);
        Array.ForEach(this.characterSelectionImages, image => image.gameObject.SetActive(false));

        // just for being on the safe side
        this.canThisKartMove = false;
        this.timer = 0f;
    }
    IEnumerator CharacterSelectionCoroutine(Image[] characterImages)
    {
        foreach (var image in characterImages)
        {
            image.gameObject.SetActive(true);
            yield return new WaitForSeconds(this.durationForCharacterSelectionImages);
        }
    }

    public void UpdateCharacterSelectionImages(int indexToUpdate)
    {
        // now from here you need to update the scriptable object ui_data
        ui_data.characterKartIndices[indexToUpdate] = true;
        // just to recall, true means the character has been unclocked
    }

    public void OnSettingsButtonClick()
    {
        // if the user touches the settings button, we need to open the panel containing the settings options
        this.settingsPanel.SetActive(true);
    }
    public void OnCancelButtonForSettingsClick()
    {
        // we need to terminate the settingsPanel
        this.settingsPanel.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        // best way to save game data if the application gets quit somehow
        // either by user's intentions or by fault
        saveLoadManager.SaveData();
    }

}
