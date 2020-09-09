using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyMode
{
    Enter,
    Stay,
    Exit,
    Off
}



public enum Key_Hub
{
    U, R, D, L,
    L1, L2, L3,
    X, A, B, Y,
    R1, R2, R3,
    Option,
    Select,
    Home,
    Share
}



public class Button_Key
{
    public KeyMode Mode;
}

[RequireComponent(typeof(KeyManager_PS4))]
[RequireComponent(typeof(KeyManager_PC))]
[DefaultExecutionOrder(-1000)]
public class KeyManagerHub : MonoBehaviour
{

    static public KeyManagerHub main;

    public enum HardMode
    {
        PS4,
        Switch,
        PC
    }

    KeyManager_PS4 KM_PS;
    KeyManager_PC KM_PC;

    public HardMode hard;

    public bool LRRevFlag = false;//⇔スティックを入れ替え


    public string GetKey_HubImage(Key_Hub keyHubName)
    {
        return Enum.GetName(typeof(Key_Hub), keyHubName);
    }

    bool Enter_PS4(Key_Hub k)
    {
        switch (k)
        {
            case Key_Hub.U: { return KM_PS.enterUP; }
            case Key_Hub.R: { return KM_PS.enterRIGHT; }
            case Key_Hub.D: { return KM_PS.enterDOWN; }
            case Key_Hub.L: { return KM_PS.enterLEFT; }

            case Key_Hub.X: { return KM_PS.enterSANKAKU; }
            case Key_Hub.A: { return KM_PS.enterMARU; }
            case Key_Hub.B: { return KM_PS.enterBATU; }
            case Key_Hub.Y: { return KM_PS.enterSIKAKU; }

            case Key_Hub.L1: { return KM_PS.enterL1; }
            case Key_Hub.L2: { return KM_PS.enterL2; }
            case Key_Hub.L3: { return KM_PS.enterL3; }

            case Key_Hub.R1: { return KM_PS.enterR1; }
            case Key_Hub.R2: { return KM_PS.enterR2; }
            case Key_Hub.R3: { return KM_PS.enterR3; }

            case Key_Hub.Option: { return KM_PS.enterOPTION; }
            case Key_Hub.Select: { return KM_PS.enterPAD; }
            case Key_Hub.Home: { return KM_PS.enterPS; }
            case Key_Hub.Share: { return KM_PS.enterSHARE; }
        }
        return false;
    }

    public void Vibrate(bool lFlag, bool rFlag)
    {
        switch (hard)
        {
            case HardMode.PS4:
                {
//                    KM_PS.Vibrate(lFlag, rFlag);
                }
                break;
            case HardMode.PC:
                {

                }break;
        }

    }

    public Transform D2Trans = null;
    public Vector3 GetStickVectorFromCamera(bool isLeft = true , bool planeFlag = false)
    {
        if (!gameObject.activeSelf) return Vector3.zero;


        //キー入力情報を取得
        Vector3 SV = GetStickVel(isLeft);

        float stickLen = Mathf.Clamp(SV.magnitude, 0f, 1f);

        Vector3 stickVel = Vector3.zero;
        if (stickLen > 0f)
        {
            //操作ベクトル計算
            Vector3 cfVel = Camera.main.transform.forward;
            Vector3 crVel = Camera.main.transform.right;
            cfVel.y = 0f;
            crVel.y = 0f;
            cfVel = cfVel.normalized;
            crVel = crVel.normalized;

            stickVel = ((crVel * SV.x) + (cfVel * SV.y));
            stickVel.y = 0f;
            stickVel = stickVel.normalized;
            stickVel = Quaternion.LookRotation(stickVel) * Vector3.forward * stickLen;

            if(D2Trans)
            {
                stickVel = Vector3.Dot(stickVel, D2Trans.right)*D2Trans.right;
            }
            if(planeFlag)
            {
                stickVel.y = 0f;
                stickVel = stickVel.normalized*stickLen;
            }
        }

        return stickVel;
    }
    public Vector3 GetStickVectorFromTarget(bool isLeft , Transform target)
    {
        if (!gameObject.activeSelf) return Vector3.zero;


        //キー入力情報を取得
        Vector3 SV = GetStickVel(isLeft);

        float stickLen = Mathf.Clamp(SV.magnitude, 0f, 1f);

        Vector3 stickVel = Vector3.zero;
        if (stickLen > 0f)
        {
            //操作ベクトル計算
            Vector3 cfVel = target.forward;
            Vector3 crVel = target.right;
            cfVel.y = 0f;
            crVel.y = 0f;
            cfVel = cfVel.normalized;
            crVel = crVel.normalized;

            stickVel = ((crVel * SV.x) + (cfVel * SV.y));
            stickVel.y = 0f;
            stickVel = stickVel.normalized;
            stickVel = Quaternion.LookRotation(stickVel) * Vector3.forward * stickLen;

            if (D2Trans)
            {
                stickVel = Vector3.Dot(stickVel, D2Trans.right) * D2Trans.right;
            }
        }

        return stickVel;
    }


