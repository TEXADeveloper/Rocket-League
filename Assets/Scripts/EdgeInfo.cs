using UnityEngine;

[System.Serializable]
public class EdgeInfo
{
    [SerializeField] private string name;
    public WheelCollider LeftWheel;
    public Transform LeftWheelTransform;
    public WheelCollider RightWheel;
    public Transform RightWheelTransform;
    public bool Motor;
    public bool Steering;
}
