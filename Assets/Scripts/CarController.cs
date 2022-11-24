using UnityEngine;
using System.Collections.Generic;

public class CarController : MonoBehaviour {
    [SerializeField] private List<EdgeInfo> edgeInfos; 
    [SerializeField] private float maxMotorTorque;
    [SerializeField] private float maxSteeringAngle;
     
    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
     
        foreach (EdgeInfo axleInfo in edgeInfos) {
            if (axleInfo.Steering) {
                axleInfo.LeftWheel.steerAngle = steering;
                axleInfo.RightWheel.steerAngle = steering;
            }
            if (axleInfo.Motor) {
                axleInfo.LeftWheel.motorTorque = motor;
                axleInfo.RightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.LeftWheel, axleInfo.LeftWheelTransform);
            ApplyLocalPositionToVisuals(axleInfo.RightWheel, axleInfo.RightWheelTransform);
        }
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform wheelTransform)
    {     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}