    public Vector3 GetStickVectorFromNormal(bool isLeft, Vector3 nor, bool revFlag)
    {
        Vector3 stickVel = -GetStickVel(isLeft);
        float stickLen = stickVel.magnitude;
        stickVel.z = stickVel.y;
        stickVel.y = 0f;

        Vector3 f = Camera.main.transform.forward;


        float camNorAngle = Vector3.Angle(Camera.main.transform.forward, nor);
        float camNorDot = Vector3.Dot(nor, Camera.main.transform.forward);

        bool fRevFlag = false;// camNorDot < 0f;//camNorAngle > (360f - 45f);

        bool backFlag = camNorAngle < 45f;

        if (backFlag && !fRevFlag)
        {
            stickVel.z = -stickVel.z;
        }
        if (revFlag)
        {
            f = Camera.main.transform.forward;
            f.y = Mathf.Abs(f.y);

        }
        else
        {
            if (nor.y < 0f && fRevFlag)
            {
                f = -f;
            }
        }
        f.y += Mathf.Abs(camNorDot);
        //        if(f.y < 0f)Vector3.Reflect(, Vector3.up);

        Vector3 r = Quaternion.LookRotation(f, nor) * Vector3.right;

        if (nor.y < 0f)
        {
            if (revFlag)
            {
                stickVel.x = -stickVel.x;
            }
        }


        Vector3 norVel = Vector3.ProjectOnPlane(((f.normalized * stickVel.z) + (r.normalized * stickVel.x)), nor);
        return norVel.normalized * stickLen;
    }
    bool Stay_PS4(Key_Hub k)
    {
        switch (k)
        {
            case Key_Hub.U: { return KM_PS.stayUP; }
            case Key_Hub.R: { return KM_PS.stayRIGHT; }
            case Key_Hub.D: { return KM_PS.stayDOWN; }
            case Key_Hub.L: { return KM_PS.stayLEFT; }

            case Key_Hub.X: { return KM_PS.staySANKAKU; }
            case Key_Hub.A: { return KM_PS.stayMARU; }
            case Key_Hub.B: { return KM_PS.stayBATU; }
            case Key_Hub.Y: { return KM_PS.staySIKAKU; }

            case Key_Hub.L1: { return KM_PS.stayL1; }
            case Key_Hub.L2: { return KM_PS.stayL2; }
            case Key_Hub.L3: { return KM_PS.stayL3; }

            case Key_Hub.R1: { return KM_PS.stayR1; }
            case Key_Hub.R2: { return KM_PS.stayR2; }
            case Key_Hub.R3: { return KM_PS.stayR3; }

            case Key_Hub.Option: { return KM_PS.stayOPTION; }
            case Key_Hub.Select: { return KM_PS.stayPAD; }
            case Key_Hub.Home: { return KM_PS.stayPS; }
            case Key_Hub.Share: { return KM_PS.staySHARE; }
        }
        return false;
    }
    bool Exit_PS4(Key_Hub k)
    {
        switch (k)
        {
            case Key_Hub.U: { return KM_PS.exitUP; }
            case Key_Hub.R: { return KM_PS.exitRIGHT; }
            case Key_Hub.D: { return KM_PS.exitDOWN; }
            case Key_Hub.L: { return KM_PS.exitLEFT; }

            case Key_Hub.X: { return KM_PS.exitSANKAKU; }
            case Key_Hub.A: { return KM_PS.exitMARU; }
            case Key_Hub.B: { return KM_PS.exitBATU; }
            case Key_Hub.Y: { return KM_PS.exitSIKAKU; }

            case Key_Hub.L1: { return KM_PS.exitL1; }
            case Key_Hub.L2: { return KM_PS.exitL2; }
            case Key_Hub.L3: { return KM_PS.exitL3; }

            case Key_Hub.R1: { return KM_PS.exitR1; }
            case Key_Hub.R2: { return KM_PS.exitR2; }
            case Key_Hub.R3: { return KM_PS.exitR3; }

            case Key_Hub.Option: { return KM_PS.exitOPTION; }
            case Key_Hub.Select: { return KM_PS.exitPAD; }
            case Key_Hub.Home: { return KM_PS.exitPS; }
            case Key_Hub.Share: { return KM_PS.exitSHARE; }
        }
        return false;
    }


