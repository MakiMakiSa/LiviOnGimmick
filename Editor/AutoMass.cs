using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoMass : EditorWindow
{

    [MenuItem("Window/AutoMass")]
    static void Open()
    {
        EditorWindow.GetWindow<AutoMass>("AutoMass");
    }

    Rigidbody obj;

    [System.Obsolete]
    void OnGUI()
    {
        EditorGUILayout.LabelField("Please Set Target Cloth");

        obj = (Rigidbody)EditorGUILayout.ObjectField(obj, typeof(Rigidbody), true);


        if (GUILayout.Button("自動で質量設定", GUILayout.Width(128), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
        {
            MeshFilter mf = obj.GetComponent<MeshFilter>();
            Mesh mesh = mf.mesh;

            if (obj && mesh)
            {
                obj.mass = VolumeOfMesh(mesh);
                Debug.Log("セット完了");
            }
            else
            {
                Debug.Log("Mesh なし");
            }
        }
    }


    float VolumeOfMesh(Mesh mesh)
    {
        if (mesh == null) return 0;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float volume = 0;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }

    float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        p1.x *= obj.transform.lossyScale.x;
        p1.y *= obj.transform.lossyScale.y;
        p1.z *= obj.transform.lossyScale.z;

        p2.x *= obj.transform.lossyScale.x;
        p2.y *= obj.transform.lossyScale.y;
        p2.z *= obj.transform.lossyScale.z;

        p3.x *= obj.transform.lossyScale.x;
        p3.y *= obj.transform.lossyScale.y;
        p3.z *= obj.transform.lossyScale.z;

        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }


}
