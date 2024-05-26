using UnityEngine;

public class BallReset : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // 초기 위치와 회전 값 저장
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ResetPosition()
    {
        // 위치와 회전 값을 초기 값으로 리셋
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // 필요한 경우 속도와 각속도도 리셋
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