    bool SwitchConvert(Key_Hub k, ref Joycon.Button button, ref bool isLeft)
    {
        button = Joycon.Button.HOME;
        isLeft = false;
        switch (k)
        {
            case Key_Hub.U: { button = Joycon.Button.DPAD_UP; isLeft = true; } break;
            case Key_Hub.R: { button = Joycon.Button.DPAD_RIGHT; isLeft = true; } break;
            case Key_Hub.D: { button = Joycon.Button.DPAD_DOWN; isLeft = true; } break;
            case Key_Hub.L: { button = Joycon.Button.DPAD_LEFT; isLeft = true; } break;

            case Key_Hub.X: { button = Joycon.Button.DPAD_UP; isLeft = false; } break;
            case Key_Hub.A: { button = Joycon.Button.DPAD_RIGHT; isLeft = false; } break;
            case Key_Hub.B: { button = Joycon.Button.DPAD_DOWN; isLeft = false; } break;
            case Key_Hub.Y: { button = Joycon.Button.DPAD_LEFT; isLeft = false; } break;

            case Key_Hub.L1: { button = Joycon.Button.SHOULDER_1; isLeft = true; } break;
            case Key_Hub.L2: { button = Joycon.Button.SHOULDER_2; isLeft = true; } break;
            case Key_Hub.L3: { button = Joycon.Button.STICK; isLeft = true; } break;

            case Key_Hub.R1: { button = Joycon.Button.SHOULDER_1; isLeft = false; } break;
            case Key_Hub.R2: { button = Joycon.Button.SHOULDER_2; isLeft = false; } break;
            case Key_Hub.R3: { button = Joycon.Button.STICK; isLeft = false; } break;

            case Key_Hub.Option: { button = Joycon.Button.PLUS; isLeft = false; } break;
            case Key_Hub.Select: { button = Joycon.Button.MINUS; isLeft = true; } break;
            case Key_Hub.Home: { button = Joycon.Button.HOME; isLeft = false; } break;
            case Key_Hub.Share: { button = Joycon.Button.CAPTURE; isLeft = true; } break;
            default: { return false; }
        }
        return true;
    }


