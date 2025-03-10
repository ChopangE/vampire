using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public partial class UIManager : MonoBehaviour
    {
        [SerializeField] private Image brightnessOverlay; // 전체 화면을 덮는 이미지
        
        private void Awake()
        {
            // 밝기 오버레이 초기화
            InitializeBrightnessOverlay();
            
            // 저장된 밝기 값 불러오기
            float savedBrightness = PlayerPrefs.GetFloat("Brightness", 50f);
            SetBrightness(savedBrightness);
        }
        
        private void InitializeBrightnessOverlay()
        {
            if (brightnessOverlay == null)
            {
                // 이미지 컴포넌트 추가
                brightnessOverlay.color = new Color(0, 0, 0, 0); // 초기 투명 검정색
                
                // 최상위 정렬 순서 설정
                brightnessOverlay.raycastTarget = false; // 레이캐스트 무시
                Canvas.ForceUpdateCanvases();
            }
        }
        
        /// <summary>
        /// 화면 밝기 설정 (0-100 범위)
        /// </summary>
        public void SetBrightness(float brightnessValue)
        {
            if (brightnessOverlay == null) return;
            
            // 밝기 값을 0-100에서 알파 값으로 변환 (100이 가장 밝음, 0이 가장 어두움)
            // 50이 기본값(중간 밝기)으로 설정
            float alpha = 0;
            
            if (brightnessValue < 50)
            {
                // 50 미만이면 어둡게 (검은색 오버레이 추가)
                alpha = Mathf.Lerp(0.5f, 0f, brightnessValue / 50f);
                brightnessOverlay.color = new Color(0, 0, 0, alpha);
            }
            else if (brightnessValue > 50)
            {
                // 50 초과면 밝게 (흰색 오버레이 추가)
                alpha = Mathf.Lerp(0f, 0.5f, (brightnessValue - 50) / 50f);
                brightnessOverlay.color = new Color(1, 1, 1, alpha);
            }
            else
            {
                // 50이면 기본 밝기 (오버레이 없음)
                brightnessOverlay.color = new Color(0, 0, 0, 0);
            }

            PlayerPrefs.SetFloat("Brightness", brightnessValue);
        }
        
        // [SerializeField] private Canvas canvas;
        // private void Start() {
        //     canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //     canvas.worldCamera = Camera.main;
        //     canvas.sortingLayerID = SortingLayer.NameToID("UI");
        //     canvas.sortingOrder = 0;
        // }
    }
}