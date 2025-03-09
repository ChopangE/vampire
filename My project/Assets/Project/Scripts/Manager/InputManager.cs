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
        
        // 화살표 키 이벤트 (방향값 포함)
        public event EventHandler<Vector2> OnArrowKeyAction;
        // 숫자키 이벤트 (눌린 숫자 포함)
        public event EventHandler<int> OnNumberKeyAction;

        private PlayerAction playerAction;

        private void Awake()
        {
            playerAction = new PlayerAction();
            playerAction.Enable();
            
            playerAction.Player.Pause.performed += Pause_Performed;
            
            // 새로운 이벤트 등록
            playerAction.Player.Arrow.performed += Arrow_Performed;
            playerAction.Player.Number.performed += Number_Performed;
        }
        private void OnDisable()
        {
            playerAction.Disable();
        }
        private void Pause_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);
        }
        
        // 화살표 키 처리 메서드
        private void Arrow_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            // 화살표 키 값 읽기 (방향 벡터)
            Vector2 direction = Vector2.zero;
            
            // 키 값에 따라 방향 설정
            string keyPressed = context.control.name;
            
            // 디버그 로그 추가
            // Debug.Log("눌린 키: " + keyPressed);
            
            // 대소문자 구분 없이 비교하고 공백과 [Keyboard] 부분 무시
            if (keyPressed.Contains("left", StringComparison.OrdinalIgnoreCase))
            {
                direction = Vector2.left;
            }
            else if (keyPressed.Contains("right", StringComparison.OrdinalIgnoreCase))
            {
                direction = Vector2.right;
            }
            else if (keyPressed.Contains("up", StringComparison.OrdinalIgnoreCase))
            {
                direction = Vector2.up;
            }
            else if (keyPressed.Contains("down", StringComparison.OrdinalIgnoreCase))
            {
                direction = Vector2.down;
            }
            
            // 이벤트 발생
            OnArrowKeyAction?.Invoke(this, direction);
        }
        
        // 숫자키 처리 메서드
        private void Number_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            // 눌린 숫자키 값 읽기
            string keyPressed = context.control.name;
            int numberValue = -1;
            
            // 키 이름에서 숫자 추출 (예: "1", "2", "3" 등)
            if (int.TryParse(keyPressed, out int number))
            {
                numberValue = number;
            }
            // numpad 키인 경우 (예: "numpad1", "numpad2" 등)
            else if (keyPressed.StartsWith("numpad") && keyPressed.Length > 6)
            {
                string numStr = keyPressed.Substring(6);
                if (int.TryParse(numStr, out int numpadNumber))
                {
                    numberValue = numpadNumber;
                }
            }
            
            // 유효한 숫자키가 눌렸을 때만 이벤트 발생
            if (numberValue >= 0 && numberValue <= 9)
            {
                OnNumberKeyAction?.Invoke(this, numberValue);
            }
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