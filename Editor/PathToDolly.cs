using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

public class PathToDolly : EditorWindow
{
    [MenuItem("Window/PathToDolly")]
    static void Open()
    {
        EditorWindow.GetWindow<PathToDolly>("PathToDolly");
    }

    Mesh mesh;


    [System.Obsolete]
    void OnGUI()
    {
        EditorGUILayout.LabelField("Please Set Target Cloth");

        mesh = (Mesh)EditorGUILayout.ObjectField( mesh, typeof(Mesh),true);

        try
        {
            if (GUILayout.Button("GO", GUILayout.Width(256), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
            {
                GameObject par = new GameObject("Dollys");
                var csp = par.AddComponent<CinemachineSmoothPath>();

                int i = 0;

                List<CinemachineSmoothPath.Waypoint> vs = new List<CinemachineSmoothPath.Waypoint>();
                foreach(var m in  mesh.vertices)
                {
                    CinemachineSmoothPath.Waypoint wp;
                    wp.position = m;
                    wp.roll = 0;
                    vs.Add(wp);
                }
                csp.m_Waypoints = vs.ToArray();
            }

        }
        catch
        {
            Debug.Log("Err");
        }
    }



}
