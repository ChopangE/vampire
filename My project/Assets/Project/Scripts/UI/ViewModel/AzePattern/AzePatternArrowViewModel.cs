using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class AzePatternArrowViewModel : ViewModel
    {
        // 인스펙터에서 할당할 화살표 스프라이트 리스트
        [SerializeField] private List<Sprite> arrowSprites = new List<Sprite>();
        
        // 방향별 인덱스 (인스펙터에서 설정)
        [SerializeField] private int upArrowIndex = 0;
        [SerializeField] private int rightArrowIndex = 1;
        [SerializeField] private int downArrowIndex = 2;
        [SerializeField] private int leftArrowIndex = 3;
        
        private Vector2 _direction;
        private bool _isActive;
        private bool _isCompleted;
        private Sprite _currentSprite;
        private Color _arrowColor;
        private float _arrowOpacity;
        [Binding]
        public Sprite CurrentSprite
        {
            get => _currentSprite;
            set
            {
                _currentSprite = value;
                OnPropertyChanged(nameof(CurrentSprite));
            }
        }
        
        // 화살표 방향 설정
        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
            
            // 방향에 따라 회전 각도 설정
            if (direction == Vector2.up)
            {
                Rotation = 0f;
                SetArrowSprite(upArrowIndex);
            }
            else if (direction == Vector2.right)
            {
                Rotation = 90f;
                SetArrowSprite(rightArrowIndex);
            }
            else if (direction == Vector2.down)
            {
                Rotation = 180f;
                SetArrowSprite(downArrowIndex);
            }
            else if (direction == Vector2.left)
            {
                Rotation = 270f;
                SetArrowSprite(leftArrowIndex);
            }
            
            OnPropertyChanged(nameof(Rotation));
        }
        
        // 화살표 스프라이트 설정
        private void SetArrowSprite(int index)
        {
            if (arrowSprites != null && arrowSprites.Count > 0 && index >= 0 && index < arrowSprites.Count)
            {
                CurrentSprite = arrowSprites[index];
            }
        }
        
        // 화살표 활성화 상태 설정
        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            ArrowColor = isActive ? Color.white : Color.gray;
        }
        
        // 화살표 완료 상태 설정 (회색으로 변경)
        public void SetCompleted(bool isCompleted)
        {
            IsCompleted = isCompleted;
            ArrowColor = isCompleted ? Color.gray : Color.white;
            ArrowOpacity = isCompleted ? 0.5f : 1f;
        }
    
        
        // 화살표 회전 각도 (바인딩용)
        [Binding]
        public float Rotation { get; private set; }
        
        // 화살표 활성화 상태 (바인딩용)
        [Binding]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
        
        // 화살표 완료 상태 (바인딩용)
        [Binding]
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }
        
        // 화살표 색상 (바인딩용)
        [Binding]
        public Color ArrowColor
        {
            get => _arrowColor;
            set
            {
                _arrowColor = value;
                OnPropertyChanged(nameof(ArrowColor));
            }
        }
        
        // 화살표 투명도 (바인딩용)
        [Binding]
        public float ArrowOpacity
        {
            get => _arrowOpacity;
            set
            {
                _arrowOpacity = value;
                OnPropertyChanged(nameof(ArrowOpacity));
            }
        }
    }
}
