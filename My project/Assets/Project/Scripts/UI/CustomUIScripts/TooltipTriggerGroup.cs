using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public class TooltipTriggerGroup : MonoBehaviour
    {
        [Header("툴팁 패널")]
        [SerializeField] protected TooltipPanel tooltipPanel;
        private List<TooltipTrigger> tooltipTriggerList = new List<TooltipTrigger>();
        [Header("툴팁 보여주기전 딜레이")]
        public float delay = 0.2f;

        private bool mouseIsHovering;
        private float mouseHoverTime;

        private bool isMouseDown = false;
        private bool isHovering = false;
        private bool isControlInGroup = false;
        private void OnEnable(){

        }

        private void OnDisable()
        {
            foreach(var tooltipTrigger in tooltipTriggerList) {
                tooltipTrigger.isControlInGroup = false;
            }
        }

        private TooltipTrigger lastTooltipTrigger = null;
        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0)) isMouseDown = true;
            if (Input.GetMouseButtonUp(0)) isMouseDown = false;

            InitialTriggers();

            CheckLastTooltip();

            if(lastTooltipTrigger != null) return;
            
            foreach (var tooltipTrigger in tooltipTriggerList)
            {
                isHovering = RectTransformUtility.RectangleContainsScreenPoint(tooltipTrigger.rectTransform,
                 Manager.Global.InputManager.GetCurMousePos());
                if(isHovering && !isMouseDown) lastTooltipTrigger = tooltipTrigger;
            }
        }

        private void CheckLastTooltip()
        {
            if(lastTooltipTrigger != null)
            {
                isHovering = RectTransformUtility.RectangleContainsScreenPoint(lastTooltipTrigger.rectTransform,
                Manager.Global.InputManager.GetCurMousePos());
                if (!isMouseDown)
                {
                    if (mouseIsHovering)
                    {
                        mouseHoverTime += Time.unscaledDeltaTime;
                        if (mouseHoverTime >= delay)
                            ShowTooltip(lastTooltipTrigger.text, lastTooltipTrigger.rectTransform);
                    }
                    else
                    {
                        mouseIsHovering = true;
                        mouseHoverTime = 0;
                    }
                }
            }
            if (!isHovering)
            {
                mouseIsHovering = false;
                HideTooltip();
                lastTooltipTrigger = null;
            }
        }

        private void InitialTriggers()
        {
            if(tooltipTriggerList.Count == 0) {
                tooltipTriggerList = transform.GetComponentsInChildren<TooltipTrigger>().ToList();
                foreach (var tooltipTrigger in tooltipTriggerList)
                {
                    tooltipTrigger.isControlInGroup = true;
                }
            }
        }

        protected virtual void ShowTooltip(string _text, RectTransform _rectTransform)
        {
            if (tooltipPanel != null)
                tooltipPanel.Show(_text, _rectTransform);
        }

        protected virtual void HideTooltip()
        {
            if (tooltipPanel != null)
                tooltipPanel.Hide();
        }
    }

}