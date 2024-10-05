using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Manager
{
    public class FowManager : MonoBehaviour
    {
        [LabelText("FOW 벽 상하좌우 순서대로 넣어주셈")]
        [SerializeField] private List<GameObject> FowObjects; //* 상하좌우 순서대로

        private void Start()
        {
            SetFowPositions();
        }

        private void SetFowPositions()
        {
            // Assuming GetMapBounds returns a Rect or Bounds representing the boundaries of the map
            Bounds mapBounds = MapManager.Instance.GetMapBounds();

            // Calculate dimensions of the map
            float mapWidth = mapBounds.size.x;
            float mapHeight = mapBounds.size.y;


            // Top (Y+ direction) - Positioned at top edge, stretches horizontally, but rotated 90 degrees
            FowObjects[0].transform.localScale = new Vector3(FowObjects[0].transform.localScale.x, mapWidth, 1f); // Adjusted for 90-degree rotation
            Vector3 topPosition = new Vector3(mapBounds.center.x, mapBounds.max.y + 9, 0f);
            FowObjects[0].transform.position = topPosition;

            // Bottom (Y- direction) - Positioned at bottom edge, stretches horizontally, but rotated 90 degrees
            FowObjects[1].transform.localScale = new Vector3(FowObjects[1].transform.localScale.x, mapWidth, 1f); // Adjusted for 90-degree rotation
            Vector3 bottomPosition = new Vector3(mapBounds.center.x, mapBounds.min.y - 9, 0f);
            FowObjects[1].transform.position = bottomPosition;

            // Left (X- direction) - Positioned at left edge, stretches vertically
            FowObjects[2].transform.localScale = new Vector3(FowObjects[2].transform.localScale.x, mapHeight, 1f);
            Vector3 leftPosition = new Vector3(mapBounds.min.x - 10.5f, mapBounds.center.y, 0f);
            FowObjects[2].transform.position = leftPosition;

            // Right (X+ direction) - Positioned at right edge, stretches vertically
            FowObjects[3].transform.localScale = new Vector3(FowObjects[3].transform.localScale.x, mapHeight, 1f);
            Vector3 rightPosition = new Vector3(mapBounds.max.x + 10.5f, mapBounds.center.y, 0f);
            FowObjects[3].transform.position = rightPosition;

        }
    }
}