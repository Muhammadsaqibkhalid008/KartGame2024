using System.Collections;
using System.Collections.Generic;
using PowerslideKartPhysics;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    // we need to manually rotate the wheels of tha cart

    [Tooltip("How much can the wheel rotate while rotating the kart")]
    [SerializeField] float yRotationValue;
    [SerializeField] InputManager inputManager;
    [SerializeField] Transform[] wheels = new Transform[0];

    private void Start()
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();
    }

    private void Update()
    {
        if (inputManager.GetMobileSteer() == 1)
        {
            // kart is moving right

            foreach (var item in wheels)
            {
                item.transform.localRotation = Quaternion.Euler(item.transform.rotation.x, yRotationValue, item.transform.rotation.z);
            }
        }
        else if (inputManager.GetMobileSteer() == -1)
        {
            // kart is moving left
            foreach (var item in wheels)
            {
                item.transform.localRotation = Quaternion.Euler(item.transform.rotation.x, -yRotationValue, item.transform.rotation.z);
            }
        }
        else
        {
            foreach (var item in wheels)
            {
                item.transform.localRotation = Quaternion.Euler(item.transform.rotation.x, 0f, item.transform.rotation.z);
            }
        }

        Debug.Log($"steer value {inputManager.GetMobileSteer()}");
    }
}
