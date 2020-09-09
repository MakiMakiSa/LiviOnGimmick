using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

public struct Keys
{
    public Vector2 JoyStick;
    public Vector3 JoyGyro;
    public Vector3 JoyAccel;
    public Quaternion JoyRad;
}

[RequireComponent(typeof(JoyconManager))]
public class KeyManager_Switch : MonoBehaviour
{
    public Keys LKey;
    public Keys RKey;

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];//現在押されているボタンの配列

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;



    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);


        foreach (Joycon.Button button in m_buttons)
        {
            Debug.Log(button);
        }
    }

    public bool GetKeyStay(Joycon.Button button, bool isLeft)
    {
        if (isLeft)
        {
            return m_joyconL.GetButton(button);
        }
        else
        {
            return m_joyconR.GetButton(button);
        }
    }
    public bool GetKeyDown(Joycon.Button button, bool isLeft)
    {
        if (isLeft)
        {
            return m_joyconL.GetButtonDown(button);
        }
        else
        {
            return m_joyconR.GetButtonDown(button);
        }
    }
    public bool GetKeyUp(Joycon.Button button, bool isLeft)
    {
        if (isLeft)
        {
            return m_joyconL.GetButtonUp(button);
        }
        else
        {
            return m_joyconR.GetButtonUp(button);
        }
    }
    void SetKeys(ref Keys key, Joycon joycon)
    {
        key.JoyStick.x = joycon.GetStick()[0];
        key.JoyStick.y = joycon.GetStick()[1];
        key.JoyGyro = joycon.GetGyro();
        key.JoyAccel = joycon.GetAccel();
        key.JoyRad = joycon.GetVector();


        Vector3 gy = key.JoyGyro;
        gy.x = -key.JoyGyro.y;
        gy.z = -key.JoyGyro.x;
        gy.y = key.JoyGyro.z;
        key.JoyGyro = gy;

        Vector3 ac = key.JoyGyro;
        ac.x = -key.JoyAccel.y;
        ac.z = -key.JoyAccel.x;
        ac.y = 0f;//key.JoyAccel.z;
        key.JoyAccel = ac;

        //なぜか↑のほうむくから補正
        key.JoyGyro.x += 0.01f;

    }
    private void FixedUpdate()
    {
        if (m_joycons == null || m_joycons.Count <= 0) return;

        foreach (var joycon in m_joycons)
        {
            if (joycon.isLeft)//左コンか？右コンか？フラグ
            {
                SetKeys(ref LKey, joycon);
            }
            else
            {
                SetKeys(ref RKey, joycon);
            }
        }
    }

    public void Vibrate(bool lFlag, bool rFlag)
    {
        if (lFlag) m_joyconL.SetRumble(160, 320, 0.6f, 200);
        if (rFlag) m_joyconR.SetRumble(160, 320, 0.6f, 200);
    }

    private void OnGUI()
    {
        //ジョイ婚なし
        if (m_joycons == null || m_joycons.Count <= 0)
        {
            return;
        }
        //左なし
        if (!m_joycons.Any(c => c.isLeft))
        {
            return;
        }
        //右なし
        if (!m_joycons.Any(c => !c.isLeft))
        {
            return;
        }
    }
}