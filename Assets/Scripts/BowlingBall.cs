using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    public Vector3 force = new Vector3(0, 0, 500);
    public float initialSpeed = 10f; // �ʱ� �ӵ�
    public float acceleration = 10f;  // ���ӵ�
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = rb.velocity * acceleration;
        rb.AddForce(force);
    }
}
