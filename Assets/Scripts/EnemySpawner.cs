using PowerslideKartPhysics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    internal class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] enemykartPrefabs = new Transform[0];
        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private int minEnemies;
        [SerializeField] private int maxEnemies;
        public BasicWaypoint mainPlayerWayPointTransform = null;


        private List<Transform> enemies;
        public List<Transform> GetAllEnemies() => this.enemies;

        private void Awake()
        {
          //  Time.timeScale = 1;
         }


        private void Start()
        {
            enemies = new List<Transform>();
            var mainPlayer = GameObject.FindGameObjectWithTag("Player");
           mainPlayerWayPointTransform = null;
            bool success = false;
            foreach (var item in mainPlayer.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject.TryGetComponent<BasicWaypoint>(out BasicWaypoint bw))
                {
                    success = true;
                    mainPlayerWayPointTransform = bw;
                    Debug.Log("Basic waypoint found " + mainPlayerWayPointTransform.name);
                    break;
                }
            }

            // .. change in previous code and modifications STARTED

            // get all the child basic waypoints of the kart
            Transform[] mainPlayerChildren = mainPlayer.GetComponentsInChildren<Transform>();
            int count = 0;
           
            foreach(var item in mainPlayerChildren)
            {
                if (item.GetComponent<BasicWaypoint>())
                {
                    // we have found our target
                    count++;
                }

            }

            BasicWaypoint[] bws = new BasicWaypoint[count];
            count = 0;

            foreach (var item in mainPlayerChildren)
            {
                if (item.GetComponent<BasicWaypoint>())
                {
                    // we have found our target
                    bws[count] = item.GetComponent<BasicWaypoint>();
                    count++;
                }

            }



            // ******  aswe have succeeded getting 5 of the waypoints for the enemies to not just attack from back but to left, right and the front as well
            // ******

            Debug.Log($"total basic waypoints are {count}");
            // .. change in previous code and modifications ENDED


            if (success)
            {
                for (int i = 0; i < Random.Range(minEnemies, spawnPositions.Length); i++)
                {

                    // generating random number of enemy kart prefabs for more dynamic gameplay
                    int enemyKartPrefabIndex = Random.Range(0, enemykartPrefabs.Length);

                    var clone = Instantiate(enemykartPrefabs[enemyKartPrefabIndex], transform.position, transform.rotation);
                    clone.transform.position = spawnPositions[i].transform.position;
                   // clone.GetComponent<BasicWaypointFollowerDrift>().targetPoint = mainPlayerWayPointTransform;
                    clone.GetComponent<BasicWaypointFollowerDrift>().targetPoint = bws[Random.Range(0,bws.Length-1)];

                 //   Debug.Log($"target point is {clone.GetComponent<BasicWaypointFollowerDrift>().targetPoint.gameObject.name}");

                    // clone.GetComponent<BasicWaypointFollowerDrift>().nextPoint=mainPlayerWayPointTransform;
                    clone.gameObject.SetActive(false);
                    clone.gameObject.SetActive(true);
                    enemies.Add((Transform)clone);
                }
                // in the end we need to add the enemy kart prefab as well, can't leave that bitch alone
                //enemies.Add(this.enemykartPrefab);
                foreach (var item in enemykartPrefabs)
                {
                    item.gameObject.SetActive(false);
                }
            }

            Debug.Log("total enemies spawned are " + enemies.Count);

            // getting their basic waypoint
            
        }

        // just for normal check
        private int totalEnemieSpawn = 0;

        private void Update()
        {
            // checking whether the timeScale is 0 or not
            Debug.Log($"time scale value {Time.timeScale}");
            Debug.Log($"total enemies are {enemies.Count}");
        }

    }

}
