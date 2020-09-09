using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKManager : MonoBehaviour
{
    [SerializeField]
    LayerMask footMask;

    static public PlayerIKManager main;

    Animator animator;


    Transform lookTarget;
    public Transform nextLookTarget;

    float weight;
    float wMain = 0f;
    float weightMain = 1f;
    public float weightBody = 0.1f;
    public float weightNeck = 0.1f;
    public float weightEye = 0.1f;
    public float weightClamp = 0.1f;

    public RaycastHit handHit_L;
    public bool handFlag_L;
    public RaycastHit handHit_R;
    public bool handFlag_R;


    public float targetHandsWeight = 1f;
    Transform Shoulder_L;
    Transform Shoulder_R;
    float handWeight_L;
    float handWeight_R;




    Transform handLook_L;
    Transform handLook_R;


    public bool isAerial;

    public Vector3 currentPos;

    public Transform handLookParent_L
    {
        set { handLook_L.parent = value; }
    }
    public Transform handLookParent_R
    {
        set { handLook_R.parent = value; }
    }


    Vector3 defaultScale;
    public float HandWeight => Mathf.Max(handWeight_L, handWeight_R);

    Vector3 handPos_L;
    Vector3 handPos_R;


    int handLayer_L;
    int handLayer_R;
    int puth_anmID;
    float puth;

    [SerializeField]
    Vector3 hand_ofs_pos;
    [SerializeField]
    Vector3 hand_ofs_rad;


    float hOfsVel = 0f;
    float hOfs = 0f;
    public float HOfs => hOfs;


    float hOfsWeight = 1f;

    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex != 0) return;


        //対象を見る
        if (lookTarget)
        {
            animator.SetLookAtPosition(lookTarget.position);
            animator.SetLookAtWeight(wMain, weightBody, weightNeck, weightEye, weightClamp);
        }

        //近くにあるものを押す

        puth = Mathf.Lerp(puth, targetHandsWeight * HandWeight, Time.deltaTime);
        animator.SetFloat(puth_anmID, puth);

        UpdateIKHand(ref handWeight_L, ref handPos_L, handFlag_L, AvatarIKGoal.LeftHand, HumanBodyBones.LeftHand, handHit_L, handLayer_L, handLook_L);
        UpdateIKHand(ref handWeight_R, ref handPos_R, handFlag_R, AvatarIKGoal.RightHand, HumanBodyBones.RightHand, handHit_R, handLayer_R, handLook_R);

        //足
        float lhFlag = CheckFootHit(HumanBodyBones.LeftFoot, AvatarIKGoal.LeftFoot);
        float rhFlag = CheckFootHit(HumanBodyBones.RightFoot, AvatarIKGoal.RightFoot);


        float tSpeed = Time.deltaTime * 15f;

        hOfsWeight += isAerial ? -Time.deltaTime*60f : Time.deltaTime*15f;
        hOfsWeight = Mathf.Clamp(hOfsWeight, 0f, 1f);

        hOfsVel = Mathf.Lerp(hOfsVel, Mathf.Max(lhFlag, rhFlag), tSpeed*hOfsWeight);
        hOfsVel = Mathf.Clamp(hOfsVel, 0f, 0.45f);

        hOfs = Mathf.Lerp(hOfs, hOfsVel, tSpeed);




    }


    float CheckFootHit(HumanBodyBones boneF, AvatarIKGoal goal)
    {
        Vector3 footPos = animator.GetIKPosition(goal);
        float footOfs = transform.InverseTransformPoint(footPos).y;
        float feetOfs = (goal == AvatarIKGoal.LeftFoot ? animator.leftFeetBottomHeight : animator.rightFeetBottomHeight);

        float RayDistance = 1f;
        Ray ray = new Ray(footPos + (transform.up*RayDistance) , -transform.up);

        Debug.DrawLine(footPos , transform.position + new Vector3(0f, footOfs, 0f) , Color.red);

        float w = 0f;
        float res = 0f;
        if (Physics.Raycast(ray, out RaycastHit hit, RayDistance*2f, footMask))
        {
            Vector3 localHitPos = transform.InverseTransformPoint(hit.point);
            res  =  -Mathf.Clamp( localHitPos.y, -1f, 0f);

            Vector3 localFootPos  = transform.InverseTransformPoint(footPos);

            float localHitOfsHeight = localFootPos.y - localHitPos.y;
            localHitOfsHeight = Mathf.Clamp(localHitOfsHeight, 0f, 1f);
            

            w = Mathf.Clamp((0.1f - (localHitOfsHeight - feetOfs)) * 10f, 0f,1f);

            animator.SetIKPosition(goal, hit.point + (/*hit.normal*/transform.up * (footOfs)));
            animator.SetIKRotation(goal, Quaternion.LookRotation(transform.forward, hit.normal));

        }
        else
        {
            res = RayDistance;
            animator.SetIKPosition(goal, animator.GetIKPosition(goal));
            animator.SetIKRotation(goal, animator.GetIKRotation(goal));
        }
        animator.SetIKPositionWeight(goal, w);
        animator.SetIKRotationWeight(goal, w);

        return res;
    }

    float CheckFootHit(HumanBodyBones boneF, AvatarIKGoal goal, bool D)
    {
        Vector3 footPos = animator.GetIKPosition(goal);
        float footOfs = transform.InverseTransformPoint(footPos).y;
        float feetOfs = (goal == AvatarIKGoal.LeftFoot ? animator.leftFeetBottomHeight : animator.rightFeetBottomHeight);


        float r = 0.75f;
        Vector3 uVel = transform.up;
        Ray ray = new Ray(footPos + (uVel * r), (-uVel * (r * 2f)));
        float range = r + feetOfs;

        float hitFlag = 0.5f;

        if (Physics.Raycast(ray, out RaycastHit hit, range + r, footMask))
        {

            float holizonFoot = Vector3.Dot((hit.point - currentPos), transform.up);

            if (holizonFoot > 0f)
            {
                hitFlag = 0f;
            }
            else
            {
                hitFlag = -holizonFoot;
            }

            Vector3 ofs = footPos - hit.point;
            float dot = Vector3.Dot(ofs, hit.normal);
            if (dot < 0f)
            {
                Debug.DrawRay(hit.point, hit.normal, Color.yellow);

                Vector3 p = hit.normal * footOfs;
                Vector3 u = Vector3.ProjectOnPlane(uVel * footOfs, hit.normal);

                animator.SetIKPosition(goal, hit.point + (p + u));
                animator.SetIKRotation(goal, Quaternion.LookRotation(transform.forward, hit.normal));


                //float posW = hit.distance / (range*2f);
                //animator.SetIKPositionWeight(goal, Mathf.Clamp(posW , 0f,1f));
                //Debug.Log("dist:" + hit.distance + "range:" + range + "weight:" + posW);

                Debug.DrawRay(ray.origin, ray.direction * range, Color.red);


            }
            else
            {
                animator.SetIKPosition(goal, animator.GetIKPosition(goal));
                animator.SetIKRotation(goal, animator.GetIKRotation(goal));
            }

        }
        else
        {
            animator.SetIKPosition(goal, animator.GetIKPosition(goal));
            animator.SetIKRotation(goal, animator.GetIKRotation(goal));
        }
        animator.SetIKPositionWeight(goal, 1f);

        float w = ((0.1f - ((footOfs - feetOfs) * 2f)) * 10f);


        animator.SetIKRotationWeight(goal, w);

        hitFlag = Mathf.Clamp(hitFlag, 0f, 0.5f);
        if (isAerial) hitFlag = 0f;

        return hitFlag;

    }

    void UpdateIKHand(ref float handWeight, ref Vector3 goalPos, bool hitFlag, AvatarIKGoal goal, HumanBodyBones hand, RaycastHit hit, int handLayer, Transform tHand)
    {



        //手の開き具合レイヤーのウェイト
        animator.SetLayerWeight(handLayer, handWeight);

        animator.SetIKPositionWeight(goal, handWeight);//ウェイトを設定
        animator.SetIKRotationWeight(goal, handWeight);

        //回転
        animator.SetIKRotation(goal, tHand.rotation);
        //座標
        Vector3 s = (hand == HumanBodyBones.LeftHand) ? Shoulder_L.position : Shoulder_R.position;
        Vector3 ofs = tHand.position - s;
        ofs = Vector3.ClampMagnitude(ofs, 0.43f);

        animator.SetIKPosition(goal, s + ofs);
    }

    public void SetHandTransform(bool isLeft, RaycastHit hit)
    {
        Transform hand = isLeft ? handLook_L : handLook_R;
        float handWeight = isLeft ? handWeight_L : handWeight_R;



        Vector3 look = Quaternion.LookRotation(hit.normal, -transform.up) * Vector3.down;



        //回転
        Vector3 toPos = hit.normal;


        if (isLeft)
        {
            toPos -= transform.right * hand_ofs_rad.y;//くるむかひらくか
        }
        else
        {
            toPos += transform.right * hand_ofs_rad.y;
        }
        hand.rotation = Quaternion.Lerp(hand.rotation, Quaternion.LookRotation(look, toPos), handWeight);

        Debug.DrawRay(hit.point, hit.normal, Color.blue);
        Debug.DrawRay(hit.point, look, Color.green);
        Debug.DrawRay(hit.point, toPos, Color.red);




        //座標
        hand.position = Vector3.Lerp(hand.position, hit.point, handWeight);
        hand.position -= look * hand_ofs_pos.z * handWeight;




    }

    // Start is called before the first frame update
    void Start()
    {
        if (!main) main = this;


        animator = GetComponent<Animator>();

        puth_anmID = Animator.StringToHash("puth");


        handLayer_L = animator.GetLayerIndex("Hand_L Layer");
        handLayer_R = animator.GetLayerIndex("Hand_R Layer");

        Shoulder_L = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        Shoulder_R = animator.GetBoneTransform(HumanBodyBones.RightShoulder);



        handLook_L = new GameObject("HandLook_L").transform;
        handLook_R = new GameObject("HandLook_R").transform;

        handLook_L.position = animator.GetBoneTransform(HumanBodyBones.LeftHand).position;
        handLook_R.position = animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        handLook_L.parent = transform;
        handLook_R.parent = transform;


        defaultScale = transform.localScale;



    }

    // Update is called once per frame
    void Update()
    {
        if (nextLookTarget != lookTarget)
        {
            weight -= Time.deltaTime;
            if (weight <= 0f)
            {
                weight = 0f;
                lookTarget = nextLookTarget;
            }
        }
        else
        {
            weight += Time.deltaTime * 2f;
            if (weight > 1f)
            {
                weight = 1f;
            }
        }
        if (lookTarget)
        {
            weightMain = (Vector3.Dot(transform.forward, (lookTarget.position - transform.position).normalized) + 9f) / 10f;
        }
        else
        {
            weightMain = Mathf.Lerp(weightMain, 0f, Time.deltaTime);
        }

        wMain = Mathf.SmoothStep(0f, weightMain, weight);

        //IKウェイト
        handWeight_L = Mathf.Lerp(handWeight_L, handFlag_L ? targetHandsWeight : 0f, Time.deltaTime * 10f);//触れられるならウェイト上昇、フレラレナイならウェイト減少
        handWeight_R = Mathf.Lerp(handWeight_R, handFlag_R ? targetHandsWeight : 0f, Time.deltaTime * 10f);//触れられるならウェイト上昇、フレラレナイならウェイト減少



        //足

        //サイズを固定化
        Vector3 lossScale = transform.lossyScale;
        Vector3 localScale = transform.localScale;
        transform.localScale = new Vector3(
                        localScale.x / lossScale.x * defaultScale.x,
                        localScale.y / lossScale.y * defaultScale.y,
                        localScale.z / lossScale.z * defaultScale.z);


    }

    Vector3 prevHipPos;
    private void LateUpdate()
    {
        //Transform chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        //chest.transform.rotation = Camera.main.transform.rotation;
        //腰位置
        //Transform hip = animator.GetBoneTransform(HumanBodyBones.Hips);
        //hip.transform.position = Vector3.Lerp(prevHipPos, hip.transform.position, Time.deltaTime);
        //prevHipPos = hip.position;

    }
}
