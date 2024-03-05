using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KartUnlockSystem : MonoBehaviour
{
    // all the karts that are unlocked will be checked over and unclocked based on the coins that the gamer has

    /* 
     * 
     * 1- lockerImage
     * 2- coins (image)
     * 3- coinsText
     
     */


    [SerializeField] private UI_Data ui_data;
   // [SerializeField] private TMP_Text coinText;
    public Text CoinsText;
    [SerializeField] private SaveAndLoadManager slm;

    private int totalCoins;
    private Button btn;
    private UI_Manager uiManager;

    public int currentCharacterSelectionIndex { get; set; } = -1;
    public bool currentUnlockStatus { get; set; } = false;

    private void Start()
    {
        totalCoins = 0;
        totalCoins = ui_data.totalCoins;
        btn = GetComponent<Button>();

        btn.onClick.AddListener(OnPressCharacter);

        uiManager = GameObject.FindObjectOfType<UI_Manager>();

        this.currentCharacterSelectionIndex = int.Parse(this.gameObject.name);

        this.currentUnlockStatus = ui_data.characterKartIndices[this.currentCharacterSelectionIndex];

        if (this.currentUnlockStatus)
        {
            // now we need to keep this current character unlocked
            this.UnlockCurrentObjectOnItsOwn();

            // assing the function to the delegate button
            this.AddEventListerer();

            // we need to remove the delegate function of OnPressCharacter()
            // because the character has already been selected, so there is not to bind the
            // this function to the kart or the character
            this.btn.onClick.RemoveListener(this.OnPressCharacter);
        }


        // there is also one more condition, the first kart should always vebe unlocked
        if (this.currentCharacterSelectionIndex == 0)
        {
            this.UnlockCurrentObjectOnItsOwn();
        }

    }

    Image GetLockerImage() => this.transform.GetChild(1).GetComponent<Image>();
    Image GetCoinImage() => this.transform.GetChild(2).GetComponent<Image>();
    TMP_Text GetCointText() => this.transform.GetChild(3).GetComponent<TMP_Text>();



    void OnPressCharacter()
    {
        Debug.Log("Button is pressed");

        // we need to get the coins each time user tries another purchase
        this.totalCoins = ui_data.totalCoins;
        int totalAmount = int.Parse(this.GetCointText().text);

        //at this place we need to display a popup message


        if (!(this.totalCoins >= totalAmount) && gameObject.name != "0")
        {
            uiManager.TriedToPurchase(uiManager.cannotPurchasePanel, 2.5f);
            return;
        }

        // make a logic so when the user has coins >= 100, he can easilty purchase the current
        // selected/pressed kart
        this.GetLockerImage().gameObject.SetActive(false);
        this.GetCoinImage().gameObject.SetActive(false);
        this.GetCointText().gameObject.SetActive(false);

        // update the coin system data as well
        ui_data.totalCoins = this.totalCoins - totalAmount;
        this.totalCoins -= totalAmount;

        // update the character selection indices from the ui manager as well
        uiManager.UpdateCharacterSelectionImages(this.currentCharacterSelectionIndex);

        // assing the function to the delegate button
        this.AddEventListerer();
        this.btn.onClick.RemoveListener(this.OnPressCharacter);

        //we've unlocked successfully
        if (gameObject.name != "0")
            uiManager.TriedToPurchase(uiManager.purchasedSuccessfully, 2.5f);


        //.........// recent change
        //  ui_data.totalCoins -= totalAmount;
        // slm.SaveData();

    }

    /// <summary>
    /// this pressing is triggered when the character is already unlocked and user selects this kart
    /// </summary>

    void OnPressCharacterUnlocked()
    {
        // so when this button is pressed user has usually selected a different kart that is already unlocked
        // we need to send data to UI manager so it can take care of updating the new UI kart
        int thisCharacterIndex = int.Parse(this.transform.gameObject.name);

        // now check if the kart at the same index is unlocked or not (already)

        // unlocked
        // now among the indices of the ui_data's allSelectable karts
        // check true the one matching with this index
        // also make sure when this ganmeObject's index is used, all other indices are 
        // marked as false

        for (int index = 0; index < ui_data.allSelectableKarts.Length; index++)
        {
            ui_data.allSelectableKarts[index] = false;
        }

        ui_data.allSelectableKarts[thisCharacterIndex] = true;


        // need to change the kart rotator so user can see the kart
        uiManager.ChangeKartOnUserTouch(this.currentCharacterSelectionIndex);


        Debug.Log($"selected kart index is {thisCharacterIndex} and the transform name is {this.transform.name}");
    }
    private void UnlockCurrentObjectOnItsOwn()
    {
        // in case if we want to unlock manually without any limitation
        this.GetLockerImage().gameObject.SetActive(false);
        this.GetCoinImage().gameObject.SetActive(false);
        this.GetCointText().gameObject.SetActive(false);

        // assing the function to the delegate button
        this.AddEventListerer();
    }


    void AddEventListerer() => this.btn.onClick.AddListener(this.OnPressCharacterUnlocked);


}
