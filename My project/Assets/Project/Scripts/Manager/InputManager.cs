using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Manager
{

    public class InputManager : MonoBehaviour
    {
        public event EventHandler OnPauseAction;
        //* 인게임 터치 처리
        public event EventHandler<Vector2> OnTouchPressAction;
        public event EventHandler<Vector2> OnTouchPressEndAction;

        private PlayerAction playerAction;

        private void Awake()
        {
            playerAction = new PlayerAction();
            playerAction.Enable();
        }
        private void OnDisable()
        {
            playerAction.Disable();
        }
        private void Pause_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);
        }
        private void TouchPress_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            // 값 유형 모르면 아래 함수 사용
            // context.ReadValueAsObject()
            Vector2 positon = Camera.main.ScreenToWorldPoint(playerAction.UI.Point.ReadValue<Vector2>());

            OnTouchPressAction?.Invoke(this, positon);

        }
        private void TouchPressEnd_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Vector2 positon = Camera.main.ScreenToWorldPoint(playerAction.UI.Point.ReadValue<Vector2>());
            OnTouchPressEndAction?.Invoke(this, positon);
        }
        private PointerEventData eventData = null;
        public Vector2 GetCurMousePos()
        {
            if(eventData == null) eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            return eventData.position;
        }
    }

}