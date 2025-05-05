using UnityEngine;
using UnityEngine.UI;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;          //The point to orbit around
    public float radius = 5f;         //Distance from target
    public Slider CameraAngleSlider;

    public float currentAngle = 0f;

    private Vector3 orbitCenter;

    void Start()
    {
        orbitCenter = target.position;
    }

    void Update()
    {
        currentAngle = CameraAngleSlider.maxValue + CameraAngleSlider.minValue - CameraAngleSlider.value;


        //Clamp the angle between min and max
        currentAngle = Mathf.Clamp(currentAngle, CameraAngleSlider.minValue, CameraAngleSlider.maxValue);

        //Calculate new position
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            0f,                          // X stays at 0 for vertical-only orbit
            Mathf.Sin(radians) * radius, // Vertical movement (up/down)
            Mathf.Cos(radians) * radius  // Distance from target
        );

        //Update position and rotation
        transform.position = orbitCenter + offset;
        transform.LookAt(orbitCenter);
    }

}
