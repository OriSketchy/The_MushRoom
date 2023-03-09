using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private void Awake()
    {
        /// get y coordinates of head
        /// set Centre trigger at x:0 y:(initial headposition) z:0
        /// Calculate height of player based on position of head and position of floor (which will have been callibrated when first callibrating the headset)
        /// Set other trigger zones to appropriate positions based on player height (shorter people closer, taller people further (this will require maths)
        /// 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
