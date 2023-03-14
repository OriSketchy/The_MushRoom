//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

namespace Valve.VR.Extras
{
    [RequireComponent(typeof(SteamVR_TrackedObject))]
    public class SteamVR_TestThrow : MonoBehaviour
    {
        public GameObject prefab;
        public GameObject innerPrefab;
        public GameObject outerPrefab;
        public Rigidbody attachPoint;
        public float angle = 0f;

        [SerializeField]
        [Range(0f, 0.2f)]
        private float minDistance = 0.1f;
        [SerializeField]
        [Range(0f, 1f)]
        private float maxDistance = 0.4f;
        [SerializeField]
        [Range(0, 10)]
        private float maxSpeed = 2f;

        public SteamVR_Action_Boolean spawn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

        SteamVR_Behaviour_Pose trackedObj;
        //FixedJoint joint;
        GameObject go;
        GameObject inner;
        GameObject outer;

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
                Quaternion rotation = Quaternion.FromToRotation(trackedObj.transform.up, Vector3.up);
                go.transform.rotation = rotation * trackedObj.transform.rotation;
                inner = GameObject.Instantiate(innerPrefab);
                outer = GameObject.Instantiate(outerPrefab);
                inner.transform.position = attachPoint.transform.position;
                outer.transform.position = attachPoint.transform.position;

                inner.transform.localScale = new Vector3(minDistance, minDistance, minDistance);
                outer.transform.localScale = new Vector3(maxDistance, maxDistance, maxDistance);
            }
            else if (go != null && spawn.GetStateUp(trackedObj.inputSource))
            {
                //GameObject go = joint.gameObject;
                //Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                Object.DestroyImmediate(go);
                Object.DestroyImmediate(inner);
                Object.DestroyImmediate(outer);
                go = null;
            
            }

            if (go != null)
            {
                Vector3 distance = trackedObj.transform.position - go.transform.position;
                //float speed = distance.magnitude;
                float range = maxDistance - minDistance;
                float speed = (Mathf.Clamp(distance.magnitude, minDistance, maxDistance) - minDistance) / range * maxSpeed * Time.fixedDeltaTime;
                //float speed = Mathf.Clamp(distance.magnitude, minDistance, maxDistance);
                Debug.Log(speed);
                Vector3 move = distance.normalized * speed;
                Debug.DrawLine(go.transform.position, go.transform.position + move);
                player.position += move;
                go.transform.position += move;
                inner.transform.position += move;
                outer.transform.position += move;
            }
        }

        private void OnDrawGizmos()
        {
            if (go != null)
            {
                Gizmos.color = new Color(0, 0, 1, 0.5f);
                Gizmos.DrawSphere(go.transform.position, minDistance);
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(go.transform.position, maxDistance);
            }
        }
    }
}