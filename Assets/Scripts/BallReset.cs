using UnityEngine;

public class BallReset : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // �ʱ� ��ġ�� ȸ�� �� ����
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ResetPosition()
    {
        // ��ġ�� ȸ�� ���� �ʱ� ������ ����
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // �ʿ��� ��� �ӵ��� ���ӵ��� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
