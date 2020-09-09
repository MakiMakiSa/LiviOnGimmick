using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoClothWeighter : MonoBehaviour
{


    List<Cloth> cloths = new List<Cloth>();

    [SerializeField]
    Animator rootAnimator;

    List<ClothSphereColliderPair> sphereColliders = new List<ClothSphereColliderPair>();

    SphereCollider[] colliders = new SphereCollider[17];
    float[] defaultRadius = new float[17];


    float startingSize = 0f;

    enum Bones
    {
        Hip,
        ULegL,ULegR,
        LLegL,LLegR,
        FootL,FootR,
        UChestL,UChestR,
        ChestL,ChestR,
        SHoulderL,ShoulderR
    }

    SphereCollider hip { get { return colliders[0]; } set { colliders[0] = value; } }
    SphereCollider uLegL { get { return colliders[1]; } set { colliders[1] = value; } }
    SphereCollider dLegL { get { return colliders[2]; } set { colliders[2] = value; } }
    SphereCollider FootL { get { return colliders[3]; } set { colliders[3] = value; } }

    SphereCollider uLegR { get { return colliders[4]; } set { colliders[4] = value; } }
    SphereCollider dLegR { get { return colliders[5]; } set { colliders[5] = value; } }
    SphereCollider FootR { get { return colliders[6]; } set { colliders[6] = value; } }
    SphereCollider UChestL { get { return colliders[7]; } set { colliders[7] = value; } }
    SphereCollider UChestR { get { return colliders[8]; } set { colliders[8] = value; } }
    SphereCollider ChestL { get { return colliders[9]; } set { colliders[9] = value; } }
    SphereCollider ChestR { get { return colliders[10]; } set { colliders[10] = value; } }
    SphereCollider ShoulderL { get { return colliders[11]; } set { colliders[11] = value; } }
    SphereCollider ShoulderR { get { return colliders[12]; } set { colliders[12] = value; } }

    SphereCollider Arm_U_L { get { return colliders[13]; } set { colliders[13] = value; } }
    SphereCollider Arm_U_R { get { return colliders[14]; } set { colliders[14] = value; } }
    SphereCollider Arm_D_L { get { return colliders[15]; } set { colliders[15] = value; } }
    SphereCollider Arm_D_R { get { return colliders[16]; } set { colliders[16] = value; } }






    void Start()
    {
        


        cloths.AddRange(rootAnimator.GetComponentsInChildren<Cloth>());

        //スフィアコライダアタッチ
        hip = rootAnimator.GetBoneTransform(HumanBodyBones.Hips).gameObject.AddComponent<SphereCollider>();

        uLegL = rootAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).gameObject.AddComponent<SphereCollider>();
        dLegL = rootAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).gameObject.AddComponent<SphereCollider>();
        FootL = rootAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).gameObject.AddComponent<SphereCollider>();

        uLegR = rootAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg).gameObject.AddComponent<SphereCollider>();
        dLegR = rootAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg).gameObject.AddComponent<SphereCollider>();
        FootR = rootAnimator.GetBoneTransform(HumanBodyBones.RightFoot).gameObject.AddComponent<SphereCollider>();

        ChestL = rootAnimator.GetBoneTransform(HumanBodyBones.UpperChest).gameObject.AddComponent<SphereCollider>();
        ChestR = rootAnimator.GetBoneTransform(HumanBodyBones.UpperChest).gameObject.AddComponent<SphereCollider>();

        UChestL = rootAnimator.GetBoneTransform(HumanBodyBones.Neck).gameObject.AddComponent<SphereCollider>();
        UChestR = rootAnimator.GetBoneTransform(HumanBodyBones.Neck).gameObject.AddComponent<SphereCollider>();

        ShoulderL = rootAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm).gameObject.AddComponent<SphereCollider>();
        ShoulderR = rootAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm).gameObject.AddComponent<SphereCollider>();

        Arm_U_L = rootAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm).gameObject.AddComponent<SphereCollider>();
        Arm_U_R = rootAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm).gameObject.AddComponent<SphereCollider>();

        Arm_D_L = rootAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).gameObject.AddComponent<SphereCollider>();
        Arm_D_R = rootAnimator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).gameObject.AddComponent<SphereCollider>();

        //サイズ補正
        uLegL.radius = 1.05f;
        uLegL.center = new Vector3(0.4f, -0.1f, 0.08f);

        uLegR.radius = 1.05f;
        uLegR.center = new Vector3(-0.4f, -0.1f, 0.08f);

        dLegL.radius = 0.55f;
        dLegR.radius = 0.55f;

        FootL.radius = 1.0f;
        FootR.radius = 1.0f;

        ChestL.radius = 0.7f;
        ChestL.center = new Vector3(-0.23f, 0f, 0.23f);
        ChestR.radius = 0.7f;
        ChestR.center = new Vector3( 0.23f, 0f, 0.23f);

        UChestL.radius = 0.8f;
        UChestL.center = new Vector3(0.0f, -0.1f, 0.2f);
        UChestR.radius = 0.8f;
        UChestR.center = new Vector3(0.0f, -0.1f, 0.2f);

        ShoulderL.radius = 0.6f;
        ShoulderL.center = new Vector3(0.23f, -0.1f, 0.2f);
        ShoulderR.radius = 0.6f;
        ShoulderR.center = new Vector3(-0.23f, -0.1f, 0.2f);

        Arm_U_L.radius = 0.3f;
        Arm_U_R.radius = 0.3f;
        Arm_D_L.radius = 0.3f;
        Arm_D_R.radius = 0.3f;

        //ShoulderL.center = new Vector3(0.3f, -0.1f, -0.2f);
        //ShoulderR.center = new Vector3(-0.3f, -0.1f, -0.2f);




        //ペア作成
        //尻と腿の付け根
        //        sphereColliders.Add(new ClothSphereColliderPair(hip, uLegL));
        //        sphereColliders.Add(new ClothSphereColliderPair(hip, uLegR));
        //腿の付け根から腰
        //sphereColliders.Add(new ClothSphereColliderPair(uLegL, ChestL));
        //sphereColliders.Add(new ClothSphereColliderPair(dLegL, ChestR));
        //腰→←
        sphereColliders.Add(new ClothSphereColliderPair(ChestL, ChestR));
        //腿の付け根から膝
        sphereColliders.Add(new ClothSphereColliderPair(uLegL, dLegL));
        sphereColliders.Add(new ClothSphereColliderPair(dLegL, FootL));
        //膝から足
        sphereColliders.Add(new ClothSphereColliderPair(uLegR, dLegR));
        sphereColliders.Add(new ClothSphereColliderPair(dLegR, FootR));
        //尻から腰
        //        sphereColliders.Add(new ClothSphereColliderPair(hip, Spine));
        //腰から胸
        //        sphereColliders.Add(new ClothSphereColliderPair(Spine,Chest));
        //両膝の間を封鎖
        //sphereColliders.Add(new ClothSphereColliderPair(dLegL, dLegR));
        //胸から両腿
        sphereColliders.Add(new ClothSphereColliderPair(ChestL, uLegL));
        sphereColliders.Add(new ClothSphereColliderPair(ChestR, uLegR));
        //肩から両腿
        //sphereColliders.Add(new ClothSphereColliderPair(ShoulderL, uLegL));
        //sphereColliders.Add(new ClothSphereColliderPair(ShoulderR, uLegR));
        //胸から上胸
        sphereColliders.Add(new ClothSphereColliderPair(ChestL, UChestL));
        sphereColliders.Add(new ClothSphereColliderPair(ChestR, UChestR));
        //胸から両肩
