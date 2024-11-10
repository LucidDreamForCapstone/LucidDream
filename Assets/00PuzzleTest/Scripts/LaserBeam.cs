using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField]
        LineRenderer lineRenderer;
        [SerializeField]
        float laserLength;
        [SerializeField]
        Vector2 startPos;
        [SerializeField]
        Vector2 startDir;
        [SerializeField]
        LayerMask layerMask;
        [SerializeField]
        Action<List<Vector2>, Vector2, float> SeperateLaser;
        [SerializeField]
        int maxReflectionCount = 10;

        #region Constructor
        public void InitLaser(Vector2 startPos, Vector2 startDir, Action<List<Vector2>, Vector2, float> onSeperate, float maxLaserLen)
        {
            this.startDir = startDir;

            this.startPos = startPos;
            SeperateLaser = onSeperate;
            maxReflectionCount = 10;
            laserLength = maxLaserLen;

        }
        #endregion

        #region MonoBehaviour
        void Start()
        {

        }
        void Update()
        {
            //UpdateRaser();
        }
        public void UpdateRaser()
        {
            List<Vector3> laserPoints = new List<Vector3>();
            laserPoints.Add(startPos);
            Vector2 nextPos = startPos;
            Vector2 nextDir = startDir;
            for (int i = 0; i < maxReflectionCount; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(nextPos, nextDir, laserLength, layerMask);
                // Debug.DrawRay(nextPos, nextDir * laserLength, Color.red, 0.1f);
                if (hit)
                {
                    Destination dest = hit.transform.GetComponent<Destination>();
                    if (dest)
                    {
                        dest.DestCount--;
                        //Debug.Log("DestCount decreased");
                        laserPoints.Add(hit.transform.position);
                        break;
                    }
                    ReflectorInformation info = hit.transform.gameObject.GetComponent<ReflectorInformation>();
                    if (info != null && info.IsUsed == false)//거울에 부딪히면
                    {
                        Collider2D collider = hit.collider;
                        Vector2 reflectorPosition = hit.transform.position; //충돌 거울의 위치
                        ReflectorInformation reflector = hit.transform.gameObject.GetComponent<ReflectorInformation>();
                        List<Vector2> reflectDirs = reflector.GetRotatedReflectionDirections();
                        laserPoints.Add(reflectorPosition);// 이번 Laser의 끝에 넣고
                        if (reflectDirs.Count >= 2) //만약 2갈래 이상이면 나눠라
                        {
                            SeperateLaser?.Invoke(reflectDirs, reflectorPosition, (collider.bounds.extents.magnitude)); // 나눠라
                            break;
                        }
                        else// 아니면 레이저 반사
                        {
                            nextPos = LaserUtil.ToBoundary(reflectorPosition, reflectDirs[0], (collider.bounds.extents.magnitude));
                            nextDir = reflectDirs[0];
                        }
                        info.IsUsed = true;
                    }
                }
                else //거울에 부딪히지 않으면
                {
                    nextPos += nextDir.normalized * laserLength; //
                    laserPoints.Add(nextPos);
                    break;
                }
            }
            Vector3[] LaserPoints = new Vector3[laserPoints.Count];
            for (int i = 0; i < laserPoints.Count; i++)
            {
                laserPoints[i] = transform.InverseTransformPoint(laserPoints[i]); // 로컬 좌표를 월드 좌표로 변환
            }
            lineRenderer.positionCount = laserPoints.Count;
            lineRenderer.SetPositions(laserPoints.ToArray());
        }
        private bool IsPathSame(List<Vector3> newPath, List<Vector3> oldPath)
        {
            if (newPath.Count != oldPath.Count)
                return false;

            for (int i = 0; i < newPath.Count; i++)
            {
                if (Vector3.Distance(newPath[i], oldPath[i]) > 0.01f) // 허용 오차 내에서 비교
                    return false;
            }

            return true;
        }
        #endregion
    }
}
