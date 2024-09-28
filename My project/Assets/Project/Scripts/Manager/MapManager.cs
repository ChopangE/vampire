using System.Collections;
using System.Collections.Generic;
using InGame;
using ObjectPooling;
using UnityEngine;

namespace Manager
{
    public class MapManager : MMSingleton<MapManager>
    {
        public Bounds GetMapBounds() => GameManager.Instance.CurStageBounds();

        private Bounds cameraBounds;
        public Vector3 GetRandomSpawnPosition(List<PoolObject> spawnedItemList, float minimumDistanceBetweenItems)
        {
            var mapBounds = GetMapBounds();
            Vector3 cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
            cameraBounds.SetMinMax(cameraBottomLeft, cameraTopRight);
            
            if (cameraBounds.size.x >= mapBounds.size.x || cameraBounds.size.y >= mapBounds.size.y)
            {
                Debug.LogError("Camera bounds exceed map bounds.");
                return Vector3.zero;
            }

            float halfMapWidth = mapBounds.size.x / 2 - 5;
            float halfMapHeight = mapBounds.size.y / 2 - 5;

            const int maxAttempts = 200;
            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                // Generate random position within bounds
                float randomX = Random.Range(-halfMapWidth, halfMapWidth);
                float randomY = Random.Range(-halfMapHeight, halfMapHeight);
                Vector3 randomPosition = mapBounds.center + new Vector3(randomX, randomY, 0);

                if (!IsBlockedPosition(randomPosition, spawnedItemList, minimumDistanceBetweenItems))
                {
                    return randomPosition;
                }
            }

            Debug.LogWarning("스폰 위치 찾기 실패");
            return Vector3.zero;
        }

        private bool IsBlockedPosition(Vector3 spawnPos, List<PoolObject> poolObjectList, float minimumDistanceBetweenItems)
        {
            if (cameraBounds.Contains(spawnPos)) return true;

            foreach (var obj in poolObjectList)
            {
                if (Vector3.Distance(obj.transform.position, spawnPos) < minimumDistanceBetweenItems)
                {
                    return true;
                }
            }

            return false;
        }

    }
}