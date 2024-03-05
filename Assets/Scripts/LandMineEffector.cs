using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LandMineEffector : MonoBehaviour
{
    [SerializeField] float landMineDamage = 2f;
    public bool dispatched = false;

    private void Start()
    {
        this.dispatched = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // when the land mine is triggerd with the enemy or the main player
        // get its child particle system first and instaniate it and then
        // destroy the landmine
        switch (other.gameObject.CompareTag("enemy"))
        {
            case true:

                // it means we have not made any attack, the enemy has simple collided with our original landmine
                string original_name = this.gameObject.name;
                var clone_2 = Instantiate(this.gameObject, this.transform.position, transform.rotation);
                clone_2.transform.SetParent(transform.parent, true);
                clone_2.gameObject.name = original_name;
                clone_2.transform.localScale = this.gameObject.transform.localScale;
                clone_2.transform.localPosition = this.gameObject.transform.localPosition;
                clone_2.gameObject.SetActive(false);
                if (transform.parent != null) transform.parent.gameObject.SetActive(false);


                // different from dispatched logic

                Debug.Log("below logic for damage");
                var ps = this.transform.GetChild(1);
                var clone = Instantiate(ps, ps.transform.position, ps.transform.rotation);
                var script = clone.AddComponent<DestroyerScript>();
                script.CallDestroyMethod(1f);
                other.GetComponent<EnemyAi>().Damage(this.landMineDamage);
                Destroy(this.gameObject);

                // one more thing for being at safe side, when the landmine gets in contact with the whether launched by the player
                // or not, it gets kaboom, set the current asset to null of the main player
                break;
        }

    }

    private void OnDestroy()
    {
        var gameStarter = GameObject.FindObjectOfType<GameStarter>();
        gameStarter.playerUI.gameObject.GetComponent<MysteriousBoxEffector>().currentAsset = null;
        Debug.Log("particle destroyed and the player UI name is " + gameStarter.playerUI.gameObject.name);
    }
}
