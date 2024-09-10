using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld;
using UnityWeld.Binding;

// Attach this component to a UI object which will be the tooltip visual
// Must include some text somewhere of course, and optionally a CanvasGroup if you want fading
namespace UI
{
    [Binding]
    public class TooltipPanel : ViewModel
    {
        [Tooltip("Whether to move the tooltip with the mouse after it's visible")]
        public bool moveWithMouse;
        [Tooltip("The distance from the mouse position the tooltip will appear at (relative to tooltip pivot)")]
        public Vector2 positionOffset;
        [Tooltip("The margins from the edge of the screen which the tooltip will stay within")]
        public Vector2 margins;
        [Tooltip("The fade in/out duration of the tooltip: requires that this object has a CanvasGroup component, ignored if it doesn't")]
        public float fadeDuration = 0.25f;


        private Canvas parentCanvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Tweener fadeTween;
        private RectTransform triggerObject;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            parentCanvas = GetComponentInParent<Canvas>();

        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (moveWithMouse)
                Reposition();
        }

        private void Reposition()
        {
            Vector2 screenPos = Input.mousePosition;
            // world position origin is wherever the pivot is
            Vector2 newPos = screenPos + positionOffset;
            float maxX = Screen.width - margins.x;
            float minX = margins.x;
            float maxY = Screen.height - margins.y;
            float minY = margins.y;
            float rightEdge = newPos.x + (1f - rectTransform.pivot.x) * rectTransform.rect.width * parentCanvas.scaleFactor;
            if (rightEdge > maxX)
            {
                newPos.x -= rightEdge - maxX;
            }
            float leftEdge = newPos.x - rectTransform.pivot.x * rectTransform.rect.width * parentCanvas.scaleFactor;
            if (leftEdge < minX)
            {
                newPos.x += minX - leftEdge;
            }

            // y is measured from top
            float topEdge = newPos.y + (1f - rectTransform.pivot.y) * rectTransform.rect.height * parentCanvas.scaleFactor;
            if (topEdge > maxY)
            {
                newPos.y -= topEdge - maxY;
            }

            float bottomEdge = newPos.y - rectTransform.pivot.y * rectTransform.rect.height * parentCanvas.scaleFactor;
            if (bottomEdge < minY)
            {
                newPos.y += minY - bottomEdge;
            }

            var cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
            if(triggerObject != null)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(triggerObject, newPos, cam, out var worldPoint);
                rectTransform.position = worldPoint;
            }

        }
        public virtual void Show(string text, RectTransform triggeredBy)
        {
            if (gameObject.activeSelf)
                return;

            triggerObject = triggeredBy;
            Info = text;

            if (fadeTween != null)
            {
                fadeTween.Kill();
                fadeTween = null;
            }

            bool fade = canvasGroup != null && fadeDuration > 0;
            if (fade)
                canvasGroup.alpha = 0;

            gameObject.SetActive(true);

            if (fade)
            {
                fadeTween = canvasGroup.DOFade(1, fadeDuration)
                    .SetUpdate(true)
                    .OnComplete(() => { fadeTween = null; });
            }
            // in case we need to resize (e.g. content fitter)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            Reposition();
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            if (fadeTween != null)
            {
                fadeTween.Kill();
                fadeTween = null;
            }

            bool fade = canvasGroup != null && fadeDuration > 0;
            if (fade)
            {
                // Fade out
                fadeTween = canvasGroup.DOFade(1, fadeDuration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        fadeTween = null;
                    });
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        private string _info;
        [Binding]
        public string Info
        {
            get => _info;
            set
            {
                _info = value;
                OnPropertyChanged(nameof(Info));
            }
        }
    }
}