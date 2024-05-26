using UnityEngine;
using Zinnia.Extension; // Zinnia 라이브러리의 확장 메소드를 사용하기 위해 추가

public class Pin : MonoBehaviour
{
    public float pinFallThreshold = 0.02f; // 핀이 넘어졌는지 감지하는 임계값
    public float toppleLife = 3f; // 핀이 넘어지고 비활성화되기까지의 시간
    public int tries = 10; // 회전 검사를 수행할 최대 시도 횟수

    private Quaternion defaultRotation; // 핀의 초기 회전값
    private int currentTries; // 현재 회전 검사의 시도 횟수

    // Awake 메소드는 스크립트 인스턴스가 로드될 때 호출됩니다.
    protected void Awake()
    {
        // 핀의 초기 회전값을 저장합니다.
        defaultRotation = transform.localRotation;
    }

    // 핀이 넘어졌는지 확인하는 체크를 시작합니다.
    public void CheckTopple()
    {
        CancelToppleCheck(); // 기존의 체크를 취소합니다.
        CheckRotation(); // 회전 검사를 즉시 수행합니다.
        // 회전 검사를 일정 간격(1초)으로 반복 실행합니다.
        InvokeRepeating("CheckRotation", 0f, 1f);
    }

    // 기존의 체크를 취소하고 초기화합니다.
    public void CancelToppleCheck()
    {
        currentTries = 0; // 시도 횟수를 초기화합니다.
        CancelInvoke("CheckRotation"); // CheckRotation 메소드 호출을 취소합니다.
        CancelInvoke("HidePin"); // HidePin 메소드 호출을 취소합니다.
    }

    // 핀의 회전을 검사하여 넘어졌는지 확인합니다.
    protected void CheckRotation()
    {
        currentTries++; // 현재 시도 횟수를 증가시킵니다.
        // 핀이 넘어졌는지 확인합니다. 
        // Quaternion.Dot은 두 회전 사이의 코사인 값을 반환합니다. 
        // ApproxEquals는 두 값이 주어진 오차 범위 내에서 같은지 확인합니다.
        if (!Mathf.Abs(Quaternion.Dot(defaultRotation, transform.localRotation)).ApproxEquals(1f, pinFallThreshold))
        {
            // 핀이 넘어졌다면 일정 시간이 지나면 HidePin 메소드를 호출하여 핀을 숨깁니다.
            Invoke("HidePin", toppleLife);
        }
        else
        {
            // 최대 시도 횟수에 도달하면 체크를 취소합니다.
            if (currentTries > tries)
            {
                CancelToppleCheck();
            }
        }
    }

    // 핀을 비활성화합니다.
    protected void HidePin()
    {
        gameObject.SetActive(false);
    }
}

// 핀 회전을 주기적으로 검사하여 핀이 넘어졌는지 확인, 넘어졌다면 일정시간 후 핀을 비활성화하는 기능