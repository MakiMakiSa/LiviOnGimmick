using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

    [DefaultExecutionOrder(-1000)]
public class CameraManager : MonoBehaviour
{
    static public CameraManager main;

    public Transform lookTarget;

    public Transform target;
    [SerializeField]
    Rigidbody targetRB;
    Rigidbody rb;

    [SerializeField] float upOfs = 20f;
    [SerializeField] float zOfs = 1f;
    [SerializeField] float radHeight = 2f;

    float hOffset;

    public bool ResetFlag = false;


    Vector3 AccelVector;

    float FPSWeight = 0f;

    public float speed = 1f;
    float defaSpeed;

    [SerializeField]
    LayerMask mask;


    enum ZoomType
    {
        NOT,
        LEFT,
        RIGHT
    }
    ZoomType zoomType = ZoomType.NOT;


    List<Transform> D2Lines = new List<Transform>();
    public void SetD2Lines(Transform[] lineMaster)
    {
        D2Lines = new List<Transform>();
        foreach (Transform t in lineMaster)
        {
            D2Lines.Add(t);
        }
    }

    public enum CamMode
    {
        LockPosRad,
        LockPosLookRad,
        D3,
        D2RadLock,
        D2RadLook
    }
    CamMode camMode;




    // Use this for initialization

    void Start()
    {
        if(!main)
        {
            main = this;
        }


        defaSpeed = speed;
        gameObject.layer = LayerMask.NameToLayer("Camera");

        rb = Camera.main.transform.GetComponent<Rigidbody>();


        //CinemachineVirtualCamera CVC = Camera.main.GetComponent<CinemachineVirtualCamera>();
        //CVC.LookAt = target;
        //CVC.Follow = camChild;


        //Camera.main.transform.parent = camChild;
        //Camera.main.transform.localPosition = Vector3.zero;
        //Camera.main.transform.localEulerAngles = Vector3.zero;




        defaZoom = Camera.main.fieldOfView;


        Animator animator = target.GetComponent<Animator>();
        if (animator)
        {
            Transform HeadTrans = Mathaf.FindChildAll( animator.transform, "EdiBone_F");
            if(HeadTrans)
            {
                target = HeadTrans;
            }
//            target = animator.GetBoneTransform(HumanBodyBones.Neck);

            transform.position = target.position;

        }


        //        PPV = Camera.main.GetComponent<PostProcessVolume>();
        //        Camera.main.GetComponent<PostProcessVolume>().profile.TryGetSettings<Vignette>(out vignette);
    }

    void Move_Look()
    {


    }
    void Move_LockPos()
    {
    }
    void Move_LockRad()
    {


    }

    void Move_2D()
    {

    }

    public Transform Scope;
    bool zoomFlag;
    float zoomValue = 1f;
    float nowZoomValue = 1f;
    float targetSpeed = 0f;
    public void SetZoom(bool flag, float value)
    {
        zoomFlag = flag;
        zoomValue = value;
    }
    public void ResetZoom()
    {
        zoomFlag = false;
    }
    public float defaZoom;
    float targetExWeight = 0f;

    public float AimRand
    {
        get { return 1f - targetExWeight; }
    }

    public bool UpperFlag = false;
    Vector3 rsVel;
    void Move_3D()
    {

        //回転
        if(lookTarget == null)
        {
            Vector3 v = KeyManagerHub.main.GetStickVel(false);
            if (UpperFlag)
            {
                v.y = 1f;
            }
            Vector3 vv = v;
            v.x = vv.y;
            v.y = vv.x;

            rsVel += v;
            rsVel /= 1f + (Time.deltaTime * 10f);


            Vector3 e = Camera.main.transform.eulerAngles;

            e += rsVel * speed;
            e.x = Mathaf.NormalizeRad(e.x);
            if (e.x > 75f) e.x = 75f;
            if (e.x < -75f) e.x = -75f;

            Camera.main.transform.eulerAngles = e;
        }
        else
        {
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, lookTarget.rotation, Time.deltaTime * 15f);
        }


