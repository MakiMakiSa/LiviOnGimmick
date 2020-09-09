using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoSetClothCollider : EditorWindow
{


    [MenuItem("Window/AutoCloth_Collider")]
    static void Open()
    {
        EditorWindow.GetWindow<AutoSetClothCollider>("AutoCloth_Collider");
    }

    GameObject rootObj;
    float radius;

    [System.Obsolete]
    void OnGUI()
    {
        EditorGUILayout.LabelField("Please Set Target Cloth");

        rootObj = (GameObject)EditorGUILayout.ObjectField(rootObj, typeof(GameObject), true);

        radius = EditorGUILayout.FloatField(radius);

        if (GUILayout.Button("自動でコライダ設定", GUILayout.Width(128), GUILayout.Height(32))) // ボタンの大きさを Width と Height を指定.
        {
            if (rootObj)
            {
                SetAutoClothCollider(rootObj);
                Debug.Log("セット完了");
            }
            else
            {
                Debug.Log("rootObjなし");
            }
        }
    }


    /// <summary>
    /// ClothCollider自動で設定する
    /// </summary>
    /// <param name="cloth"></param>
    /// <returns></returns>
    void SetAutoClothCollider(GameObject obj)
    {
        List<Collider> colliders = new List<Collider>();
        List<ClothSphereColliderPair> sphereColliders = new List<ClothSphereColliderPair>();
        List<Cloth> cloths = new List<Cloth>();
        cloths.AddRange(obj.GetComponentsInChildren<Cloth>());

        //スフィアコライダアタッチ
        Transform rootSpine = Mathaf.FindChildAll(obj.transform, "tweak_spine");

        SphereCollider hipL = rootSpine.gameObject.AddComponent<SphereCollider>();
        SphereCollider hipR = rootSpine.gameObject.AddComponent<SphereCollider>();

        SphereCollider shinL = Mathaf.FindChildAll(obj.transform, "shin_tweak.L").gameObject.AddComponent<SphereCollider>();
        SphereCollider shinR = Mathaf.FindChildAll(obj.transform, "shin_tweak.R").gameObject.AddComponent<SphereCollider>();



        //サイズ補正
        hipL.radius = 0.1f;
        hipR.radius = 0.1f;
        shinL.radius = 0.1f;
        shinR.radius = 0.1f;


        //座標補正
        hipL.center = new Vector3( 0.1f, 0f, 0f);
        hipR.center = new Vector3(-0.1f, 0f, 0f);
        shinL.center = new Vector3(0f, 0f, 0f);
        shinR.center = new Vector3(0f, 0f, 0f);




        //ペア作成
        //尻とひざ
        sphereColliders.Add(new ClothSphereColliderPair(hipL, shinL));
        sphereColliders.Add(new ClothSphereColliderPair(hipR, shinR));
        //右尻左尻
        sphereColliders.Add(new ClothSphereColliderPair(hipL,hipR));


        //コライダをリストに追加する
        colliders.Add(hipL);
        colliders.Add(hipR);
        colliders.Add(shinL);
        colliders.Add(shinR);

        //接触判定を消す
        foreach (SphereCollider sc in colliders)
        {
            sc.isTrigger = true;
        }

        //バッファ設定

        foreach (Cloth c in cloths)
        {
            c.sphereColliders = sphereColliders.ToArray();
            c.transform.parent = rootSpine.transform;

        }


    }

}
