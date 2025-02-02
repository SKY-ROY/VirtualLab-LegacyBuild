﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    public Camera aRCamera;
    public GameObject battleArenaGameobject;

    private ARRaycastManager m_ARRaycastManager;
    private static List<ARRaycastHit> raycast_Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2);

        Ray ray = aRCamera.ScreenPointToRay(centerOfScreen);

        if(m_ARRaycastManager.Raycast(ray, raycast_Hits, TrackableType.PlaneWithinPolygon))
        {
            //Ray intersects with a plane
            Pose hitPose = raycast_Hits[0].pose;

            Vector3 positionToBePlaced = hitPose.position;

            battleArenaGameobject.transform.position = positionToBePlaced;
        }

    }
}
