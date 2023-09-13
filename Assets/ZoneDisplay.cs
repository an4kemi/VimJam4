using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZoneDisplay : MonoBehaviour
{
    public List<Zone> Zones;
#if UNITY_EDITOR
    public bool RenderGizmos;
    private void OnDrawGizmos()
    {
        if (!RenderGizmos) return;
        foreach (var zone in Zones)
        {
            Handles.Label(zone.Position, zone.Name);
            Gizmos.color = zone.Color;
            Gizmos.DrawCube(zone.Position, zone.Size);
        }
    }
#endif
}

[Serializable]
public class Zone
{
    public Vector3 Position;
    public Vector3 Size;
    public Color Color;
    public string Name;
}