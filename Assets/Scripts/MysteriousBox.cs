using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteriousBox : MonoBehaviour
{
    [SerializeField] private float timeoutSeconds;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioSource hitEffectAudio;
    [Tooltip("for example -> rocket launcher, machine guns, fire balls etc")]
    [SerializeField] private Transform[] assets = new Transform[0];
    [Range(0, 1.5f)]
    [SerializeField] private float recoverAmount = 2f;

    private Transform objectToHideOnHitByMainPlayer;
    public Transform holdingAsset;  // the asset this mysterious box is holding right now

    private void Start()
    {
        //objectToHideOnHitByMainPlayer = this.transform.GetChild(0).transform;
        // we need to hide the object completely so soon we get the power up
        // no other kart including ourselves or the enemy can get it 
        objectToHideOnHitByMainPlayer = this.transform;
        hitEffect.Stop();
        holdingAsset = assets[Random.Range(0, assets.Length)];
        Debug.Log($"mysterious box {this.gameObject.name} is holding {holdingAsset.name}");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("enemy"))
        {
            // make sure one thing, if this mysterious box has health magic, it should return
            if (holdingAsset.gameObject.CompareTag("healthRecovery"))
            {
                if (!other.gameObject.CompareTag("Player")) return;
                // code for health recovery of the player which has collided .....
                objectToHideOnHitByMainPlayer.gameObject.SetActive(false);
                hitEffect.Play();
                hitEffectAudio.Play();
                Invoke(nameof(MysteriousBoxTimeout), timeoutSeconds);
                var holdingAssetClone = Instantiate(holdingAsset, other.transform.position, transform.rotation);
                // holdingAssetClone.transform.SetParent(other.gameObject.transform);
                //holdingAssetClone.transform.localPosition = Vector3.zero;

                // get the player UI and recover the health as well
                var player_ui = other.GetComponent<PlayerUI>();
                player_ui.RecoverHealth(this.recoverAmount);

                return;
            }

            // if there's already an asset contained by the player, then it won't be destroyed
            //other.GetComponent<MysteriousBoxEffector>().currentAsset == null
            if (other.GetComponent<MysteriousBoxEffector>().currentAsset == null)
            {

                objectToHideOnHitByMainPlayer.gameObject.SetActive(false);
                hitEffect.Play();
                hitEffectAudio.Play();
                Invoke(nameof(MysteriousBoxTimeout), timeoutSeconds);

                var script = other.GetComponent<MysteriousBoxEffector>();
                script.currentAsset = this.holdingAsset;
                Debug.Log($"Mysterious box holding asset is {this.holdingAsset.gameObject.name}");

                // this step is just for debugging
                script.InitiateAsset();

                // this step is just for debugging
                //script.CheckCurrentAsset();  // working
            }

            //conditions for new code
            return;
            if(!other.GetComponent<MysteriousBoxEffector>().isHoldingAssetRightNow)
            {
                //so if he's not holding we need to assign the given asset to him contained in this mysterious box
                objectToHideOnHitByMainPlayer.gameObject.SetActive(false);
                hitEffect.Play();
                hitEffectAudio.Play();
                Invoke(nameof(MysteriousBoxTimeout), timeoutSeconds);

                var script = other.GetComponent<MysteriousBoxEffector>();
                script.InitiateItem($"{holdingAsset.name}");

                // this step is just for debugging
                //script.CheckCurrentAsset();  // working
            }
        }
    }

    void MysteriousBoxTimeout()
    {
        objectToHideOnHitByMainPlayer.gameObject.SetActive(true);
    }

}
