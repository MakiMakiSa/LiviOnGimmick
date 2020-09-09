using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class KeyManager_PS4 : MonoBehaviour
{
    public bool isActive = true;

    public bool CamMoveFlag = true;

    //	public GameObject objCam;
    //	public GameObject TPSCam;
    //	public GameObject TPSBumpBoal;
    //	public GameObject SubCamera;
    //	public GameObject HeadSphere;
    //入力系統変数
    public float LStickRadWorld;     //角度
    public float LStickRadLocal;     //角度
    public float LStickLen;      //強度
    public Vector3 LStickVelLocal;        //スティック入力による、ローカルベクトル
    public Vector3 LStickVelWorld;        //スティック入力による、ローカルベクトル
    public bool enterLStick;
    public bool enterLStickUp;
    public bool enterLStickDown;
    public bool enterLStickLeft;
    public bool enterLStickRight;

    public float RStickRadWorld;     //角度
    public float RStickRadLocal;     //角度
    public float RStickLen;     //強度
    public Vector3 RStickVelLocal;        //スティック入力による、ワールドベクトル
    public Vector3 RStickVelWorld;        //スティック入力による、ワールドベクトル
    public bool enterRStick;
    public bool enterRStickUp;
    public bool enterRStickDown;
    public bool enterRStickLeft;
    public bool enterRStickRight;




    public float L2Axis = 0f;
    public float R2Axis = 0f;
    public bool enterL2Axis = false;
    public bool enterR2Axis = false;

    //Key DATA
    public bool enterUP = false;
    public bool enterDOWN = false;
    public bool enterLEFT = false;
    public bool enterRIGHT = false;
    public bool enterSANKAKU = false;
    public bool enterMARU = false;
    public bool enterBATU = false;
    public bool enterSIKAKU = false;
    public bool enterL3 = false;
    public bool enterR3 = false;
    public bool enterL2 = false;
    public bool enterR2 = false;
    public bool enterL1 = false;
    public bool enterR1 = false;
    public bool enterOPTION = false;
    public bool enterSHARE = false;
    public bool enterPAD = false;
    public bool enterPS = false;

    public bool stayUP = false;
    public bool stayDOWN = false;
    public bool stayLEFT = false;
    public bool stayRIGHT = false;
    public bool staySANKAKU = false;
    public bool stayMARU = false;
    public bool stayBATU = false;
    public bool staySIKAKU = false;
    public bool stayL3 = false;
    public bool stayR3 = false;
    public bool stayL2 = false;
    public bool stayR2 = false;
    public bool stayL1 = false;
    public bool stayR1 = false;
    public bool stayOPTION = false;
    public bool staySHARE = false;
    public bool stayPAD = false;
    public bool stayPS = false;

    public bool exitUP = false;
    public bool exitDOWN = false;
    public bool exitLEFT = false;
    public bool exitRIGHT = false;
    public bool exitSANKAKU = false;
    public bool exitMARU = false;
    public bool exitBATU = false;
    public bool exitSIKAKU = false;
    public bool exitL3 = false;
    public bool exitR3 = false;
    public bool exitL2 = false;
    public bool exitR2 = false;
    public bool exitL1 = false;
    public bool exitR1 = false;
    public bool exitOPTION = false;
    public bool exitSHARE = false;
    public bool exitPAD = false;
    public bool exitPS = false;

    public bool anyKeyFlag = false;

    public void GetMyAxis(string key, ref bool enter, ref bool stay, ref bool exit)
    {
        if (Input.GetAxis(key) == 1)
        {
            anyKeyFlag = true;
            if (stay == false)
            {
                enter = true;
            }
            else
            {
                enter = false;
            }

            exit = false;
            stay = true;
        }
        else
        {
            if (stay == true)
            {
                exit = true;
            }
            else
            {
                exit = false;
            }
            enter = false;
            stay = false;
        }
    }
    public void GetMyAxisRev(string key, ref bool enter, ref bool stay, ref bool exit)
    {
        if (Input.GetAxis(key) == -1)
        {
            anyKeyFlag = true;
            if (stay == false)
            {
                enter = true;
            }
            else
            {
                enter = false;
            }

            exit = false;
            stay = true;
        }
        else
        {
            if (stay == true)
            {
                exit = true;
            }
            else
            {
                exit = false;
            }
            enter = false;
            stay = false;
        }
    }

    void SetKey()
    {
        //ぼたん
        GetMyAxis("SANKAKU", ref enterSANKAKU, ref staySANKAKU, ref exitSANKAKU);
        GetMyAxis("MARU", ref enterMARU, ref stayMARU, ref exitMARU);
        GetMyAxis("BATU", ref enterBATU, ref stayBATU, ref exitBATU);
        GetMyAxis("SIKAKU", ref enterSIKAKU, ref staySIKAKU, ref exitSIKAKU);
        GetMyAxis("L3", ref enterL3, ref stayL3, ref exitL3);
        GetMyAxis("R3", ref enterR3, ref stayR3, ref exitR3);
        GetMyAxis("L2", ref enterL2, ref stayL2, ref exitL2);
        GetMyAxis("R2", ref enterR2, ref stayR2, ref exitR2);
        GetMyAxis("L1", ref enterL1, ref stayL1, ref exitL1);
        GetMyAxis("R1", ref enterR1, ref stayR1, ref exitR1);
        GetMyAxis("OPTION", ref enterOPTION, ref stayOPTION, ref exitOPTION);
        GetMyAxis("SHARE", ref enterSHARE, ref staySHARE, ref exitSHARE);
        GetMyAxis("PAD", ref enterPAD, ref stayPAD, ref exitPAD);
        GetMyAxis("PS", ref enterPS, ref stayPS, ref exitPS);
        //十時キー
        GetMyAxis("UorD", ref enterUP, ref stayUP, ref exitUP);
        GetMyAxisRev("UorD", ref enterDOWN, ref stayDOWN, ref exitDOWN);
        GetMyAxis("LorR", ref enterRIGHT, ref stayRIGHT, ref exitRIGHT);
        GetMyAxisRev("LorR", ref enterLEFT, ref stayLEFT, ref exitLEFT);


        //L2R2押し込み具合
        float L2A = Input.GetAxis("L2Axis");
        float R2A = Input.GetAxis("R2Axis");
        if (L2A >= 1f && L2Axis < 1f) { enterL2Axis = true; } else { enterL2Axis = false; }
        if (R2A >= 1f && R2Axis < 1f) { enterR2Axis = true; } else { enterR2Axis = false; }
        L2Axis = L2A;
        R2Axis = R2A;





        //スティック左
        //ローカルベクトル
        LStickVelLocal.z = Input.GetAxis("Vertical");
        LStickVelLocal.x = Input.GetAxis("Horizontal");
        //スティック入力パワー取得
        float prevLSlickLen = LStickLen;
        enterLStick = false;
        enterLStickUp = false;
        enterLStickDown = false;
        enterLStickLeft = false;
        enterLStickRight = false;
        LStickLen = Vector2.Distance(new Vector2(0, 0), new Vector2(LStickVelLocal.x, LStickVelLocal.z));
        if (LStickLen > 1.0f) LStickLen = 1.0f;
        if (LStickLen < 0.0f)
        {
            LStickLen = 0.0f;
        }
        else
        {
            //スティックの入力角度
            float StickCalRad = (Mathf.Atan2(-LStickVelLocal.x, -LStickVelLocal.z) * 180 / Mathf.PI);
            //ワールド角度
            LStickRadWorld = -StickCalRad + Camera.main.transform.eulerAngles.y;
            //ローカル角度
            LStickRadLocal = -StickCalRad;
        }
        if(prevLSlickLen < 0.75f && LStickLen >= 0.75f)
        {
            enterLStick = true;
            if (LStickRadLocal < -90f + 45f && LStickRadLocal > -90f - 45f)
            {
                enterLStickLeft = true;
            }
            if (LStickRadLocal < 90f + 45f && LStickRadLocal > 90f - 45f)
            {
                enterLStickRight = true;
            }
            if (LStickRadLocal <= 45f && LStickRadLocal >= -45f)
            {
                enterLStickUp = true;
            }
            if (LStickRadLocal >= 90 + 45f || LStickRadLocal <= -90 - 45f)
            {
                enterLStickDown = true;
            }
        }
        LStickRadWorld = Mathaf.NormalizeRad(LStickRadWorld);
        LStickRadLocal = Mathaf.NormalizeRad(LStickRadLocal);
        //ワールドベクトル
        LStickVelWorld = Vector3.forward * LStickLen;
        LStickVelWorld = Quaternion.Euler(0, -LStickRadWorld, 0) * LStickVelWorld;
        //Rotate Cal
        float StickRadR = Input.GetAxis("RHorizontal");
        float StickRadRV = Input.GetAxis("RVartical");





        //スティック右
        //ローカルベクトル
        RStickVelLocal.z = Input.GetAxis("RVartical");
        RStickVelLocal.x = Input.GetAxis("RHorizontal");
        //スティック入力パワー取得
        float prevRSlickLen = RStickLen;
        enterRStick = false;
        enterRStickUp = false;
        enterRStickDown = false;
        enterRStickLeft = false;
        enterRStickRight = false;
        RStickLen = Vector2.Distance(new Vector2(0, 0), new Vector2(RStickVelLocal.x, RStickVelLocal.z));
        if (RStickLen > 1.0f) RStickLen = 1.0f;
        if (RStickLen < 0.0f)
        {
            RStickLen = 0.0f;
        }
        else
        {
            //スティックの入力角度
            float StickCalRad = (Mathf.Atan2(-RStickVelLocal.x, -RStickVelLocal.z) * 180 / Mathf.PI);
            //ワールド角度
            RStickRadWorld = -StickCalRad + Camera.main.transform.eulerAngles.y;
            //ローカル角度
            RStickRadLocal = -StickCalRad;
        }
        if (prevRSlickLen < 0.75f && RStickLen >= 0.75f)
        {
            enterRStick = true;
            if (RStickRadLocal < -90f + 45f && RStickRadLocal > -90f - 45f)
            {
                enterRStickLeft = true;
            }
            if (RStickRadLocal < 90f + 45f && RStickRadLocal > 90f - 45f)
            {
                enterRStickRight = true;
            }
            if (RStickRadLocal <= 45f && RStickRadLocal >= -45f)
            {
                enterRStickUp = true;
            }
            if (RStickRadLocal >= 90+45f || RStickRadLocal <= -90-45f)
            {
                enterRStickDown = true;
            }

        }
        RStickRadWorld = Mathaf.NormalizeRad(RStickRadWorld);
        RStickRadLocal = Mathaf.NormalizeRad(RStickRadLocal);
        //ワールドベクトル
        RStickVelWorld = Vector3.forward * RStickLen;
        RStickVelWorld = Quaternion.Euler(0, -RStickRadWorld, 0) * RStickVelWorld;

    }


    // Use this for initialization
    void Start()
    {


    }

    public void AllOff()
    {
        //Key DATA
        enterUP = false;
        enterDOWN = false;
        enterLEFT = false;
        enterRIGHT = false;
        enterSANKAKU = false;
        enterMARU = false;
        enterBATU = false;
        enterSIKAKU = false;
        enterL3 = false;
        enterR3 = false;
        enterL2 = false;
        enterR2 = false;
        enterL1 = false;
        enterR1 = false;
        enterOPTION = false;
        enterSHARE = false;
        enterPAD = false;
        enterPS = false;

        stayUP = false;
        stayDOWN = false;
        stayLEFT = false;
        stayRIGHT = false;
        staySANKAKU = false;
        stayMARU = false;
        stayBATU = false;
        staySIKAKU = false;
        stayL3 = false;
        stayR3 = false;
        stayL2 = false;
        stayR2 = false;
        stayL1 = false;
        stayR1 = false;
        stayOPTION = false;
        staySHARE = false;
        stayPAD = false;
        stayPS = false;

        exitUP = false;
        exitDOWN = false;
        exitLEFT = false;
        exitRIGHT = false;
        exitSANKAKU = false;
        exitMARU = false;
        exitBATU = false;
        exitSIKAKU = false;
        exitL3 = false;
        exitR3 = false;
        exitL2 = false;
        exitR2 = false;
        exitL1 = false;
        exitR1 = false;
        exitOPTION = false;
        exitSHARE = false;
        exitPAD = false;
        exitPS = false;

        anyKeyFlag = false;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            AllOff();
            return;
        }
            anyKeyFlag = false;
            SetKey();
    }
    

}
