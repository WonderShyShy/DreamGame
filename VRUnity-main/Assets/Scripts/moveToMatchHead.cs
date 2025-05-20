using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToMatchHead : MonoBehaviour
{
    public bool moveComplete = false;

    public GameObject headObject;
    public float timeUntilMove = 5.0f;

    public GameObject startUI;
    public GameObject[] lights;
    public GameObject[] invisible;
    public AudioClip music;
    private AudioSource audioSource;

    public float offset;


    // Start is called before the first frame update
    void Start()
    {
        startUI.SetActive(true);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false; // Disable visibility for all renderers
        }
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveComplete)
        {
            startUI.SetActive(false);
            foreach (GameObject light in lights)
            {
      
                light.SetActive(true);
            }
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true; // Disable visibility for all renderers
            }
            foreach (GameObject offObject in invisible)
            {
                Renderer[] offRenderers = offObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in offRenderers)
                {
                    renderer.enabled = false; // Disable visibility for renderers in invisible objects
                }
            }
            return; //we are done, no need to do anything else!
        }

        if (Time.time > timeUntilMove) //after some seconds lets move objects!
        {
            Debug.Log("we are moving the world to match the users head NOW!");

            Vector3 newPos = this.headObject.transform.position;
            newPos.y = 0+offset; //don't adjust for y (height)

            this.transform.position = newPos; //set the position of the parent that holds all the environmental objects. 

            moveComplete = true;
            if (music != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("No AudioClip assigned to 'music'!");
            }

        }

    }
}

