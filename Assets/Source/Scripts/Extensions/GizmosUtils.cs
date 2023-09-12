using UnityEngine;

namespace Source.Scripts.Extensions
{
    public static class GizmosUtils
    {
        public static void DrawArrow(Vector3 from, Vector3 to, float arrowheadAngle, float arrowheadDistance, float arrowheadLength)
        {
            var dir = to - from;
            var arrowPos = from + (dir * arrowheadDistance);
 
            var up = Quaternion.LookRotation(dir) * new Vector3(0f, Mathf.Sin(arrowheadAngle* Mathf.Deg2Rad), -1f) * arrowheadLength;
            var down = Quaternion.LookRotation(dir) * new Vector3(0f, -Mathf.Sin(arrowheadAngle* Mathf.Deg2Rad), -1f) * arrowheadLength;
            var left= Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin(arrowheadAngle* Mathf.Deg2Rad), 0f, -1f) * arrowheadLength;
            var right = Quaternion.LookRotation(dir) * new Vector3(-Mathf.Sin(arrowheadAngle* Mathf.Deg2Rad), 0f, -1f) * arrowheadLength;
 
            var upPos = arrowPos + up;
            var downPos = arrowPos + down;
            var leftPos = arrowPos + left;
            var rightPos = arrowPos + right;
 
            Gizmos.DrawLine(from, to);
 
            Gizmos.DrawRay(arrowPos, up);
            Gizmos.DrawRay(arrowPos, down);
            Gizmos.DrawRay(arrowPos, left);
            Gizmos.DrawRay(arrowPos, right);
 
            Gizmos.DrawLine(upPos, leftPos);
            Gizmos.DrawLine(leftPos, downPos);
            Gizmos.DrawLine(downPos, rightPos);
            Gizmos.DrawLine(rightPos, upPos);
        }
    }
}