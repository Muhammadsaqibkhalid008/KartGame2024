using Assets.Scripts;
using PowerslideKartPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomInputManager : MonoBehaviour
{
    Transform player;
    [SerializeField] Joystick joyStick;
    [SerializeField] UI_Data ui_data;

    [SerializeField] Transform[] arrowControls_ui = new Transform[0];
    [SerializeField] Transform[] joysticksControls_ui = new Transform[0];
    // in case if there are more control ui's then what we're expecting

    [SerializeField] GameObject decalPrefab;

    private InputManager inputManager;
    private bool projectDecal = false;


    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        player = go.transform;

        // get the inputManager component
        inputManager = GetComponent<InputManager>();
        this.CheckForControls();

        // we need to make some logics for udpating the control ui's in the scriptable database as well
        projectDecal = false;
    }

    public void CheckForControls()
    {
        switch (ui_data.isUsingArrows)
        {
            case true:
                // yes we're using arrows for kart controls
                // here we're basically activating the controls for the user to play
                Array.ForEach(this.joysticksControls_ui, ui => ui.gameObject.SetActive(false));
                Array.ForEach(this.arrowControls_ui, ui => ui.gameObject.SetActive(true));
                break;
            case false:
                // yes we're using joystick for kart controls
                // here we're basically activating the controls for the user to play
                Array.ForEach(this.joysticksControls_ui, ui => ui.gameObject.SetActive(true));
                Array.ForEach(this.arrowControls_ui, ui => ui.gameObject.SetActive(false));
                break;
        }
    }
    public void OnPressShoot()
    {

        var script = player.GetComponent<MysteriousBoxEffector>();
        if (script.currentAsset != null)
        {
            // we need to launch the attack by calling another method from the player's script
            script.LaunchAttack();
        }
        else return;
    }

    private void Update()
    {

        if (this.joyStick.gameObject.activeSelf)
        {
            // if the joystick is in use by the user
            inputManager.SetSteerMobile(joyStick.Horizontal);
        }

        if (projectDecal)
        {
            // when the user is pressing the brakes, we need to project the decals on the surface of the floor

            // getting the floor located under the player
            RaycastHit hitInfo;
            Ray ray = new(player.transform.position, -player.transform.up);
            if (Physics.Raycast(ray, out hitInfo, 50f))
            {
                Debug.Log("raycast hitting the floor " + hitInfo.collider.gameObject.name);

                // working, now we need to project the decalPrefab on the floor
                var prefab = Instantiate(decalPrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
                prefab.AddComponent<DestroyerScript>().CallDestroyMethod(5f);
            }
        }
    }

    public void ProjectDriftDeccals(bool value)
    {
        projectDecal = value;
    }
}
