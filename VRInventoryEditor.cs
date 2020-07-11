#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(VRInventory))]
public class VRInventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VRInventory inv = (VRInventory)target;
        if (GUILayout.Button("Apply"))
        {
            inv.EditorApply();
        }
    }
}
#endif