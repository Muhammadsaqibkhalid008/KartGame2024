using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Cinemachine;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using Assets.Scripts;

public class MysteriousBoxEffector : MonoBehaviour
{
    [SerializeField] float mysteriousBoxDetectionRadius;
    [SerializeField] LayerMask mysteriousBoxLayermask;
    [SerializeField] Transform mysteriousBoxGizmosTransform;
    [Tooltip("the bottom child of the rotator, so we can easily manage asset selection by matchmaking")]
    [SerializeField] Transform _Assets;
    [Tooltip("all prefabs which will be randomly assigned when the player shoots with the rocket launcher")]
    [SerializeField] Transform[] rocketLauncherPrefabs = new Transform[0];
    [SerializeField] Transform[] machineGunPrefabs = new Transform[0];
    [SerializeField] NoiseSettings shakeNoiseSettingsAsset;
    [SerializeField] float shakeDuration;
    [Header("Settings for automated assets, (machine gun / rocket launcher")]
    [Tooltip("if you make it true, the assets(guns) will rotate towards the enemy on their own when they're near")]
    [SerializeField] bool automateAssets;
    [SerializeField] float assetAutomationThreshold;
    [SerializeField] LayerMask assetAutomationLayermask;
    [SerializeField] float machineGunBulletsDuration = .5f;
    [SerializeField] int maxMachineGunBullets = 7;

    private Transform mysteriousBoxTransform;
    private CinemachineVirtualCamera vcam;
    private Transform nearestEnemyDetecetdForAutomationAsset;
    public Transform currentAsset;   // ok so this asset is just a prefab so you can't handle it most probably
    private Transform currentHoldingAsset;
    private Quaternion currentHoldingAssetDefaultRotation;

    public bool isHoldingAssetRightNow = false;

    private void Start()
    {
        this.vcam = GetHighestPriorityCamera();
        this.vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = null;

        // turning off karts assets and their children as well
        /*_Assets.gameObject.SetActive(false);
        foreach (var item in _Assets.GetComponentsInChildren<Transform>(includeInactive: true))
        {
            item.gameObject.SetActive(false);
        }*/
    }

    public CinemachineVirtualCamera GetHighestPriorityCamera()
    {
        var s = GameObject.FindGameObjectWithTag("mainVcam").transform.GetComponent<CinemachineVirtualCamera>();
        Debug.Log($"got it {s.name}");
        return GameObject.FindGameObjectWithTag("mainVcam").transform.GetComponent<CinemachineVirtualCamera>();
    }


    [SerializeField] float rotationLerpTime = 1f;
    private float timer = 0f;

    void RotateCurrentAssetTowardsEnemy(Transform asset, Transform enemy)
    {
        // rest of the code .... //
    }

