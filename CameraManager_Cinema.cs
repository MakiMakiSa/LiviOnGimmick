using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[DefaultExecutionOrder(-100)]
public class CameraManager_Cinema : MonoBehaviour
{
    static public CameraManager_Cinema main;

    [SerializeField]
    LayerMask lookLayer;
    Vector3 targetPos;
    public Vector3 TargetPos => targetPos;

    [SerializeField]
    CinemachineVirtualCamera cvc;
    CinemachineFramingTransposer cft;
    
    PitPoint currentPit => SDManager.main.CurrentPit;


    float defaultScreenX;
    float defaultScreenY;
    float defaultDistance;
    // Start is called before the first frame update
    void Start()
    {
        if (!main) main = this;

        cft = cvc.GetCinemachineComponent<CinemachineFramingTransposer>();
        defaultScreenX = cft.m_ScreenX;
        defaultScreenY = cft.m_ScreenY;
        defaultDistance = cft.m_CameraDistance;


    }

    // Update is called once per frame
    void Update()
    {


        ///////
        if (currentPit && currentPit.isRideOn)
        {
            cft.m_ScreenX = currentPit.CameraOffset.x;
            cft.m_ScreenY = currentPit.CameraOffset.y;
            cft.m_CameraDistance = currentPit.CameraOffset.z;

        }
        else
        {
            cft.m_ScreenX = defaultScreenX;
            cft.m_ScreenY = defaultScreenY;
            cft.m_CameraDistance = defaultDistance;

        }


        Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(camRay, out RaycastHit hit, 10000f, lookLayer))
        {
            targetPos = hit.point;
        }


    }
}
