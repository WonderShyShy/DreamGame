using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverScaleEffect : MonoBehaviour
{
    private Vector3 originalScale;
    public float scaleFactor = 1.2f; // Factor by which the size increases (e.g., 1.2x)

    void Start()
    {
        // Store the original scale of the object
        originalScale = transform.localScale;
    }

    public void OnHoverEntered()
    {
        // Increase the size of the object
        transform.localScale = originalScale * scaleFactor;
    }

    public void OnHoverExited()
    {
        // Reset to the original scale
        transform.localScale = originalScale;
    }
}
