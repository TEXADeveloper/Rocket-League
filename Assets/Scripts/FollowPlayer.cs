using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float sensibility;
    private float xRotation;
    private float yRotation;

    void Update()
    {
        this.transform.position = playerTransform.position;

        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        float axisY = inputY * sensibility * Time.deltaTime;
        xRotation -= axisY;
        float axisX = inputX * sensibility * Time.deltaTime;
        yRotation -= axisX;
        
        this.transform.localRotation = Quaternion.Euler(-xRotation, -yRotation, 0);
    }
}
