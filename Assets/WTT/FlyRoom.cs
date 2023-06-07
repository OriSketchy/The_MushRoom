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
    Collider room;

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

    // Update is called once per frame
    void Update()
    {

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
        var quad = new Valve.VR.HmdQuad_t();
        if (!playArea || !Valve.VR.SteamVR_PlayArea.GetBounds(playArea.size, ref quad))
            return;

        Vector3 corner0 = new Vector3(quad.vCorners0.v0, quad.vCorners0.v1, quad.vCorners0.v2);
        Vector3 corner1 = new Vector3(quad.vCorners1.v0, quad.vCorners1.v1, quad.vCorners1.v2);
        Vector3 corner2 = new Vector3(quad.vCorners2.v0, quad.vCorners2.v1, quad.vCorners2.v2);
        Vector3 corner3 = new Vector3(quad.vCorners3.v0, quad.vCorners3.v1, quad.vCorners3.v2);

        float minX = Mathf.Min(quad.vCorners0.v0, quad.vCorners1.v0, quad.vCorners2.v0, quad.vCorners3.v0);
        float maxX = Mathf.Max(quad.vCorners0.v0, quad.vCorners1.v0, quad.vCorners2.v0, quad.vCorners3.v0);
        float minZ = Mathf.Min(quad.vCorners0.v2, quad.vCorners1.v2, quad.vCorners2.v2, quad.vCorners3.v2);
        float maxZ = Mathf.Max(quad.vCorners0.v2, quad.vCorners1.v2, quad.vCorners2.v2, quad.vCorners3.v2);

        Bounds bounds = new Bounds();
        bounds.min = new Vector3(minX, 0, minZ);
        bounds.max = new Vector3(maxX, playArea.wireframeHeight, maxZ);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.TransformPoint(corner0), transform.TransformPoint(corner2));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.TransformPoint(corner1), transform.TransformPoint(corner3));
        //Gizmos.DrawCube(transform.TransformPoint(bounds.center), bounds.size);


        if (go != null)
        {
            Vector3 distance = controllerPos - go.transform.position;
            Vector3 direction = distance.normalized;

            //Gizmos.color = new Color(0, 0, 1, 0.5f);
            //Gizmos.DrawSphere(go.transform.position, minDistance);
            //Gizmos.color = new Color(1, 0, 0, 0.5f);
            //Gizmos.DrawSphere(go.transform.position, maxDistance);

            if (Physics.Raycast(go.transform.position, direction, out RaycastHit hitInfo, brakeRange, walls))
            {
                //Debug.Log("hit: " + hitInfo.distance);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(go.transform.position, direction * hitInfo.distance);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(go.transform.position + direction * hitInfo.distance, direction * (brakeRange - hitInfo.distance));
            }
            else
            {
                //Debug.Log("miss");
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(go.transform.position, direction * brakeRange);
            }
        }
    }
}
