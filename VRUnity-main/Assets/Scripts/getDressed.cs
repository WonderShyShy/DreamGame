using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getDressed : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject trigger;
    public GameObject target;
    public bool on;

    private HighLight triggerHighLight;

    void Start()
    {
        triggerHighLight = trigger.GetComponent<HighLight>();
        on = triggerHighLight.isSelected;
        
    }

    // Update is called once per frame
    void Update()
    {
        //check state of on
        //if on show
        //if not hide

        if (triggerHighLight.isSelected)
        {
            Debug.Log("Activated");
            target.SetActive(true);
        }
        else
        {
            //Debug.Log("Deactivated");
            target.SetActive(false);
        }

        
    }
}
