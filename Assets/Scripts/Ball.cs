using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float force;

    private void throwBall()
    {
        transform.position = spawnPos.position;
        rb.AddForce(0, force, 0, ForceMode.Force);
    }

    void Start()
    {
        Goal.Score += throwBall;
        throwBall();
    }

    void OnDisable()
    {
        Goal.Score -= throwBall;
    }
}
