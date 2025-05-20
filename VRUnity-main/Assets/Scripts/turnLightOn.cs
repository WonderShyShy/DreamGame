using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnLightOn : MonoBehaviour
{
    private Light thisLight;
    public float brightness;
    public float timeToTurnOn; // Use float for more precise timing
    public GameObject move;

    private moveToMatchHead moveComp;

    private bool lightStart;

    // Start is called before the first frame update
    void Start()
    {
        moveComp = move.GetComponent<moveToMatchHead>();
        thisLight = GetComponent<Light>();
        thisLight.intensity = 0;
        lightStart = false;
        gameObject.SetActive(false);

    }

    private void Update()
    {
        if(moveComp.moveComplete == true && lightStart == false)
        {

            StartCoroutine(GraduallyTurnOnLight());
            lightStart = true;
        }
    }
    IEnumerator GraduallyTurnOnLight()
    {

        float elapsedTime = 0f;

        while (elapsedTime < timeToTurnOn)
        {
            // Lerp intensity based on elapsed time
            thisLight.intensity = Mathf.Lerp(0, brightness, elapsedTime / timeToTurnOn);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the intensity is set to the target brightness
        thisLight.intensity = brightness;
    }
}
