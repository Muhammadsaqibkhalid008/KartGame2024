using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MysteriousBoxSpawner : MonoBehaviour
{
    [SerializeField] Transform[] mysteriousBoxesPositions;
    [SerializeField] MysteriousBox[] mysteriousBoxes;
    [SerializeField] MysteriousBox mysteriousBoxPrefab;
    [SerializeField] string placesTagName;

    private void Start()
    {
        // saving all the positions
        GameObject[] positions = GameObject.FindGameObjectsWithTag(placesTagName);
        mysteriousBoxesPositions = new Transform[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            mysteriousBoxesPositions[i] = positions[i].transform;
        }

        // instantiationg all the mysterious boxes in the saved positions
        int totalPlaces = positions.Length;
        mysteriousBoxes = new MysteriousBox[totalPlaces];


        for (int i = 0; i < totalPlaces; i++)
        {
            var clone = Instantiate(mysteriousBoxPrefab, Vector3.zero, Quaternion.identity);
            clone.transform.position = mysteriousBoxesPositions[i].position;
            mysteriousBoxes[i] = clone;
        }
    }
}
