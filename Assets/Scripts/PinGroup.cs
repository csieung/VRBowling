using UnityEngine;

public class PinGroup : MonoBehaviour
{
    private Pin[] pins; // Pin 객체 배열
    private Vector3[] positions; // 각 핀의 초기 위치를 저장할 배열
    private Quaternion[] rotations; // 각 핀의 초기 회전값을 저장할 배열

    // Awake 메소드는 스크립트 인스턴스가 로드될 때 호출됩니다.
    protected void Awake()
    {
        // 자식 객체들에서 Pin 컴포넌트를 모두 가져옵니다.
        pins = GetComponentsInChildren<Pin>();
        // 핀의 초기 위치와 회전값을 저장합니다.
        SavePositions();
    }

    // 핀의 현재 위치와 회전값을 저장합니다.
    public void SavePositions()
    {
        // 배열을 핀의 개수만큼 초기화합니다.
        positions = new Vector3[pins.Length];
        rotations = new Quaternion[pins.Length];
        // 각 핀의 위치와 회전값을 저장합니다.
        for (int index = 0; index < pins.Length; index++)
        {
            positions[index] = pins[index].transform.position;
            rotations[index] = pins[index].transform.rotation;
        }
    }

    // 핀의 위치와 회전값을 초기 값으로 리셋합니다.
    public void ResetPositions()
    {
        // 각 핀의 위치와 회전값을 초기 값으로 되돌립니다.
        for (int index = 0; index < pins.Length; index++)
        {
            pins[index].transform.position = positions[index];
            pins[index].transform.rotation = rotations[index];
            pins[index].CancelToppleCheck(); // 핀의 상태 체크를 취소합니다.

            // 핀의 Rigidbody를 가져와서 속도와 각속도를 0으로 설정합니다.
            Rigidbody pinRigidbody = pins[index].GetComponent<Rigidbody>();
            pinRigidbody.velocity = Vector3.zero;
            pinRigidbody.angularVelocity = Vector3.zero;

            // 핀을 활성화합니다.
            pins[index].gameObject.SetActive(true);
        }
    }
}
