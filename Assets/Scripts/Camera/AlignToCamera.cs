using UnityEngine;

public class AlignToCamera : MonoBehaviour
{
    private Camera cam;
    
    private void Awake()
    {
        cam = Camera.main;
        if (cam == null)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
