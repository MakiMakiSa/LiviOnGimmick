using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoConvex : EditorWindow
{

    [MenuItem("Window/AutoConvex")]
    static void Open()
    {
        EditorWindow.GetWindow<AutoConvex>("AutoConvex");
    }

    Transform rootObj;
    int flag;

    [System.Obsolete]
    void OnGUI()
    {
        EditorGUILayout.LabelField("Please Set Target Cloth");

        rootObj = (Transform)EditorGUILayout.ObjectField(rootObj, typeof(Transform), true);
        flag = EditorGUILayout.IntField(flag);


        if (GUILayout.Button("全ての子のConvexを"+flag+"にする", GUILayout.Width(128), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
        {
            if (rootObj)
            {
                Convex(rootObj);
                Debug.Log("セット完了");
            }
            else
            {
                Debug.Log("Mesh なし");
            }
        }
    }


    void Convex(Transform root)
    {
        foreach(Transform t in root)
        {
            MeshCollider mc = t.GetComponent<MeshCollider>();
            if(mc)
                mc.convex = flag != 0 ? true:false;
                mc.convex = flag != 0 ? true:false;

            {
                mc.convex = flag != 0 ? true:false;
            }
            Convex(t);
        }
    }


}
