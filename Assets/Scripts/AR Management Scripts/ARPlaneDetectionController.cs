﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARPlaneDetectionController : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager;

    public TextMeshProUGUI informUIPanel_Text;
    public GameObject placeButton;
    public GameObject adjustButton;
    public GameObject searchForGameButton;
    public GameObject scaleSlider;

    public bool keepScaleSlider = true;

    private void Awake()
    {
        m_ARPlacementManager = GetComponent<ARPlacementManager>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        placeButton.SetActive(true);
        scaleSlider.SetActive(keepScaleSlider);
        
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);
    
        informUIPanel_Text.text = "Move the phone around to detect plane and place the Virtual-Object(s).";
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void SetPlaneActivation(bool value)
    {
        foreach(var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;

        SetPlaneActivation(false);

        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        searchForGameButton.SetActive(true);

        scaleSlider.SetActive(false);

        informUIPanel_Text.text = "Virtual-Object(s) placed, now search for 'Active Sessions' to join.";
    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;

        SetPlaneActivation(true);

        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);

        scaleSlider.SetActive(keepScaleSlider);

        informUIPanel_Text.text = "Move the phone around to detect plane and place the Virtual-Object(s).";
    }
}
