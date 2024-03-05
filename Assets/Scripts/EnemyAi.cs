using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    internal class EnemyAi : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject healthUI;
        [SerializeField] private float yieldTime = .5f;
        [SerializeField] private ParticleSystem effectOnDestroy;

        [Header("raycast settings for detecting and shooting at the main player")]
        [SerializeField] private float rayCastMaxDistance = 2f;
        [SerializeField] private LayerMask rayCastLayermask;
        [SerializeField] private Vector3 rayCastPositionModifier;  // will be added to the current position for adjustment
        [SerializeField] private Transform rayCastTransform;
        [Tooltip("The transform/gameObject that contains all the gameobjects")]
        [SerializeField] private Transform coinHolder;
        [Header("scripatble object for counitng enemy kills")]
        [SerializeField] private UI_Data ui_data;

        private Image sr;
        private bool isDamaging;
        private float reachValue;
        private float x;
        private MysteriousBoxEffector effector;
        private bool attackStarted = false;
        private Transform[] coins;

        private void Start()
        {
            // this is because we're storing data in a scriptable object so we need to make default scores at the start as well
            ui_data.totalEnemiesKilled = 0;


            isDamaging = false;
            sr = healthUI.GetComponent<Image>();
            effector = GetComponent<MysteriousBoxEffector>();
            attackStarted = false;


            // getting all the coins
            this.coins = this.coinHolder.GetComponentsInChildren<Transform>(includeInactive: true);
            this.coinHolder.gameObject.SetActive(false);


        }
        public void Damage(float damageAmount, bool oneShotBlast = true)
        {
            // we need to decrease the value of the health UI
            //sr.fillAmount -= damageAmount;
            reachValue = sr.fillAmount - damageAmount;
            x = damageAmount;
            StartCoroutine(SmoothDamageEffect());

        }
        IEnumerator SmoothDamageEffect()
        {
            isDamaging = true;
            yield return new WaitForSeconds(yieldTime);
            isDamaging = false;
        }

        private void Update()
        {
            if (isDamaging)
            {
                sr.fillAmount = Mathf.SmoothDamp(sr.fillAmount, reachValue, ref currentSmoothVector, smoothTransitionValue);
            }

            // we need to chek if the fillAmount is lesser than 0

            if (sr.fillAmount <= 0f)
            {
                Destroy(this.gameObject);
            }

            // checking if the rayCast is detected or not
            switch (IsPlayerDetected() && !attackStarted)
            {
                case true:
                    Debug.Log("player detected");
                    StartCoroutine(AttackCroroutine());
                    break;
            }
        }

        private bool IsPlayerDetected()
        {
            var newPos = rayCastTransform.position + rayCastPositionModifier;
            return Physics.Raycast(newPos, rayCastTransform.forward, rayCastMaxDistance, rayCastLayermask);
        }

        IEnumerator AttackCroroutine()
        {
            attackStarted = true;
            effector.LaunchAttack();
            yield return new WaitForSeconds(2f);
            attackStarted = false;
        }
        private void OnDestroy()
        {
            var particleClone = Instantiate(effectOnDestroy, this.transform.position, transform.rotation);
            var script = particleClone.AddComponent<DestroyerScript>();
            script.CallDestroyMethod(1.5f);

            // just destroy random coins instead of spawning them with complex logics


            for (int i = 0; i < Random.Range(0, this.coins.Length); i++)
            {
                var clone = Instantiate(this.coins[i], this.coins[i].transform.position, this.coins[i].transform.rotation);
            }

            ui_data.totalEnemiesKilled += 1;


        }

        float currentVector;
        float currentSmoothVector;
        [SerializeField] private float smoothTransitionValue = .5f;


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(rayCastTransform.position + rayCastPositionModifier, rayCastTransform.forward * rayCastMaxDistance);
        }
    }
}
