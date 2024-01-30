using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

public class PlayerTeleportation : MonoBehaviour
{

    public VRTeleporter teleporter;
    private bool isTeleportActive = false;
    //[SerializeField] private OVRInput.Axis2D thumbstick = OVRInput.Button.SecondaryThumbstickLeft;

    // Update is called once per frame
    void Update()
    {
        //textMeshResult.text = OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp).ToString();
        if (!isTeleportActive && OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
        {
            teleporter.ToggleDisplay(true);
            isTeleportActive = true;
        }
        else if (isTeleportActive && !OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
        {
            teleporter.Teleport();
            teleporter.ToggleDisplay(false);
            isTeleportActive = false;
        }
    }

}
