using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class FlyController : MonoBehaviour
{
    [SerializeField]
    Transform attachPoint;

    [SerializeField]
    FlyRoom flyRoom;

    public SteamVR_Action_Boolean spawn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

    SteamVR_Behaviour_Pose trackedObj;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
    }

    private void FixedUpdate()
    {
        if (spawn.GetStateDown(trackedObj.inputSource))
        {
            flyRoom.Start(attachPoint);
        }
        else if (spawn.GetStateUp(trackedObj.inputSource))
        {
            flyRoom.End();
        }
        else if (spawn.GetState(trackedObj.inputSource))
        {
            flyRoom.Fly(attachPoint);
        }
    }
}
