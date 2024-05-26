using UnityEngine;
using Zinnia.Extension; // Zinnia ���̺귯���� Ȯ�� �޼ҵ带 ����ϱ� ���� �߰�

public class Pin : MonoBehaviour
{
    public float pinFallThreshold = 0.02f; // ���� �Ѿ������� �����ϴ� �Ӱ谪
    public float toppleLife = 3f; // ���� �Ѿ����� ��Ȱ��ȭ�Ǳ������ �ð�
    public int tries = 10; // ȸ�� �˻縦 ������ �ִ� �õ� Ƚ��

    private Quaternion defaultRotation; // ���� �ʱ� ȸ����
    private int currentTries; // ���� ȸ�� �˻��� �õ� Ƚ��

    // Awake �޼ҵ�� ��ũ��Ʈ �ν��Ͻ��� �ε�� �� ȣ��˴ϴ�.
    protected void Awake()
    {
        // ���� �ʱ� ȸ������ �����մϴ�.
        defaultRotation = transform.localRotation;
    }

    // ���� �Ѿ������� Ȯ���ϴ� üũ�� �����մϴ�.
    public void CheckTopple()
    {
        CancelToppleCheck(); // ������ üũ�� ����մϴ�.
        CheckRotation(); // ȸ�� �˻縦 ��� �����մϴ�.
        // ȸ�� �˻縦 ���� ����(1��)���� �ݺ� �����մϴ�.
        InvokeRepeating("CheckRotation", 0f, 1f);
    }

    // ������ üũ�� ����ϰ� �ʱ�ȭ�մϴ�.
    public void CancelToppleCheck()
    {
        currentTries = 0; // �õ� Ƚ���� �ʱ�ȭ�մϴ�.
        CancelInvoke("CheckRotation"); // CheckRotation �޼ҵ� ȣ���� ����մϴ�.
        CancelInvoke("HidePin"); // HidePin �޼ҵ� ȣ���� ����մϴ�.
    }

    // ���� ȸ���� �˻��Ͽ� �Ѿ������� Ȯ���մϴ�.
    protected void CheckRotation()
    {
        currentTries++; // ���� �õ� Ƚ���� ������ŵ�ϴ�.
        // ���� �Ѿ������� Ȯ���մϴ�. 
        // Quaternion.Dot�� �� ȸ�� ������ �ڻ��� ���� ��ȯ�մϴ�. 
        // ApproxEquals�� �� ���� �־��� ���� ���� ������ ������ Ȯ���մϴ�.
        if (!Mathf.Abs(Quaternion.Dot(defaultRotation, transform.localRotation)).ApproxEquals(1f, pinFallThreshold))
        {
            // ���� �Ѿ����ٸ� ���� �ð��� ������ HidePin �޼ҵ带 ȣ���Ͽ� ���� ����ϴ�.
            Invoke("HidePin", toppleLife);
        }
        else
        {
            // �ִ� �õ� Ƚ���� �����ϸ� üũ�� ����մϴ�.
            if (currentTries > tries)
            {
                CancelToppleCheck();
            }
        }
    }

    // ���� ��Ȱ��ȭ�մϴ�.
    protected void HidePin()
    {
        gameObject.SetActive(false);
    }
}

// �� ȸ���� �ֱ������� �˻��Ͽ� ���� �Ѿ������� Ȯ��, �Ѿ����ٸ� �����ð� �� ���� ��Ȱ��ȭ�ϴ� ���