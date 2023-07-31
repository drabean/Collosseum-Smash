using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    private Vector2 joystickCenter;
    private bool isJoystickActive = false;

    float radius;
   void Awake()
    {
        radius = joystickBackground.sizeDelta.x * 0.5f;
    }

    public Action<Vector2> joystickAction;
    void invokeJoystick(Vector2 input) { joystickAction?.Invoke(input); }

    public void OnPointerDown(PointerEventData eventData)
    {
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
        joystickHandle.anchoredPosition = Vector2.zero;
        invokeJoystick(Vector2.zero);
    }

    private void UpdateJoystick(PointerEventData eventData)
    {
        Vector2 position = Input.mousePosition;
        joystickHandle.position = position;

        Vector2 direction = (joystickHandle.localPosition).normalized;



        float clampMagnitude = Mathf.Clamp((joystickHandle.localPosition).magnitude, 0, radius);

        joystickHandle.localPosition = direction * clampMagnitude;

        //조이스틱 값 전달
        //invokeJoystick(joystickHandle.localPosition / radius);

        invokeJoystick(direction);

    }
}

