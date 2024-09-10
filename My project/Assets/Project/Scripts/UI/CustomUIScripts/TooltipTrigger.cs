using System;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TooltipTrigger : MonoBehaviour
    {
        [Tooltip("The tooltip canvas which will display the tooltip. If not specified, we'll try to find one.")]
        public TooltipPanel tooltipPanel;
        [Tooltip("The text to display in the tooltip")]
        public string text;
        [Tooltip("How long the mouse must linger on us before showing the tooltip")]
        public float delay = 1f;
        
        public RectTransform rectTransform;
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