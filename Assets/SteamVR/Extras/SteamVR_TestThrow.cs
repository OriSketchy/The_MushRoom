//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Valve.VR.Extras
{
    [RequireComponent(typeof(SteamVR_TrackedObject))]
    public class SteamVR_TestThrow : MonoBehaviour
    {
        public GameObject prefab;
        public GameObject innerPrefab;
        public GameObject outerPrefab;
        public Rigidbody attachPoint;

        [SerializeField]
        private LayerMask walls = 0;

        [SerializeField]
        [Range(0f, 10f)]
        private float brakeRange = 10;

        [SerializeField]
        [Range(0f, 10f)]
        private float stopRange = 1;

        [SerializeField]
        [Range(0f, 0.2f)]
        private float minDistance = 0.1f;
        [SerializeField]
        [Range(0f, 1f)]
        private float maxDistance = 0.4f;
        [SerializeField]
        [Range(0, 100)]
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

                go.transform.SetParent(player);
                inner.transform.SetParent(player);
                outer.transform.SetParent(player);


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
                //Debug.Log(speed);

                float brake = 1;
                if (Physics.Raycast(go.transform.position, distance, out RaycastHit hitInfo, brakeRange, walls))
                {
                    brake = Mathf.Max(0, hitInfo.distance - stopRange) / (brakeRange - stopRange);
                }
                speed *= brake;

                Vector3 move = distance.normalized * speed;
                Debug.DrawLine(go.transform.position, go.transform.position + move * 10);
                player.position += move;

            }
        }

        private void OnDrawGizmos()
        {
            float sizeX = Valve.VR.InteractionSystem.ChaperoneInfo.instance.playAreaSizeX;
            float sizeY = 2f;
            float sizeZ = Valve.VR.InteractionSystem.ChaperoneInfo.instance.playAreaSizeZ;

            Gizmos.color = new Color(1, 1, 0, 0.5f);
            Gizmos.DrawCube(player.position, new Vector3(sizeX, sizeY, sizeZ));

            if (go != null)
            {
                Vector3 distance = trackedObj.transform.position - go.transform.position;
                Vector3 direction = distance.normalized;

                //Gizmos.color = new Color(0, 0, 1, 0.5f);
                //Gizmos.DrawSphere(go.transform.position, minDistance);
                //Gizmos.color = new Color(1, 0, 0, 0.5f);
                //Gizmos.DrawSphere(go.transform.position, maxDistance);

                if (Physics.Raycast(go.transform.position, direction, out RaycastHit hitInfo, brakeRange, walls))
                {
                    Debug.Log("hit: " + hitInfo.distance);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(go.transform.position, direction * hitInfo.distance);
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(go.transform.position + direction * hitInfo.distance, direction * (brakeRange - hitInfo.distance));
                }
                else
                {
                    Debug.Log("miss");
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(go.transform.position, direction * brakeRange);
                }
            }
        }
    }
}