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
        private void OnEnable(){
            InitialTriggers();
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

            bool foundHoveringTrigger = false;
            TooltipTrigger currentHoveringTrigger = null;
            int highestSiblingIndex = -1;

            // 모든 tooltip trigger를 검사하여 가장 위에 있는 것을 찾음
            foreach (var tooltipTrigger in tooltipTriggerList)
            {
                // 계층 구조상 비활성화된 오브젝트는 건너뛰기
                if (!tooltipTrigger.gameObject.activeInHierarchy) continue;

                bool isCurrentHovering = RectTransformUtility.RectangleContainsScreenPoint(
                    tooltipTrigger.rectTransform,
                    Manager.Global.InputManager.GetCurMousePos());

                if (isCurrentHovering && !isMouseDown)
                {
                    int currentSiblingIndex = tooltipTrigger.transform.GetSiblingIndex();
                    if (currentSiblingIndex > highestSiblingIndex)
                    {
                        highestSiblingIndex = currentSiblingIndex;
                        currentHoveringTrigger = tooltipTrigger;
                        foundHoveringTrigger = true;
                    }
                }
            }

            // 어떤 트리거 위에도 없다면 tooltip 숨기기
            if (!foundHoveringTrigger)
            {
                HideTooltip();
                lastTooltipTrigger = null;
                mouseIsHovering = false;
                mouseHoverTime = 0;
                return;
            }

            // 새로운 트리거로 변경되었다면 초기화
            if (currentHoveringTrigger != lastTooltipTrigger)
            {
                HideTooltip();
                mouseIsHovering = false;
                mouseHoverTime = 0;
                lastTooltipTrigger = currentHoveringTrigger;
            }

            // 현재 호버링 중인 트리거 체크
            if (lastTooltipTrigger != null && !isMouseDown)
            {
                if (mouseIsHovering)
                {
                    mouseHoverTime += Time.unscaledDeltaTime;
                    if (mouseHoverTime >= delay)
                    {
                        ShowTooltip(lastTooltipTrigger.text, lastTooltipTrigger.rectTransform);
                    }
                }
                else
                {
                    mouseIsHovering = true;
                    mouseHoverTime = 0;
                }
            }
        }

        private void InitialTriggers()
        {
            tooltipTriggerList = transform.GetComponentsInChildren<TooltipTrigger>().ToList();
            foreach (var tooltipTrigger in tooltipTriggerList)
            {
                tooltipTrigger.isControlInGroup = true;
            }
        }

        public void RegisterTooltipTriger(TooltipTrigger tooltipTrigger)
        {
            foreach(var t in tooltipTriggerList) {
                if(t == tooltipTrigger)
                {
                    return;
                }
            }
            tooltipTriggerList.Add(tooltipTrigger);
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