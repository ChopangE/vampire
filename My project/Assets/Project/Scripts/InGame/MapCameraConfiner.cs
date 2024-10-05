using Cinemachine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

//* 맵에 맞게 카메라 영역 제한 할당
namespace InGame
{
    public class MapCameraConfiner : MonoBehaviour
    {
        private CinemachineConfiner2D _confiner;
        void Start()
        {
            ConfinerTimer().Forget();
        }
        private async UniTaskVoid ConfinerTimer()
        {
            await UniTask.WaitForSeconds(1);
            var Map = GameManager.Instance.CurStagePos().GetChild(0);
            var coli = Map.AddComponent<PolygonCollider2D>();
            coli.isTrigger = true;

            // Get the bounds of the current stage
            Bounds stageBounds = GameManager.Instance.CurStageBounds();

            // Initialize the boundsPoints array
            Vector2[] boundsPoints = new Vector2[4];

            // Set the bounds points (corners) based on the Bounds object
            boundsPoints[0] = new Vector2(stageBounds.min.x - 4, stageBounds.min.y - 4 + Mathf.Abs(stageBounds.center.y)); // Bottom left
            boundsPoints[1] = new Vector2(stageBounds.max.x + 4, stageBounds.min.y - 4 + Mathf.Abs(stageBounds.center.y)); // Bottom right
            boundsPoints[2] = new Vector2(stageBounds.max.x + 4, stageBounds.max.y + 4 + Mathf.Abs(stageBounds.center.y)); // Top right
            boundsPoints[3] = new Vector2(stageBounds.min.x - 4, stageBounds.max.y + 4 + Mathf.Abs(stageBounds.center.y)); // Top left

            // Assign the points to the PolygonCollider2D
            coli.points = boundsPoints;

            if (TryGetComponent(out _confiner))
                _confiner.m_BoundingShape2D = coli;
        }
    }
}