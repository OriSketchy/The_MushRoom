using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyRoom : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;
    
    [SerializeField]
    GameObject innerPrefab;
    
    [SerializeField]
    GameObject outerPrefab;

    [SerializeField]
    LayerMask walls = 0;

    [SerializeField]
    [Range(0f, 10f)]
    float brakeRange = 10;

    [SerializeField]
    [Range(0f, 10f)]
    float stopRange = 1;

    [SerializeField]
    [Range(0f, 0.2f)]
    float minDistance = 0.1f;
    
    [SerializeField]
    [Range(0f, 1f)]
    float maxDistance = 0.4f;
    
    [SerializeField]
    [Range(0, 100)]
    float maxSpeed = 2f;
    
    [SerializeField]
    BoxCollider room;

    Valve.VR.SteamVR_PlayArea playArea = null;
    
    GameObject go;
    GameObject inner;
    GameObject outer;

    Vector3 controllerPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {

        playArea = gameObject.GetComponent<Valve.VR.SteamVR_PlayArea>();
    }

    private void FixedUpdate()
    {
        FindWalls();
    }

    private void FindWalls()
    {
        var quad = new Valve.VR.HmdQuad_t();
        if (!playArea || !Valve.VR.SteamVR_PlayArea.GetBounds(playArea.size, ref quad))
            return;

        float minX = Mathf.Min(quad.vCorners0.v0, quad.vCorners1.v0, quad.vCorners2.v0, quad.vCorners3.v0);
        float maxX = Mathf.Max(quad.vCorners0.v0, quad.vCorners1.v0, quad.vCorners2.v0, quad.vCorners3.v0);
        float minZ = Mathf.Min(quad.vCorners0.v2, quad.vCorners1.v2, quad.vCorners2.v2, quad.vCorners3.v2);
        float maxZ = Mathf.Max(quad.vCorners0.v2, quad.vCorners1.v2, quad.vCorners2.v2, quad.vCorners3.v2);

        Bounds bounds = new Bounds();
        bounds.min = new Vector3(minX, 0, minZ);
        bounds.max = new Vector3(maxX, playArea.wireframeHeight, maxZ);

        room.center = bounds.center;
        room.size = bounds.size;
    }

    public void Start(Transform controller)
    {
        End();

        controllerPos = controller.position;

        go = GameObject.Instantiate(prefab);
        go.transform.position = controller.position;
        Quaternion rotation = Quaternion.FromToRotation(controller.up, Vector3.up);
        go.transform.rotation = rotation * controller.rotation;
        inner = GameObject.Instantiate(innerPrefab);
        outer = GameObject.Instantiate(outerPrefab);
        inner.transform.position = controller.position;
        outer.transform.position = controller.position;

        inner.transform.localScale = new Vector3(minDistance, minDistance, minDistance);
        outer.transform.localScale = new Vector3(maxDistance, maxDistance, maxDistance);

        go.transform.SetParent(transform);
        inner.transform.SetParent(transform);
        outer.transform.SetParent(transform);
    }

    public void Fly(Transform controller)
    {
        if (!go)
            return;

        controllerPos = controller.position;

        Vector3 distance = controller.position - go.transform.position;
        float range = maxDistance - minDistance;
        float speed = (Mathf.Clamp(distance.magnitude, minDistance, maxDistance) - minDistance) / range * maxSpeed * Time.fixedDeltaTime;

        float brake = 1;
        if (Physics.Raycast(go.transform.position, distance, out RaycastHit hitInfo, brakeRange * 2, walls))
        {
            Vector3 closest = room.ClosestPoint(hitInfo.point);
            float wallDistance = (hitInfo.point - closest).magnitude;
            brake = Mathf.Max(0, wallDistance - stopRange) / (brakeRange - stopRange);
        }
        speed *= brake;

        Vector3 move = distance.normalized * speed;
        Debug.DrawLine(go.transform.position, go.transform.position + move * 10);
        transform.position += move;
    }

    public void End()
    {
        if (go)
        {
            Object.DestroyImmediate(go);
            go = null;
        }
        if (inner)
        {
            Object.DestroyImmediate(inner);
            inner = null;
        }
        if (outer)
        {
            Object.DestroyImmediate(outer);
            outer = null;
        }

        controllerPos = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (go != null)
        {
            Vector3 distance = controllerPos - go.transform.position;
            Vector3 direction = distance.normalized;
            Ray ray = new Ray(go.transform.position, direction);

            if (Physics.Raycast(go.transform.position, direction, out RaycastHit hitInfo, brakeRange * 2, walls))
            {
                Vector3 closest = room.ClosestPoint(hitInfo.point);

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(go.transform.position, closest);

                Vector3 brakeLine = hitInfo.point - closest;

                Vector3 stopEnd = closest + brakeLine.normalized * stopRange;
                Vector3 brakeEnd = closest + brakeLine.normalized * brakeRange;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(closest, stopEnd);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(stopEnd, brakeEnd);

                if ((brakeEnd - closest).magnitude < (hitInfo.point - closest).magnitude)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(brakeEnd, hitInfo.point);
                }

                //Debug.Log("hit: " + hitInfo.distance);
                Gizmos.color = Color.white;
                Gizmos.DrawRay(go.transform.position, direction * hitInfo.distance);
            }
            else
            {
                //Debug.Log("miss");
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(go.transform.position, direction * brakeRange * 2);
            }
        }
    }
}
