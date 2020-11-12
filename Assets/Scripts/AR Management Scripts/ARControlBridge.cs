using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARControlBridge : MonoBehaviour
{
    private GameObject cam;
    private PhotonView photonView;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    private void Start()
    {
            
    }

    // Update is called once per frame
    private void Update()
    {
           
    }

    private void FixedUpdate()
    {
        if(photonView.IsMine)
        {
            gameObject.transform.position = cam.transform.position;
        }
    }
}
