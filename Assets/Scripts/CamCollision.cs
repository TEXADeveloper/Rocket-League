using UnityEngine;

public class CamCollision : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private float defaultCamDistance;
    [SerializeField] private LayerMask mask;

    void FixedUpdate()
    {
        Debug.Log(parentTransform.position + " " + transform.position);
        RaycastHit[] hit = Physics.RaycastAll(parentTransform.position, transform.position, defaultCamDistance, mask);
        if (hit.Length > 0)
            transform.position = new Vector3(transform.position.x, transform.position.y, hit[0].point.z);
        /* else
            transform.position = new Vector3(transform.position.x, transform.position.y, parentTransform.position.z - defaultCamDistance);*/
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawLine(parentTransform.position, transform.position * defaultCamDistance, Color.green);
    }
}
