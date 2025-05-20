using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onStartSetUp : MonoBehaviour
{
    public GameObject headModel;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = headModel.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
