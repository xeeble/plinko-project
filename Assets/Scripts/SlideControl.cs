using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlideControl : MonoBehaviour, IPointerUpHandler
{
    public Slider slider;
    float oldValue;

    void Start()
    {
        slider = GetComponent<Slider>();
        oldValue = slider.value;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (slider.value != 0)
        {
            Debug.Log("Slider value changed from " + oldValue + " to " + slider.value);
            oldValue = slider.value;
            slider.value = 0;
        }
    }
    
}