    bool NearestEnemyDetected()
    {
        // get the enemies first
        Collider[] hits = Physics.OverlapSphere(transform.position, assetAutomationThreshold, assetAutomationLayermask);

        // check if there is only one enemy in the range
        if (hits.Length == 1 && hits != null)
        {
            // obtain the enemy transform
            nearestEnemyDetecetdForAutomationAsset = hits[0].transform;

            return true;
        }
        return false;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mysteriousBoxGizmosTransform.position, mysteriousBoxDetectionRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, assetAutomationThreshold);
    }


    public void ApplyShake() => StartCoroutine(ShakeCoroutune());
    public void StopShake() => StopCoroutine(ShakeCoroutune());
    IEnumerator ShakeCoroutune()
    {
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = this.shakeNoiseSettingsAsset;
        yield return new WaitForSeconds(shakeDuration);
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = null;
    }

    #region OldCode
    public void LaunchAttack()
    {
        if (this.currentAsset == null) return;
        //if (isHoldingAssetRightNow == false) return;
        if (this.currentAsset.gameObject.name == "machineGun")
        {
            Transform firePoint = null;
            Transform machineGun = null;
            ParticleSystem spark = null;
            foreach (var item in this._Assets.GetComponentsInChildren<Transform>(includeInactive: true))
            {
                // getting the initial children of assets
                if (item.name == "machineGun")
                {
                    machineGun = item;

                    // now getting the children furhter to get the fire point for the projectile
                    foreach (var item2 in item.GetComponentsInChildren<Transform>(includeInactive: true))
                    {
                        if (item2.name == "firePoint")
                        {
                            firePoint = item2;
                        }
                        if (item2.name == "sparkEffect")
                        {
                            spark = item2.GetComponent<ParticleSystem>();
                        }
                    }
                }
            }

            // coroutine for shooting machine gun bullets
            StartCoroutine(MachineGunCoroutine(machineGunPrefabs, firePoint, spark, machineGun));
        }

        IEnumerator MachineGunCoroutine(Transform[] prefabs, Transform _firePoint, ParticleSystem _spark, Transform _machineGun)
        {
            Debug.Log($"machine gun unloading coroutine");
            // we need to get the random prefab first and use that as the machine gun
            int randomPrefab = Random.Range(0, prefabs.Length);
            int totalBullets = maxMachineGunBullets;
            //
            while (this.currentAsset != null || this.currentHoldingAsset != null)
            {
                var projectile = Instantiate(machineGunPrefabs[randomPrefab], _firePoint.position, transform.rotation);
                projectile.transform.forward = _firePoint.transform.forward;
                var script = projectile.AddComponent<DestroyerScript>();
                script.CallDestroyMethod(2.5f);
                 totalBullets--;
                // StartCoroutine(ShakeCoroutune());
                yield return new WaitForSeconds(machineGunBulletsDuration);

                // checking if there are no more bullets
                if (totalBullets <= 0)
                {
                    Debug.Log($"bullets over");
                    isHoldingAssetRightNow = false;
                    //this.currentAsset = this.currentHoldingAsset = null;
                    break;
                }
            }
            this.currentAsset = this.currentHoldingAsset = null;
            // play the spark effect as well
            _spark.Play();

            // play cinemachine shake effect
            // StartCoroutine(ShakeCoroutune());

            Debug.Log("Spark is " + _spark.gameObject.name);

            this.currentAsset = null;
            isHoldingAssetRightNow = false;
            this.currentHoldingAsset = null;

            StartCoroutine(UnloadAsset(_machineGun));
            yield return null;
        }

        if (this.currentAsset.gameObject.name == "rocketLauncher")
        {
            // when we'll launch the attack, there is no more current asset available unless we get another mysterious box
            Transform firePoint = null;
            Transform rocketLauncher = null;
            ParticleSystem spark = null;
            foreach (var item in this._Assets.GetComponentsInChildren<Transform>(includeInactive: true))
            {
                // getting the initial children of the assets
                if (item.name == "rocketLauncher")
                {
                    rocketLauncher = item;
                    // now getting the children further to get the fire point for the projectile
                    foreach (var item2 in item.GetComponentsInChildren<Transform>(includeInactive: true))
                    {
                        if (item2.name == "firePoint")
                        {
                            firePoint = item2;
                            Debug.Log($"got the firePoint {firePoint.name}");
                            //break;
                        }
                        if (item2.name == "sparkEffect")
                        {
                            spark = item2.GetComponent<ParticleSystem>();
                        }


                    }
                }
            }
            int randomPrefab = Random.Range(0, rocketLauncherPrefabs.Length);
            var projectile = Instantiate(rocketLauncherPrefabs[randomPrefab], firePoint.position, transform.rotation);
            projectile.transform.forward = firePoint.transform.forward;

            // play the spark effect as well
            spark.Play();

            // play cinemachine shake effect
            // StartCoroutine(ShakeCoroutune());

            Debug.Log("Spark is " + spark.gameObject.name);

            this.currentAsset = null;
            this.currentHoldingAsset = null;

            StartCoroutine(UnloadAsset(rocketLauncher));
        }


        if (this.currentAsset.gameObject.name == "landMine")
        {
            // we need to place the land mine and get it sticked with the surface
            Transform[] children = this._Assets.GetComponentsInChildren<Transform>(includeInactive: true);
            Transform landMine = null;
            foreach (var item in children)
            {
                if (item.name == "landMine")
                {
                    landMine = item;
                    currentHoldingAsset = item;
                    currentHoldingAssetDefaultRotation = currentHoldingAsset.transform.localRotation;
                    break;
                }
            }

            var new_clone = Instantiate(landMine, landMine.transform.position, landMine.transform.rotation);
            if (new_clone.TryGetComponent<LandMineEffector>(out LandMineEffector lme))
            {
                lme.dispatched = true;
            }
            // new_clone.AddComponent<DestroyerScript>().CallDestroyMethod(2f)
            landMine.gameObject.SetActive(false);
            this._Assets.gameObject.SetActive(false);
            this.currentAsset = null;
            isHoldingAssetRightNow = false;
        }
    }
    IEnumerator UnloadAsset(Transform tr)
    {
        Debug.Log($"machine gun unloading");
        // as we have launched our attack, we need to remove the asset (rocketLauncher etc) now
        var animator = tr.GetChild(0).GetComponent<Animator>();
        animator.CrossFade("unload", .1f);
        yield return new WaitForSeconds(.25f);
        tr.gameObject.SetActive(false);
        isHoldingAssetRightNow = false;
        this.currentAsset = null;
        this.currentHoldingAsset = null;

    }
    public void InitiateAsset()
    {
        if (this.currentAsset == null) return;
        // if (isHoldingAssetRightNow) return;
        if (this.currentAsset.gameObject.name == "machineGun")
        {
            isHoldingAssetRightNow = true;
            // now we need to get the asset transform and enable its animation first
            Transform[] children = _Assets.GetComponentsInChildren<Transform>(includeInactive: true);
            Transform machineGun = null;
            foreach (var item in children)
            {
                if (item.name == "machineGun")
                {
                    machineGun = item;
                    currentHoldingAsset = item;
                    currentHoldingAssetDefaultRotation = currentHoldingAsset.transform.localRotation;
                    break;
                }
            }
            // settings these assets active
            this._Assets.gameObject.SetActive(true);
            machineGun.gameObject.SetActive(true);
            machineGun.GetChild(0).GetComponent<Animator>().CrossFade("load", .1f);
        }
        if (this.currentAsset.gameObject.name == "rocketLauncher")
        {
            isHoldingAssetRightNow = true;
            // now we need to get the Assets Transform and enable its animation first
            Transform[] children = _Assets.GetComponentsInChildren<Transform>(includeInactive: true);
            Transform rocketLauncher = null;
            foreach (var item in children)
            {
                Debug.Log("Looping " + item.name);
                if (item.name == "rocketLauncher")
                {
                    rocketLauncher = item;
                    currentHoldingAsset = item;
                    currentHoldingAssetDefaultRotation = currentHoldingAsset.transform.localRotation;
                    break;
                }
            }

            // settings these assets active
            this._Assets.gameObject.SetActive(true);
            rocketLauncher.gameObject.SetActive(true);
            rocketLauncher.GetChild(0).GetComponent<Animator>().CrossFade("load", .1f);

        }
        if (this.currentAsset.gameObject.name == "landMine")
        {
            isHoldingAssetRightNow = true;
            // we need to place the land mine and get it sticked with the surface
            Transform[] children = this._Assets.GetComponentsInChildren<Transform>(includeInactive: true);
            Transform landMine = null;
            foreach (var item in children)
            {
                if (item.name == "landMine")
                {
                    landMine = item;
                    currentHoldingAsset = item;
                    currentHoldingAssetDefaultRotation = currentHoldingAsset.transform.localRotation;
                    break;
                }
            }

            // now we have obtained the land mine asset 
            // now place it on the surface
            this._Assets.gameObject.SetActive(true);
            landMine.gameObject.SetActive(true);
        }
    }
    public void CheckCurrentAsset() => Debug.Log($"current asset is {currentAsset}");
    #endregion


    #region NewCode

    public void InitiateItem(string itemName)
    {
        if (itemName == "machineGun")
        {
            Transform firePoint = null;
            Transform machineGun = null;
            ParticleSystem spark = null;
            foreach (var item in this._Assets.GetComponentsInChildren<Transform>(includeInactive: true))
            {
                // getting the initial children of assets
                if (item.name == "machineGun")
                {
                    machineGun = item;

                    // now getting the children furhter to get the fire point for the projectile
                    foreach (var item2 in item.GetComponentsInChildren<Transform>(includeInactive: true))
                    {
                        if (item2.name == "firePoint")
                        {
                            firePoint = item2;
                        }
                        if (item2.name == "sparkEffect")
                        {
                            spark = item2.GetComponent<ParticleSystem>();
                        }
                    }
                }
            }

            //now we have the machine gun and the spark particle effect as well
            //the firepoint as well
        }




    }


    #endregion
}
