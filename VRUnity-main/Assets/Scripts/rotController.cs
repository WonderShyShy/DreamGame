using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotController : MonoBehaviour
{
    public Transform cameraTransform; // Assign your camera's transform here
    // Start is called before the first frame update

    public float normalizedRotation;
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        float rotationY = cameraTransform.eulerAngles.y;

        float normalizedRotation = (rotationY <= 180) ? (rotationY / 180f) * 100 : ((360 - rotationY) / 180f) * 100;

        //Debug.Log("Normalized Rotation (Y-axis): " + normalizedRotation);
    }
}
