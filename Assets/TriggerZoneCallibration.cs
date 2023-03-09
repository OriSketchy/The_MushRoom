using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class TriggerZoneCallibration : MonoBehaviour
    {
        private void Awake()
        {
            //detect when the headset has been removed then replaced on the user's head. Once the user is present again, call InputTracking.Recenter().
            SteamVR_Input.GetBooleanAction("HeadsetOnHead");
            Debug.Log("Headset Active");

            
            //Debug.Log("Headset Inactive");
            /// When callibrate button held for 3 seconds get y coordinates of head
            /// set Centre trigger at x:0 y:(initial head position) z:0
            /// Calculate height of player based on position of head and position of floor (which will have been callibrated when first callibrating the headset)
            /// Set other trigger zones to appropriate positions based on player height (shorter people closer, taller people further (this will require maths)
            /// For now just have a debuglog tell you what trigger youre triggering
        }
    }
}