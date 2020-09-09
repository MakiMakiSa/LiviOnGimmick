using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OITimer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI tmp;


    String text = "liviongimmick";


    // Start is called before the first frame update
    void Start()
    {
        text = tmp.text;
    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = OI(text);
    }


    string OI(string t)
    {
        DateTime presentTime = DateTime.Now;
        string Year = Convert.ToString(presentTime.Year, 2);//年
        string Month = Convert.ToString(presentTime.Month, 2);//月
        string Day = Convert.ToString(presentTime.Day, 2);//日
        string Hour = Convert.ToString(presentTime.Hour, 2);//時
        string Minute = Convert.ToString(presentTime.Minute, 2);//分
        string Second = Convert.ToString(presentTime.Second, 2);//秒


        List<int> bits_Hour = bitSet(Hour, 5);
        List<int> bits_Minute = bitSet(Minute, 6);
        List<int> bits_Second = bitSet(Second, 6);

        List<int> flags = new List<int>();
        flags.AddRange(bits_Minute);
        flags.Add(-1);
        flags.AddRange(bits_Second);
        flags.Add(-1);
        flags.AddRange(bits_Hour);


        int i = 0;

        string uText = t.ToUpper();
        string lText = t.ToLower();

        string res = "";

        foreach (int flag in flags)
        {
            if(flag == -1)
            {
                res += " ";
            }
            else
            {
                res += (flag != 0) ? uText[i] : lText[i];
                i++;
            }

            if (i >= t.Length) break;
        }


        return res;
    }



    List<int> bitSet(string t , int bit)
    {
        List<int> res = new List<int>();

        foreach(char c in t)
        {
            res.Add((c != '0') ? 1 : 0);
        }

        int ad = bit - res.Count;
        for(int i = 0; i < ad; i++)
        {
            res.Insert(0, 0);
        }

        return res;
    }

}
