using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoClothWeightSet : EditorWindow
{
    [MenuItem("Window/AutoCloth")]
    static void Open()
    {
        EditorWindow.GetWindow<AutoClothWeightSet>("AutoCloth");
    }

    Cloth obj;

    [Range(0f, 1f)]
    float offset = 0.5f;

    float maxClothDistance = 10f;

    [System.Obsolete]
    void OnGUI()
    {
        EditorGUILayout.LabelField("Please Set Target Cloth");

        obj = (Cloth)EditorGUILayout.ObjectField( obj, typeof(Cloth),true);
        offset = EditorGUILayout.FloatField(offset);
        maxClothDistance = EditorGUILayout.FloatField(maxClothDistance);

        try
        {
            if (GUILayout.Button("自動でウェイト設定_Smooth", GUILayout.Width(256), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
            {
                obj.coefficients = SetAutoWeight(obj, 0);
            }
            if (GUILayout.Button("自動でウェイト設定_Inverse", GUILayout.Width(256), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
            {
                obj.coefficients = SetAutoWeight(obj, 1);
            }
            if (GUILayout.Button("自動でウェイト設定_Liner", GUILayout.Width(256), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
            {
                obj.coefficients = SetAutoWeight(obj, 2);
            }

        }
        catch
        {
            Debug.Log("Err");
        }
    }



    /// <summary>
    /// ClothのMaxDistanceを自動で設定する
    /// </summary>
    /// <param name="cloth"></param>
    /// <returns></returns>
    ClothSkinningCoefficient[] SetAutoWeight(Cloth cloth , int mode)
    {
        float maxH = float.MinValue;
        float minH = float.MaxValue;

        foreach (Vector3 v in cloth.vertices)
        {
            if (v.z > maxH) maxH = v.z;
            if (v.z < minH) minH = v.z;
        }

        maxH -= minH;

        Debug.Log("max="+maxH + "min="+minH);
        ClothSkinningCoefficient[] CSC = new ClothSkinningCoefficient[cloth.coefficients.Length];

        maxH -= (maxH * offset);

        for (int i = 0; i < cloth.coefficients.Length; i++)
        {
            float h = cloth.vertices[i].z;

            float w = 0f;
            switch(mode)
            {
                case 0:{w = Mathf.SmoothStep(maxClothDistance, 0f, Mathf.Abs((h - minH) / maxH));
                    }break;
                case 1:{w = Mathf.InverseLerp(maxClothDistance, 0f, Mathf.Abs((h - minH) / maxH));
                    }break;
                default:{w = Mathf.Lerp(maxClothDistance, 0f, Mathf.Abs((h - minH) / maxH));
                    }break;
            }

            CSC[i].maxDistance = w;
            CSC[i].collisionSphereDistance = 0f;
        }

        return CSC;

    }



}
