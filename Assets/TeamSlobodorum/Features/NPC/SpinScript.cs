using UnityEngine;

public class SpinY : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second.")]
    public float spinSpeed = 90f;
    
    [Tooltip("True rotates around the world Y axis. False rotates around the object's local Y axis.")]
    public bool useWorldSpace = false;

    void Update()
    {
        // Time.deltaTime is mandatory to ensure the spin is frame-rate independent
        Space space = useWorldSpace ? Space.World : Space.Self;
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, space);
    }
}