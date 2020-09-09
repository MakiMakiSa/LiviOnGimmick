using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[DefaultExecutionOrder(10000)]
public class LovoManager : MonoBehaviour
{
    [SerializeField]
    LayerMask lookLayer;

    [SerializeField]
    Transform TailGun;
    [SerializeField]
    Bullet TailGun_Bullet;
    float recoil = 0f;
    float recoilAccel = 0f;


    [SerializeField]
    Rigidbody parentRB;

    Animator animator;
    int anmID_Vel_X = Animator.StringToHash("vel_X");
    int anmID_Vel_Z = Animator.StringToHash("vel_Z");
    int anmID_Speed = Animator.StringToHash("speed");
    int anmID_RideOn = Animator.StringToHash("rideOn");
    int anmID_Rad = Animator.StringToHash("rad");

    bool RideOnFlag => pit.isRideOn;
    PitPoint pit;


    float moveWeight = 0f;
    float vel_X;
    float vel_Z;

    float moveAnimationSpeed => Time.deltaTime * 3f;

    LineRenderer lr;
    Vector3 aimPos;

    [SerializeField]
    Transform Tale;
    public List<Transform> Tales = new List<Transform>();
    Dictionary<Transform, Quaternion> ikTales = new Dictionary<Transform, Quaternion>();

    void InitTale(Transform p)
    {
        foreach (Transform t in p)
        {
            if (t.childCount > 0)
            {
                Tales.Add(t);
                InitTale(t);
            }
        }
    }



    private void OnAnimatorMove()
    {
        //        transform.position += animator.velocity;
        Vector3 vel = animator.velocity;
        parentRB.velocity = new Vector3(vel.x, parentRB.velocity.y, vel.z);
        parentRB.angularVelocity = new Vector3(0f, animator.angularVelocity.y, 0f);
    }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        pit = GetComponentInChildren<PitPoint>();


        InitTale(Tale);


        for (int i = 0; i < Tales.Count; i++)
        {
            Transform t = Tales[i];

            Transform ik = new GameObject("IKObj").transform;
            ik.parent = t.parent;
            ik.transform.localPosition = t.localPosition;

            Transform dIK = new GameObject("dIKObj").transform;
            dIK.position = t.parent.position;
            dIK.rotation = t.parent.rotation;

            if (i + 1 < Tales.Count)
            {
                ik.LookAt(Tales[i + 1]);
            }
            else
            {
                ik.LookAt(ik.position + (ik.position - Tales[i - 1].position));
            }
            dIK.parent = ik;
            t.parent = dIK;

            ikTales.Add(ik, ik.localRotation);
        }



        lr = gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetBool(anmID_RideOn, RideOnFlag);

        float toVel_X;
        float toVel_Z;

        if (RideOnFlag)
        {
            toVel_Z = Vector3.Dot(transform.forward, KeyManagerHub.main.GetStickVectorFromCamera(true));
            toVel_X = Vector3.Dot(transform.right, KeyManagerHub.main.GetStickVectorFromCamera(true));
            moveWeight += Time.deltaTime * 0.3f;

            //射撃
            if (KeyManagerHub.main.GetKey(KeyMode.Enter, Key_Hub.R1))
            {
                Bullet b = GameObject.Instantiate(TailGun_Bullet);

                b.transform.SetPositionAndRotation(TailGun.transform.position, Quaternion.LookRotation(aimPos - TailGun.transform.position));
                b.Init();

                recoilAccel = 0.1f;
            }
        }
        else
        {
            toVel_Z = 0f;
            toVel_X = 0f;
            moveWeight -= Time.deltaTime * 0.3f;
        }
        moveWeight = Mathf.Clamp(moveWeight, 0f, 1f);


        vel_Z = Mathf.Lerp(vel_Z, toVel_Z, moveAnimationSpeed);
        vel_X = Mathf.Lerp(vel_X, toVel_X, moveAnimationSpeed);




        Vector3 vel = new Vector3(vel_X, 0f, vel_Z);


        //animator.SetLayerWeight(1, vel.z * moveWeight);
        //animator.SetLayerWeight(2, vel.x * moveWeight);



        //リコイル減衰
        recoil += recoilAccel;
        recoil = Mathf.Lerp(recoil, 0f, Time.deltaTime * 15f);
        if (recoil > 0f) recoil -= Time.deltaTime * 10f;

        recoilAccel = Mathf.Lerp(recoilAccel, 0f, Time.deltaTime * 15f);
        if (recoilAccel > 0f) recoilAccel -= Time.deltaTime * 15f;

        recoil = Mathf.Clamp(recoil, 0f, 1f);
        recoilAccel = Mathf.Clamp(recoilAccel, 0f, 1f);


        //尾Look

        Quaternion lookRad = Quaternion.identity;


        float dot = Vector3.Dot(Camera.main.transform.forward, transform.forward);
        dot *= 2f;
        dot = Mathf.Clamp(dot, 0f, 1f);

        float i = 0f;
        foreach (KeyValuePair<Transform, Quaternion> d in ikTales)
        {
            float w = i / (float)ikTales.Count;

            float revW = (ikTales.Count - i) / ikTales.Count;


            Transform t = d.Key;

            Quaternion prevRad = t.localRotation;

            lookRad = Quaternion.LookRotation(CameraManager_Cinema.main.TargetPos - TailGun.position);
            t.rotation = lookRad;//Quaternion.LookRotation(Camera.main.transform.forward , t.up);



            t.localRotation = Quaternion.Lerp(prevRad, Quaternion.Lerp(d.Value, t.localRotation, w * moveWeight * dot), Time.deltaTime * (5f + (w*10f)));


            //リコイル
            Vector3 eRot = t.localEulerAngles;
            eRot.x -= revW * recoil * 100f * dot * moveWeight;
            t.localEulerAngles = eRot;


            i++;
        }

        //照準
        //lr.SetPosition(0, TailGun.position);
        aimPos = Vector3.Lerp(TailGun.position + (TailGun.up * 100f), CameraManager_Cinema.main.TargetPos, dot);
        //lr.SetPosition(1, aimPos);




        //アニメ
        float radOfs = Mathaf.NormalizeRad(lookRad.eulerAngles.y - transform.eulerAngles.y) / 90f;
        animator.SetFloat(anmID_Rad, radOfs);
        animator.SetFloat(anmID_Vel_Z, vel.z);
        animator.SetFloat(anmID_Vel_X, vel.x);
        animator.SetFloat(anmID_Speed, Mathf.Clamp(vel.magnitude + Mathf.Abs(radOfs), 0f, 1f));




    }
}
