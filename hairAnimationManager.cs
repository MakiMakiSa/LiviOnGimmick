using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hairAnimationManager : MonoBehaviour
{
    Animator animator;

    static public hairAnimationManager main;


    [SerializeField]
    string parentHairBoneName;

    [SerializeField]
    Transform hairObject;


    Transform hairBone;


    int animID_UD;float UD;
    int animID_LR;float LR;
    int animID_FB;float FB;

    void FindChildAll(Transform p)
    {

        if(p.name == parentHairBoneName)
        {
            hairBone = p;
        }
        else
        {
            foreach (Transform t in p)
            {
                FindChildAll(t);
            }
        }
    }




    void InitAnimatorID()
    {
        animID_FB = Animator.StringToHash("FB");
        animID_UD = Animator.StringToHash("UD");
        animID_LR = Animator.StringToHash("LR");
    }

    void UpdateAnimatorID()
    {
        Vector3 downVel = hairObject.InverseTransformDirection(Vector3.down);

        FB = downVel.z;
        LR = downVel.x;
        UD = downVel.y;

        //animator.SetFloat(animID_FB, FB);
        //animator.SetFloat(animID_UD, UD);
        //animator.SetFloat(animID_LR, LR);

    }    // Start is called before the first frame update
    void Start()
    {
        if (!main) main = this;


        animator = hairObject.GetComponent<Animator>();
        InitAnimatorID();



        FindChildAll(transform.parent);
        hairObject.parent = hairBone;
        hairObject.localPosition = Vector3.zero;//Vector3.up * 0.02f;//職人補正
        hairObject.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateAnimatorID();
    }
}
