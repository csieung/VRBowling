using UnityEngine;

public class PinGroup : MonoBehaviour
{
    private Pin[] pins; // Pin ��ü �迭
    private Vector3[] positions; // �� ���� �ʱ� ��ġ�� ������ �迭
    private Quaternion[] rotations; // �� ���� �ʱ� ȸ������ ������ �迭

    // Awake �޼ҵ�� ��ũ��Ʈ �ν��Ͻ��� �ε�� �� ȣ��˴ϴ�.
    protected void Awake()
    {
        // �ڽ� ��ü�鿡�� Pin ������Ʈ�� ��� �����ɴϴ�.
        pins = GetComponentsInChildren<Pin>();
        // ���� �ʱ� ��ġ�� ȸ������ �����մϴ�.
        SavePositions();
    }

    // ���� ���� ��ġ�� ȸ������ �����մϴ�.
    public void SavePositions()
    {
        // �迭�� ���� ������ŭ �ʱ�ȭ�մϴ�.
        positions = new Vector3[pins.Length];
        rotations = new Quaternion[pins.Length];
        // �� ���� ��ġ�� ȸ������ �����մϴ�.
        for (int index = 0; index < pins.Length; index++)
        {
            positions[index] = pins[index].transform.position;
            rotations[index] = pins[index].transform.rotation;
        }
    }

    // ���� ��ġ�� ȸ������ �ʱ� ������ �����մϴ�.
    public void ResetPositions()
    {
        // �� ���� ��ġ�� ȸ������ �ʱ� ������ �ǵ����ϴ�.
        for (int index = 0; index < pins.Length; index++)
        {
            pins[index].transform.position = positions[index];
            pins[index].transform.rotation = rotations[index];
            pins[index].CancelToppleCheck(); // ���� ���� üũ�� ����մϴ�.

            // ���� Rigidbody�� �����ͼ� �ӵ��� ���ӵ��� 0���� �����մϴ�.
            Rigidbody pinRigidbody = pins[index].GetComponent<Rigidbody>();
            pinRigidbody.velocity = Vector3.zero;
            pinRigidbody.angularVelocity = Vector3.zero;

            // ���� Ȱ��ȭ�մϴ�.
            pins[index].gameObject.SetActive(true);
        }
    }
}