//        sphereColliders.Add(new ClothSphereColliderPair(UChestL, ShoulderL));
//        sphereColliders.Add(new ClothSphereColliderPair(UChestR, ShoulderR));
        //両肩連結
//        sphereColliders.Add(new ClothSphereColliderPair(ShoulderL, ShoulderR));

        ////両上腕
        //sphereColliders.Add(new ClothSphereColliderPair(ShoulderL, Arm_U_L));
        //sphereColliders.Add(new ClothSphereColliderPair(ShoulderR, Arm_U_R));
        ////両下腕
        //sphereColliders.Add(new ClothSphereColliderPair(Arm_U_L , Arm_D_L));
        //sphereColliders.Add(new ClothSphereColliderPair(Arm_U_R , Arm_D_R));



        //デフォルト半径を保存しておく
        for(int i=0; i< colliders.Length; i++)
        {
            defaultRadius[i] = colliders[i].radius;
//            colliders[i].radius = 0f;
        }


        //接触判定を消す
        foreach (SphereCollider sc in colliders)
        {
            sc.isTrigger = true;
        }

        //バッファ設定

        foreach(Cloth c in cloths)
        {
            c.sphereColliders = sphereColliders.ToArray();
            c.transform.parent = rootAnimator.GetBoneTransform(HumanBodyBones.Hips);

        }

    }


    // Update is called once per frame
    void Update()
    {
        //if (startingSize >= 1f) return;

        //時間経過で半径を復帰させる
        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    colliders[i].radius = Mathf.Lerp(0f, defaultRadius[i] , startingSize);
        //}
        //startingSize += Time.deltaTime*0.01f;
        //startingSize = Mathf.Clamp(startingSize, 0f, 1f);

    }
}
