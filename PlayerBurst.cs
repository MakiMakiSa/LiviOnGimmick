using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;

[DefaultExecutionOrder(-100)]
public class PlayerBurst : MonoBehaviour
{
    [SerializeField]
    Transform targetObj;


    [SerializeField]
    SDManager SD;

    [SerializeField]
    VisualEffect ve;

    [SerializeField]
    CamTargetManager ctm;

    Animator animator;
    static int sid_Dissolve = Shader.PropertyToID("Dissolve");

    public List<Renderer> DissolveRenderers = new List<Renderer>();

    public bool dissolveFlag = true;
    float dissolve = 1f;

//    public List<SpringManager> springManagers = new List<SpringManager>();
    public List<Cloth> cloths = new List<Cloth>();

    // Start is called before the first frame update
    void Start()
    {
        animator = targetObj.GetComponent<Animator>();

//        springManagers.AddRange(targetObj.GetComponentsInChildren<SpringManager>());
        cloths.AddRange(targetObj.GetComponentsInChildren<Cloth>());


        //Color col = meshRenderer.material.color;
        //ve.SetTexture("ColorTexture", MeshVertexsTo2DMap.MeshToMap_Color_Init(meshRenderer.sharedMesh , col));

        DissolveRenderers.AddRange(animator.gameObject.GetComponentsInChildren<Renderer>());



        ////////////

        if (dissolveFlag) dissolve = 1f;
        var mpb = new MaterialPropertyBlock();
        mpb.SetFloat(sid_Dissolve, dissolve);
        foreach (Renderer r in DissolveRenderers)
        {
            r.SetPropertyBlock(mpb);
        }

    }

    // Update is called once per frame
    void Burst()
    {

        MapManager.MeshToColorMap(targetObj.gameObject, out Texture2D posBuff, /*out Texture2D norBuff,out Texture2D uvBuff ,*/ true, 1f);
        ve.SetTexture("PositionTexture", posBuff);
        //ve.SetTexture("NormalTexture", norBuff);
        //ve.SetTexture("UVTexture", uvBuff);
        ve.SetInt("ParticleCount", posBuff.width * posBuff.height);

        ve.SendEvent("Burst");



        //        MeshCreate.CreateFromMeshCollider(animator.gameObject , LayerMask.NameToLayer("Player"));

    }


    void Hide()
    {
        targetObj.gameObject.SetActive(false);

    }

    private void FixedUpdate()
    {

        if (SD.EnterInPlayerFlag)
        {
            Burst();
            Invoke("Hide", 0.1f);
            dissolveFlag = true;
            animator.enabled = false;
            //でぃそるぶで消す
//            foreach (SpringManager sm in springManagers) { sm.enabled = false; }
            foreach (Cloth c in cloths) { c.enabled = false; }
        }


        if (SD.ExitInPlayerFlag)
        {
            CancelInvoke("Hide");
            targetObj.transform.position = SD.transform.position;
            animator.enabled = true;
//            foreach (SpringManager sm in springManagers) { sm.enabled = true; }
            foreach (Cloth c in cloths) { c.enabled = true; }
            targetObj.gameObject.SetActive(true);
            dissolveFlag = false;
        }
    }
    private void Update()
    {

        float timeSpeed = 2f;
        dissolve += dissolveFlag ? Time.deltaTime * timeSpeed * 15f : -Time.deltaTime * timeSpeed;
        dissolve = Mathf.Clamp(dissolve, -1f, 1f);


        var mpb = new MaterialPropertyBlock();
        mpb.SetFloat(sid_Dissolve, dissolve);
        foreach (Renderer r in DissolveRenderers)
        {
            r.SetPropertyBlock(mpb);
        }


        ve.SetVector3("toPos", dissolveFlag ? transform.InverseTransformPoint(SD.transform.position) : transform.InverseTransformPoint(targetObj.transform.position));


        if (dissolveFlag)
        {
            ctm.ballWeight += Time.deltaTime * 3f;
        }
        else
        {
            ctm.ballWeight = 0f;
        }
        ctm.ballWeight = Mathf.Clamp(ctm.ballWeight, 0f, 1f);
    }








}
