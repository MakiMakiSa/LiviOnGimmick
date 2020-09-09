using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneColliderManager : MonoBehaviour
{
    public bool debF = false;
    [SerializeField]
    PhysicMaterial physicsMaterial;


    List<BoneCollider> cols = new List<BoneCollider>();


    private void OnDisable()
    {
        debF = true;
        foreach (BoneCollider c in cols)
        {
            c.isTrigger = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponent<Animator>();

        Transform hip = animator.GetBoneTransform(HumanBodyBones.Hips);
        Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        Transform chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        Transform upperchest = animator.GetBoneTransform(HumanBodyBones.UpperChest);
        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);

        Transform leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        Transform leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);

        Transform leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        Transform leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        Transform leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

        Transform rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        Transform rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Transform rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

        Transform rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Transform rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        Transform rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

        List<Transform> endTrans = new List<Transform>();
        endTrans.Add(head);
        //endTrans.Add(leftFoot);
        //endTrans.Add(rightFoot);
        //endTrans.Add(leftHand);
        //endTrans.Add(rightHand);

        List<Transform> spineTrans = new List<Transform>();
        spineTrans.Add(hip);
        spineTrans.Add(spine);
        spineTrans.Add(chest);
        spineTrans.Add(upperchest);

        List<Transform> bodyTrans = new List<Transform>();
        int maxBoneNum = System.Enum.GetValues((typeof(HumanBodyBones))).Length;
        for (int i = 0; i < maxBoneNum - 1; i++)
        {
            bodyTrans.Add(animator.GetBoneTransform((HumanBodyBones)i));
        }

        Adds(hip, bodyTrans, endTrans, spineTrans);

        //頭だけは球体コライダ
        AddSphere(head, new Vector3(0f, 0.1f, 0f), 0.145f);
    }


    void Adds(Transform parent, List<Transform> bodyTrans, List<Transform> endTrans, List<Transform> spineTrans)
    {
        //終了ボーンに到達したら終了
        foreach (Transform t in endTrans)
        {
            if (parent == t) return;
        }

        //現在ボーンがUnityHumanoidRigに含まれていなかったら終了
        bool okFlag = false;
        foreach (Transform t in bodyTrans)
        {
            if (parent == t) okFlag = true;
        }
        if (!okFlag) return;





        //メイン
        List<Transform> trans = new List<Transform>();
        foreach (Transform t in parent)
        {
            Adds(t, bodyTrans, endTrans, spineTrans);
            trans.Add(t);
        }

        foreach (Transform t in trans)
        {
            bool flag = false;
            foreach (Transform s in spineTrans)
            {
                if (t == s) flag = true;
            }
            Add(parent, t, flag ? 1f : 0.2f);
        }
    }

    void Add(Transform a, Transform b, float radius = 0.2f)
    {
        GameObject obj = new GameObject();

        obj.layer = LayerMask.NameToLayer("BoneCollider");

        obj.transform.position = (a.position + b.position) / 2f;
        obj.transform.LookAt(b);
        obj.transform.parent = a;
        obj.name = "BCollider" + a.name;



        CapsuleCollider cc = obj.AddComponent<CapsuleCollider>();
        float d = Vector3.Distance(a.position, b.position);

        cc.material = physicsMaterial;

        cc.radius = d * radius;
        cc.height = d + cc.radius;
        cc.direction = 2;

        cols.Add(obj.AddComponent<BoneCollider>());
    }

    void AddSphere(Transform a, Vector3 offset, float radius = 0.2f)
    {
        GameObject obj = new GameObject();

        obj.layer = LayerMask.NameToLayer("BoneCollider");

        obj.transform.position = a.position;
        obj.transform.parent = a;
        obj.name = "BCollider_Sphere" + a.name;


        SphereCollider sc = obj.AddComponent<SphereCollider>();
        sc.radius = radius;
        sc.center = offset;

        cols.Add(obj.AddComponent<BoneCollider>());
    }

    // Update is called once per frame
}
