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
        Vector2 position = eventData.position;
        Vector2 direction = (position - joystickCenter).normalized;

        float radius = joystickBackground.sizeDelta.x * 0.5f;
        float clampMagnitude = Mathf.Clamp((position - joystickCenter).magnitude, 0, radius);
        joystickHandle.anchoredPosition = direction * clampMagnitude;

        //조이스틱 값 전달
        invokeJoystick(direction);
    }
}

