using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ARScaleController : MonoBehaviour
{
    ARSessionOrigin m_ARSessionOrigin;
    public Slider scale_Slider;

    private void Awake()
    {
        m_ARSessionOrigin = GetComponent<ARSessionOrigin>();    
    }

    private void Start()
    {
        scale_Slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Update()
    {
        
    }

    public void OnSliderValueChanged(float value)
    {
        if (scale_Slider != null)
        {
            m_ARSessionOrigin.transform.localScale = Vector3.one / value;
        }
    }
}
