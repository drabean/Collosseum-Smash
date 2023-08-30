using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIControlCanvas : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joyStick;

    public Action<Vector2> joystickAction;
    void invokeJoystick(Vector2 input) { joystickAction?.Invoke(input); }

    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    private Vector2 joystickCenter;
    private bool isJoystickActive = false;

    float radius;

    void Awake()
    {
        radius = joystickBackground.sizeDelta.x * 0.5f;
    }

    public void setTarget(Action<Vector2> targetAction)
    {
        joystickAction = targetAction;
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        joyStick.gameObject.SetActive(true);
        joyStick.position = eventData.position;

        joystickCenter = joystickBackground.position;

        isJoystickActive = true;

        UpdateJoystick(eventData);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isJoystickActive)
        {
            UpdateJoystick(eventData);
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        isJoystickActive = false;
        joyStick.anchoredPosition = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        invokeJoystick(Vector2.zero);
    }


    private void UpdateJoystick(PointerEventData eventData)
    {
        Vector2 position = eventData.position;
        joystickHandle.position = position;

        Vector2 direction = (joystickHandle.localPosition).normalized;



        float clampMagnitude = Mathf.Clamp((joystickHandle.localPosition).magnitude, 0, radius);

        joystickHandle.localPosition = direction * clampMagnitude;

        //조이스틱 값 전달
        //invokeJoystick(joystickHandle.localPosition / radius);

        invokeJoystick(direction);

    }
}
