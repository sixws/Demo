using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = (MapGenerator)target;
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }
        if (GUILayout.Button("Éú³ÉµØÍ¼"))
        {
            map.GenerateMap();
        }
    }
}
