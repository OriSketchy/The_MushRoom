using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControl : MonoBehaviour
{
    public Transform player;
    public Transform centre;
    public Transform controller;

    public float speed = 1;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            Vector3 direction = transform.position - centre.position;
            Vector3 move = direction.normalized * speed * Time.deltaTime;
            player.position += move;
            controller.position += move;
        }
    }
}
