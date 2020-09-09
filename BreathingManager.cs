using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BreathingManager : MonoBehaviour
{
    Animator animator;


    Transform UpperChest;

    float breathingSpeed = 1f;
    float breathingWeight = 1f;

    public bool muteFlag = false;

    int breathingLayer;

    int breathingSpeed_AnimID;

    [SerializeField]
    AnimationCurve upperCurve;
    [SerializeField]
    VisualEffect effect;
    [SerializeField]
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        UpperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);


        breathingLayer = animator.GetLayerIndex("Breathing Layer");

        breathingSpeed_AnimID = Animator.StringToHash("breathingSpeed");


        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        effect.transform.parent = head;
        effect.transform.localPosition = new Vector3(0f,-0.3f,0.05f);
        effect.transform.rotation = Quaternion.identity;

    }


    public void Fatigue(float power)
    {
        if (muteFlag) return;
        breathingWeight += Time.deltaTime * power * 0.1f;
        breathingSpeed += Time.deltaTime * power * 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        //うぃえと減少
        breathingWeight = Mathf.Lerp(breathingWeight, 0.1f, Time.deltaTime * 0.1f);

        if(muteFlag)
        {
            breathingWeight -= Time.deltaTime;
        }
        if (breathingWeight < 0f) breathingWeight = 0;

        //スピード減少
        breathingSpeed = Mathf.Lerp(breathingSpeed, 0.5f, Time.deltaTime * 0.12f);


        animator.SetLayerWeight(breathingLayer, Mathf.Clamp(breathingWeight, 0f, 1f));
        animator.SetFloat(breathingSpeed_AnimID, breathingSpeed);





        //現在のアニメーション情報を取得
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(breathingLayer);

        //呼吸による胸と子のサイズを変更
        UpperChest.transform.localScale = (Vector3.one) * (1f + (upperCurve.Evaluate(currentState.normalizedTime % 1f) * Mathf.Clamp(breathingWeight, 1f, 1.1f)));
        foreach (Transform c in UpperChest)
        {
            c.localScale = new Vector3(1f / UpperChest.localScale.x, 1f / UpperChest.localScale.y, 1f / UpperChest.localScale.z);
        }


        //呼吸VFX
        float breathPower = currentState.normalizedTime % 1f;
        breathPower -= 0.5f;
        if (breathPower < 0f) breathPower = 0f;
        effect.SetFloat("BreathPower", Mathf.Lerp(0f, breathingWeight*0.5f, breathPower*3f));

        Vector3 addVel = effect.transform.InverseTransformVector(rb.velocity);
        effect.SetVector3("AddVel", addVel);
    }
}
