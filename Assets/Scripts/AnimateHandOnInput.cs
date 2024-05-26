using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>(); // 트리거 버튼 입력을 알 수 있다.
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAnimationAction.action.ReadValue<float>(); // 그랩 버튼 입력을 알 수 있다.
        handAnimator.SetFloat("Grip", gripValue);
    }
}
