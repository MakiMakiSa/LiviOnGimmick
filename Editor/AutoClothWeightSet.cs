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

        if (GUILayout.Button("自動でウェイト設定", GUILayout.Width(128), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
        {
            if(obj)
            {
                obj.coefficients = SetAutoWeight(obj);
                Debug.Log("セット完了");
            }
            else
            {
                Debug.Log("Clothなし");
            }
        }
    }



    /// <summary>
    /// ClothのMaxDistanceを自動で設定する
    /// </summary>
    /// <param name="cloth"></param>
    /// <returns></returns>
    ClothSkinningCoefficient[] SetAutoWeight(Cloth cloth)
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

            float w = Mathf.Lerp(maxClothDistance , 0f, Mathf.Abs((h - minH) / maxH));

            CSC[i].maxDistance = w;
            CSC[i].collisionSphereDistance = 0f;
        }

        return CSC;

    }



}
