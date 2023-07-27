using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIJoystickCanvas : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joyStick;


    public void setTarget(Action<Vector2> targetAction)
    {
        joyStick.GetComponent<FloatingJoystick>().joystickAction = targetAction;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        joyStick.gameObject.SetActive(true);
        joyStick.position = Input.mousePosition;

        EventSystem.current.SetSelectedGameObject(joyStick.gameObject);
        ExecuteEvents.Execute(joyStick.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(joyStick.gameObject, eventData, ExecuteEvents.dragHandler);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ExecuteEvents.Execute(joyStick.gameObject, eventData, ExecuteEvents.pointerUpHandler);
        joyStick.gameObject.SetActive(false);


    }
}
