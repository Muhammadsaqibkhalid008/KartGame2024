using Assets.PowerslideKartPhysics.Scripts;
using Assets.Scripts;
using Cinemachine;
using PowerslideKartPhysics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerUI : MonoBehaviour
{
    // this UI will basically control all the health related stuff
    [SerializeField] Image healthBar;
    [Range(0.1f, 1f)]
    [SerializeField] float damageFormEnemy = .2f;
    [SerializeField] float smoothTransitionValue = .5f;
    [SerializeField] ParticleSystem destroyEffect;
    [Tooltip("scriptable object for ui data")]
    [SerializeField] UI_Data ui_data;
   // [SerializeField] TMP_Text coinText;
    public Text CoinsText;
    [SerializeField] ParticleSystem coinCollectionEffect;

    [Header("settings for ui coin")]
    [Tooltip("Both of these texts are tagged as afterDeathUI")]
 //   [SerializeField] private TMP_Text totalCoinsCollected;
  //  [SerializeField] private TMP_Text totalEnemiesKilled;
    public Text TotalCoinsCollect;
    public Text TotalEnemiesKill;

    // private members
    private float reachValue = 0;
    private float currentSmoothVector;
    private MysteriousBoxEffector mysteriousBoxEffector;
    private NoiseSettings noiseSettings;
    private CinemachineVirtualCamera vcam;
    private CinemachineVirtualCamera vcam_2;
    public int totalCoins { get; set; }
    public bool playerDestroyed { get; set; }

    private GameStarter gameStarter;
    private Kart kart;



    private void Start()
    {
        // so the health doesn't get all the way to 0 on start of the match
        reachValue = healthBar.fillAmount;
        this.mysteriousBoxEffector = GetComponent<MysteriousBoxEffector>();

        coinCollectionEffect.Stop();
      //  coinText.text = "0";
        CoinsText.text = "0";
        totalCoins = 0;
        this.gameStarter = GameObject.FindObjectOfType<GameStarter>();
        kart = GetComponent<Kart>();
        ui_data.totalEnemiesKilled = 0;
    }

    public void ApplyDamage(float damageAmount)
    {
        reachValue = healthBar.fillAmount - damageAmount;

        if (healthBar.fillAmount <= 0f)
        {
            this.mysteriousBoxEffector.GetHighestPriorityCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = null;
            Invoke(nameof(DestroyPlayer), .2f);
            playerDestroyed = true;
            this.vcam_2.Priority = this.vcam.Priority + 1;
        }
        /*else
        {
            mysteriousBoxEffector.ApplyShake();
            kart.enabled = false;
        }*/
    }
    public void RecoverHealth(float recoverAmount)
    {
        reachValue = healthBar.fillAmount + recoverAmount;
    }

    private void Update()
    {
        healthBar.fillAmount = Mathf.SmoothDamp(healthBar.fillAmount, reachValue, ref currentSmoothVector, smoothTransitionValue);

        // check if all the enemies have been killed
        if (ui_data.totalEnemiesKilled == 6)
        {
            // game over
            this.gameStarter.DisplayGameOverPanel();
            saveLoadManager.SaveData();
        }
    }

    void DestroyPlayer() => Destroy(this.gameObject);
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            ApplyDamage(damageFormEnemy);
        }
        if (collision.gameObject.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
        {
            if (ps.TryGetComponent<ProjectileEffect>(out ProjectileEffect pe))
            {
                ApplyDamage(damageFormEnemy);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("coin"))
        {
            // we have triggered a coin
            Destroy(other.gameObject);

            // instantiate a coin collection effect as well
            var coinClone = Instantiate(this.coinCollectionEffect, other.transform.position, this.coinCollectionEffect.transform.rotation);
            var script = coinClone.AddComponent<DestroyerScript>();
            script.CallDestroyMethod(1f);

            // upadte the coin Ui
            totalCoins++;
        //    coinText.text = totalCoins.ToString();
            CoinsText.text = totalCoins.ToString();
        }
    }

    [SerializeField] SaveAndLoadManager saveLoadManager;

    private void OnDestroy()
    {
        var clone = Instantiate(destroyEffect, this.transform.position, this.transform.rotation);
        var script = clone.AddComponent<DestroyerScript>();
        script.CallDestroyMethod(1f);

        // update the original ui coin score when the game ends
        ui_data.totalCoins += this.totalCoins;
        this.gameStarter.DisplayGameOverPanel();

       // this.totalCoinsCollected.text = "Total coins collected " + this.totalCoins.ToString();
        this.TotalCoinsCollect.text = "Total coins collected " + this.totalCoins.ToString();
      //  this.totalEnemiesKilled.text = "Total enemies killed " + ui_data.totalEnemiesKilled.ToString();
        this.TotalEnemiesKill.text = "Total enemies killed " + ui_data.totalEnemiesKilled.ToString();
        saveLoadManager.SaveData();
    }
}
