using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileEffect : MonoBehaviour
{
    [Range(0, 1.5f)]
    [SerializeField] private float damageAmount;
    [Tooltip("if you're applying this to a rocket launcher then this should be true, as rocket launcher will damage more than normal machine guns")]
    [SerializeField] private bool blastOnOneHit;
    [SerializeField] private ParticleSystem[] onHitExplosionEffects;
    [SerializeField] private float destroyTimeDelay = 2f;

    private void Start()
    {
        // stopping all the effects at once in the start method
        Array.ForEach(onHitExplosionEffects, effect => effect.Stop());
    }

    private void OnParticleCollision(GameObject other)
    {
        switch (other.tag)
        {
            
            case "enemy":
                // getting the collision contact point
                Vector3 collisionPoint = other.GetComponent<BoxCollider>().ClosestPointOnBounds(this.transform.position);

                other.GetComponent<IDamageable>().Damage(this.damageAmount, true);
                var enemyPosition = other.transform.position;
                int randomNumber = Random.Range(0, onHitExplosionEffects.Length);
                var indexParticle = onHitExplosionEffects[randomNumber];
                var particleClone = Instantiate(indexParticle, enemyPosition, Quaternion.identity);
                var script = particleClone.gameObject.AddComponent<DestroyerScript>();
                script.CallDestroyMethod(this.destroyTimeDelay);
                particleClone.Play();
                 
                break;
            case "Player":
                var script_2 = other.GetComponent<PlayerUI>();
                script_2.ApplyDamage(this.damageAmount);
                break;
        }
    }
}

