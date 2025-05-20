using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    Renderer r;
    public Color emissiveColor;
    public Color baseColor;
    public bool isSelected;
    public bool changeColor;
    public GameObject[] otherOptions;

    private Color originalEmissive;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        r=GetComponentInChildren<Renderer>();
        if(r==null){
            Debug.LogError("couldn't find a renderer component among the children!");
        }

        if (r.material.HasProperty("_EmissionColor"))
        {
            originalEmissive = r.material.GetColor("_EmissionColor");
        }
        else
        {
            originalEmissive = new Color(0.0f, 0.0f, 0.0f); 
        }

        originalColor = r.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            Debug.Log("selected");
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", emissiveColor);
            if (changeColor)
            {
                SetMaterialColor(baseColor);
            }
        }
        
    }

    private void SetMaterialColor(Color baseCol)
    {
        if (r != null)
        {
            r.material.color = baseCol; // Set the base color
        }
    }

    public void Highlight()
    {
        SetMaterialColor(baseColor);
        r.material.EnableKeyword("_EMISSION");
        r.material.SetColor("_EmissionColor",emissiveColor);
        handleClick();
    }

    public void NoHighLight()
    {
        Debug.Log("no highlight");
        r.material.SetColor("_EmissionColor",originalEmissive);
        if (changeColor)
        {
            SetMaterialColor(originalColor);

        }
    }

    public void handleClick()
    {
        Debug.Log("handleClick");
        if (isSelected)
        {
            isSelected = false;
        }
        else
        {
            //check other options to see if selected
            if (otherOptions.Length <= 0)
            {
                isSelected = true;
            }

            else
            {
                int i = otherOptions.Length;
                int j = 0;
                bool noOthers = true;
                while (j < i)
                {
                    HighLight option = otherOptions[j].GetComponent<HighLight>();

                    if (option.isSelected)
                    {
                        noOthers = false;

                    }
                    j++;
                }
                if (noOthers)
                {
                    isSelected = true;
                }


            }

        }
    }
}
