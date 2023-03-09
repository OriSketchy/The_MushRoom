//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

namespace Valve.VR.Extras
{
    [RequireComponent(typeof(SteamVR_TrackedObject))]
    public class SteamVR_TestThrow : MonoBehaviour
    {
        public GameObject prefab;
        public Rigidbody attachPoint;
        public float angle = 0f;

        public SteamVR_Action_Boolean spawn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

        SteamVR_Behaviour_Pose trackedObj;
        //FixedJoint joint;
        GameObject go;

        public Transform player;

        private void Awake()
        {
            trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
        }

        private void FixedUpdate()
        {
            if (go == null && spawn.GetStateDown(trackedObj.inputSource))
            {
                go = GameObject.Instantiate(prefab);
                go.transform.position = attachPoint.transform.position;
                go.transform.rotation = trackedObj.transform.rotation;

                //go.transform.rotation = trackedObj.transform.rotation;
                //joint = go.AddComponent<FixedJoint>();
                //joint.connectedBody = attachPoint;
            }
            else if (go != null && spawn.GetStateUp(trackedObj.inputSource))
            {
                //GameObject go = joint.gameObject;
                //Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                Object.DestroyImmediate(go);
                go = null;
                //Object.Destroy(go, 15.0f);

                // We should probably apply the offset between trackedObj.transform.position
                // and device.transform.pos to insert into the physics sim at the correct
                // location, however, we would then want to predict ahead the visual representation
                // by the same amount we are predicting our render poses.

                //Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
                //if (origin != null)
                //{
                //    rigidbody.velocity = origin.TransformVector(trackedObj.GetVelocity());
                //    rigidbody.angularVelocity = origin.TransformVector(trackedObj.GetAngularVelocity());
                //}
                //else
                //{
                //    rigidbody.velocity = trackedObj.GetVelocity();
                //    rigidbody.angularVelocity = trackedObj.GetAngularVelocity();
                //}

                //rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
            }

            if (go != null)
            {
                //Vector3 distance = go.transform.position - transform.position;
                //Debug.Log(distance.normalized);
            }
        }
    }
}