        //移動
        Vector3 pos = target.position + (UpperFlag ? Vector3.up:Vector3.zero);

        pos -= Camera.main.transform.forward * zOfs * zoomValue;

        if(lookTarget != null)
        {
            pos -= Camera.main.transform.forward * 10f;
        }




        Vector3 r = Camera.main.transform.eulerAngles;
        r.x = Mathaf.NormalizeRad(r.x);
        if (r.x > 75f) r.x = 75f;
        if (r.x < -75f) r.x = -75f;

        pos += target.up * upOfs;
        pos += target.up * radHeight * r.x;

        Vector3 rbv = targetRB.velocity;

        float d = Vector3.Dot(rbv.normalized, Camera.main.transform.forward);
        if (d < 0f) d = 0f;
        d = Mathf.Abs(d - 1f)*0.5f;

        AccelVector = Vector3.Lerp(AccelVector, rbv * d, Time.deltaTime);

        pos += AccelVector;



        Vector3 fVel = (pos - Camera.main.transform.position);
        fVel.y *= 0.5f;
        rb.AddForce( fVel * Mathf.Clamp(Time.deltaTime*100f , 0f,1f), ForceMode.VelocityChange);
//        Camera.main.transform.position = pos;

    }
    Vector3 recoilMem = Vector3.zero;
    Vector3 recoilVel = Vector3.zero;
    float recoilTime = 0f;
    float recoilInv = 0f;
    float recoilInvTime = 0f;
    const float minRecoil = 0.3f;

    public void Recoil(float h, float l, float r, int RecoilPow)
    {
        recoilInv -= 1f / RecoilPow;
        recoilInv = Mathf.Clamp(recoilInv, 0.1f, 1f);
        recoilVel.x = h * recoilInv;
        recoilVel.y = UnityEngine.Random.Range(-l * (recoilInv + minRecoil), r * (recoilInv + minRecoil));
        recoilMem += recoilVel * 0.1f;
        recoilMem = recoilMem * 0.5f * recoilInv;
        recoilInvTime = 1f;
        recoilTime = 0.1f;
    }


    void Bump()
    {
        Vector3 aPos = targetRB.position;
        Vector3 bPos = Camera.main.transform.position;

        Ray ray = new Ray(aPos, bPos - aPos);
        float cameraPosDistanceFromA = Vector3.Distance(aPos, bPos);
        float radius = 0.05f;


        Vector3 nearNormal = Vector3.zero;

        bool hitFlag = false;
        RaycastHit hit = new RaycastHit();

        foreach (RaycastHit h in Physics.SphereCastAll(ray, radius, cameraPosDistanceFromA, mask , QueryTriggerInteraction.Ignore))
        {
            if (!hitFlag)
            {
                hitFlag = true;
                hit = h;
            }
            else
            {
                float aDistance = Vector3.Distance(hit.point, aPos);
                float bDistance = Vector3.Distance(h.point, aPos);

                if (aDistance > bDistance)
                {
                    hit = h;
                }
            }

        }


        if (hitFlag)
        {
            Vector3 NearPos = Mathaf.NearPosOnLine(hit.point, aPos, bPos, false);
            float intrusion = (radius - Vector3.Distance(NearPos, hit.point)) / radius;

            nearNormal = (NearPos - hit.point).normalized * intrusion * radius;



            float nearPosDistanceFromA = Vector3.Distance(aPos, NearPos) - radius;

            float percentage = 1f - (nearPosDistanceFromA / (cameraPosDistanceFromA));

            FPSWeight = percentage;

            Debug.DrawLine(aPos, aPos + Vector3.up, new Color(255f, 0f, 0f));

        }
        else
        {
            FPSWeight -= Time.deltaTime;
        }

        FPSWeight = Mathf.Clamp(FPSWeight, 0f, 1f);

        if (aPos.y < 10.1f) aPos.y = 10.1f;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, aPos, FPSWeight) + nearNormal;



    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Move_3D();
//        Bump();
        UpperFlag = false;
        lookTarget = null;
    }
}
