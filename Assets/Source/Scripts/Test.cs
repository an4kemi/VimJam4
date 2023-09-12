using System;
using Reflex.Attributes;
using Source.Scripts;
using Source.Scripts.Extensions;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private NavigationPoint[] points;
    [Inject] private TestClass _testClass;

    private void OnDrawGizmos()
    {
        for (var i = 0; i < points.Length; i++)
        {
            var navigationPoint = points[i];
            Gizmos.DrawSphere(navigationPoint.point, .5f);

            if (navigationPoint.connections is { Length: > 0 })
            {
                foreach (var connection in navigationPoint.connections)
                {
                    var connectionPointExists = connection < points.Length && connection > -1;
                    if (!connectionPointExists) continue;
                    GizmosUtils.DrawArrow(navigationPoint.point, points[connection].point, 30, .5f, .5f);
                }
            }
        }
    }
}

[Serializable]
public class NavigationPoint
{
    public Vector3 point;
    public int[] connections;
}