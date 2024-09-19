using System;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TooltipTrigger : MonoBehaviour
    {
        [LabelText("툴팁 띄울 패널")]
        public TooltipPanel tooltipPanel;
        [LabelText("툴팁이 띄울 텍스트")]
        public string text;
        [LabelText("툴팁 보여주기 전 딜레이")]
        public float delay = 1f;
        
        public RectTransform rectTransform;
        [LabelText("그룹으로 관리할것인가")]
        public bool isControlInGroup = false;

        private bool mouseIsHovering;
        private float mouseHoverTime;
        
        private bool isMouseDown = false;
        private bool isHovering = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            if (tooltipPanel == null)
            {
                var canvases = Resources.FindObjectsOfTypeAll<TooltipPanel>();
                if (canvases.Length > 0)
                    tooltipPanel = canvases[0];
            }
            if(isControlInGroup)
            {
                TooltipTriggerGroup tooltipTriggerGroup = transform.GetComponentInParent<TooltipTriggerGroup>();
                tooltipTriggerGroup?.RegisterTooltipTriger(this);
            }
        }

        private void Update()
        {
            if(!isControlInGroup)
                CheckMouseHover();
        }

        private void CheckMouseHover()
        {
            isHovering = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Global.InputManager.GetCurMousePos());

            if (Input.GetMouseButtonDown(0)) isMouseDown = true;
            if (Input.GetMouseButtonUp(0)) isMouseDown = false;

            if (isHovering && !isMouseDown)
            {
                if (mouseIsHovering)
                {
                    mouseHoverTime += Time.unscaledDeltaTime;
                    if (mouseHoverTime >= delay)
                        ShowTooltip();
                }
                else
                {
                    mouseIsHovering = true;
                    mouseHoverTime = 0;
                }
            }
            else if (!isHovering)
            {
                mouseIsHovering = false;
                HideTooltip();
            }
        }


        private void ShowTooltip()
        {
            if (tooltipPanel != null)
                tooltipPanel.Show(text, rectTransform);
        }

        private void HideTooltip()
        {
            if (tooltipPanel != null)
                tooltipPanel.Hide();
        }

    }
}