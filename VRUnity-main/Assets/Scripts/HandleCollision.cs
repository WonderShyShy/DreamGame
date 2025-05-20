using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCollision : MonoBehaviour
{
    float timeSinceLastSound=-1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(timeSinceLastSound>=0)
            timeSinceLastSound+=Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision between floor and " + collision.gameObject.name);

        if(timeSinceLastSound==-1 || timeSinceLastSound>0.1){
            timeSinceLastSound=0.0f;
            GetComponent<AudioSource>().Play();
        }
    }
}
