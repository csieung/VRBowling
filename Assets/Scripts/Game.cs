using UnityEngine;
using UnityEngine.XR;

public class Game : MonoBehaviour
{
    public BallReset[] ballResets;
    public PinReset[] pinResets;
    public ScorePin[] scorepins;

    void Update()
    {
        // ���� ��Ʈ�ѷ��� Ʈ���� ��ư �Է� ���� (HTC Vive)
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool triggerValue;
        if (leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue) && triggerValue)
        {
            ResetGameObjects();
        }
    }

    private void ResetGameObjects()
    {
        // ������ ��ġ ����
        //if (ballReset != null)
        //{
        //    ballReset.ResetPosition();
        //}
        // ��� �� ��ġ ����
        foreach (var ballReset in ballResets)
        {
            if (ballReset != null)
            {
                ballReset.ResetPosition();
            }
        }
        // ��� �� ��ġ ����
        foreach (var pinReset in pinResets)
        {
            if (pinReset != null)
            {
                pinReset.ResetPosition();
            }
        }
        foreach (var scorepin in scorepins)
        {
            if (scorepin != null)
            {
                scorepin.ResetPinScore();
            }
        }
    }
}
