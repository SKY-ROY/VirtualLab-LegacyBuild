using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 3600f;
    public bool doSpin = false;
    public GameObject playerGraphics;

    private Rigidbody rb;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(doSpin)
        {
            playerGraphics.transform.Rotate(new Vector3(0f, spinSpeed * Time.deltaTime, 0f));
        }
    }
}