    bool Enter_PC(Key_Hub k)
    {
        switch (k)
        {
            case Key_Hub.U: { return Input.GetKeyDown(KeyCode.Alpha1); }
            case Key_Hub.R: { return Input.GetKeyDown(KeyCode.Alpha2); }
            case Key_Hub.D: { return Input.GetKeyDown(KeyCode.Alpha3); }
            case Key_Hub.L: { return Input.GetKeyDown(KeyCode.Alpha4); }

            case Key_Hub.X: { return Input.GetMouseButtonDown(1); }
            case Key_Hub.A: { return Input.GetMouseButtonDown(0); }
            case Key_Hub.B: { return Input.GetKeyDown(KeyCode.Space); }

            case Key_Hub.Option: { return Input.GetKeyDown(KeyCode.Escape); }

            case Key_Hub.L3: { return Input.GetMouseButtonDown(2); }
        }
        return false;
    }
    bool Stay_PC(Key_Hub k)
    {
        switch (k)
        {
            case Key_Hub.U: { return Input.GetKeyDown(KeyCode.Alpha1); }
            case Key_Hub.R: { return Input.GetKeyDown(KeyCode.Alpha2); }
            case Key_Hub.D: { return Input.GetKeyDown(KeyCode.Alpha3); }
            case Key_Hub.L: { return Input.GetKeyDown(KeyCode.Alpha4); }

            case Key_Hub.X: { return Input.GetMouseButtonDown(1); }
            case Key_Hub.A: { return Input.GetMouseButtonDown(0); }
            case Key_Hub.B: { return Input.GetKeyDown(KeyCode.Space); }

            case Key_Hub.Option: { return Input.GetKeyDown(KeyCode.Escape); }

            case Key_Hub.L3: { return Input.GetMouseButtonDown(2); }


        }
        return false;
    }
    bool Exit_PC(Key_Hub k)
    {
        switch (k)
        {
            case Key_Hub.U: { return Input.GetKeyUp(KeyCode.Alpha1); }
            case Key_Hub.R: { return Input.GetKeyUp(KeyCode.Alpha2); }
            case Key_Hub.D: { return Input.GetKeyUp(KeyCode.Alpha3); }
            case Key_Hub.L: { return Input.GetKeyUp(KeyCode.Alpha4); }

            case Key_Hub.X: { return Input.GetMouseButtonUp(1); }
            case Key_Hub.A: { return Input.GetMouseButtonUp(0); }
            case Key_Hub.B: { return Input.GetKeyUp(KeyCode.Space); }

            case Key_Hub.Option: { return Input.GetKeyUp(KeyCode.Escape); }

            case Key_Hub.L3: { return Input.GetMouseButtonUp(2); }


        }
        return false;
    }

    bool GetEnter(Key_Hub k, HardMode h)
    {
        if (!gameObject.activeSelf) return false;

        bool flag = false;
        switch (h)
        {
            case HardMode.PS4: { flag = Enter_PS4(k); } break;
            case HardMode.PC: { flag = Enter_PC(k); } break;
        }

        return flag;
    }
    bool GetStay(Key_Hub k, HardMode h)
    {
        if (!gameObject.activeSelf) return false;

        bool flag = false;
        switch (h)
        {
            case HardMode.PS4: { flag = Stay_PS4(k); } break;
            case HardMode.PC: { flag = Stay_PC(k); } break;
        }

        return flag;

    }
    bool GetExit(Key_Hub k, HardMode h)
    {
        if (!gameObject.activeSelf) return false;

        bool flag = false;
        switch (h)
        {
            case HardMode.PS4: { flag = Exit_PS4(k); } break;
            case HardMode.PC: { flag = Exit_PC(k); } break;
        }

        return flag;

    }


    public bool GetAnyKey()
    {
        if (!gameObject.activeSelf) return false;

        for (int i = 0; i < Enum.GetNames(typeof(KeyMode)).Length; i++)
        {
            for (int j = 0; j < Enum.GetNames(typeof(Key_Hub)).Length; j++)
            {
                if (GetKey((KeyMode)i, (Key_Hub)j))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool GetKey(KeyMode km, Key_Hub key)
    {
        if (!gameObject.activeSelf) return false;

        bool flag = false;
        switch (km)
        {
            case KeyMode.Enter:
                {
                    flag = GetEnter(key, hard);
                }
                break;
            case KeyMode.Stay:
                {
                    flag = GetStay(key, hard);
                }
                break;
            case KeyMode.Exit:
                {
                    flag = GetExit(key, hard);
                }
                break;
        }

        return flag;
    }
    KeyMode stickModeL = KeyMode.Off;
    KeyMode stickModeR = KeyMode.Off;
    public KeyMode StickModeL { get { return stickModeL; } }
    public KeyMode StickModeR { get { return stickModeR; } }

    public Vector3 GetStickVel(bool isLeft)
    {
        if (!gameObject.activeSelf) return Vector3.zero;

        if (LRRevFlag) { isLeft = !isLeft; }
        Vector3 vel = new Vector3(0f, 0f, 0f);
        switch (hard)
        {
            case HardMode.PS4:
                {
                    if (isLeft)
                    {
                        vel = KM_PS.LStickVelLocal;
                        float tmp = vel.y;
                        vel.y = -vel.z;
                        vel.z = tmp;
                    }
                    else
                    {
                        vel = KM_PS.RStickVelLocal;
                        float tmp = vel.y;
                        vel.y = vel.z;
                        vel.z = tmp;
                    }
                }
                break;
            case HardMode.PC:
                {
                    vel = KM_PC.GetStickVel(isLeft);
                }
                break;
        }

        //スティックの入力状態
        if (isLeft)
        {
            if (vel.magnitude == 0)
            {
                switch (stickModeL)
                {
                    case KeyMode.Off: { stickModeL = KeyMode.Enter; } break;
                    case KeyMode.Stay: { stickModeL = KeyMode.Stay; } break;
                    case KeyMode.Enter: { stickModeL = KeyMode.Stay; } break;
                    case KeyMode.Exit: { stickModeL = KeyMode.Enter; } break;
                }
            }
            else
            {
                switch (stickModeL)
                {
                    case KeyMode.Off: { stickModeL = KeyMode.Off; } break;
                    case KeyMode.Stay: { stickModeL = KeyMode.Exit; } break;
                    case KeyMode.Enter: { stickModeL = KeyMode.Exit; } break;
                    case KeyMode.Exit: { stickModeL = KeyMode.Off; } break;
                }
            }
        }
        else
        {
            if (vel.magnitude == 0)
            {
                switch (stickModeR)
                {
                    case KeyMode.Off: { stickModeR = KeyMode.Enter; } break;
                    case KeyMode.Stay: { stickModeR = KeyMode.Stay; } break;
                    case KeyMode.Enter: { stickModeR = KeyMode.Stay; } break;
                    case KeyMode.Exit: { stickModeR = KeyMode.Enter; } break;
                }
            }
            else
            {
                switch (stickModeR)
                {
                    case KeyMode.Off: { stickModeR = KeyMode.Off; } break;
                    case KeyMode.Stay: { stickModeR = KeyMode.Exit; } break;
                    case KeyMode.Enter: { stickModeR = KeyMode.Exit; } break;
                    case KeyMode.Exit: { stickModeR = KeyMode.Off; } break;
                }
            }
        }
        return vel;
    }

    public float GetStickRadFromCameraTarget(bool isLeft,Transform target = null)
    {
        if (!gameObject.activeSelf) return 0f;

        Vector3 vel = GetStickVel(isLeft);
        if (vel.magnitude <= 0f) return 0f;

        //スティックの入力角度
        float StickCalRad = (Mathf.Atan2(-vel.x, vel.y) * 180 / Mathf.PI);

        float r = 0f;
        if(target)
        {
            r = (-StickCalRad + Camera.main.transform.eulerAngles.y)-target.eulerAngles.y;
        }
        else
        {
            r = -StickCalRad;
        }
        r = Mathaf.NormalizeRad(r);
        return r;
    }

    public Vector3 GetGyroVel(bool isLeft)
    {
        if (!gameObject.activeSelf) return Vector3.zero;

        Vector3 vel = new Vector3(0f, 0f, 0f);
        switch (hard)
        {
            case HardMode.PS4:
                {
                    if (isLeft) {/* vel = KM_PS.LStickVelLocal;*/ }
                    else
                    {/*vel = KM_PS.RStickVelLocal;*/}
                }
                break;
            case HardMode.PC:
                {
                }
                break;
        }

        return vel;
    }

    // Use this for initialization
    void Start()
    {
        if (!main) main = this;

        KM_PS = GetComponent<KeyManager_PS4>();
        KM_PC = GetComponent<KeyManager_PC>();


        string[] deviceNames = Input.GetJoystickNames();

        foreach(string str in deviceNames)
        {

            if(str.Contains("HORIPAD 4 ") || str.Contains("Wireless Controller"))
            {
                hard = HardMode.PS4;
            }
            Debug.Log(str);
        }


    }

    public void Off()
    {
        gameObject.SetActive(false);
        KM_PS.isActive = false;
        KM_PS.AllOff();
    }

    public void On()
    {
        gameObject.SetActive(true);
        KM_PS.isActive = true;
        KM_PS.AllOff();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
