using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    static public EquipmentManager main;
    [SerializeField]
    List<GameObject> Equipments = new List<GameObject>();

    int num;
    private void Start()
    {
        if (!main) main = this;

        num = PlayerPrefs.GetInt("EquipmentNum", 0);
        Select(num);
    }

    void Save()
    {
        PlayerPrefs.SetInt("EquipmentNum", num);
        PlayerPrefs.Save();
    }

    void Select(int i)
    {
        foreach (GameObject obj in Equipments)
        {
            obj.SetActive(false);
        }

        Equipments[i % Equipments.Count].SetActive(true);
    }

    private void Update()
    {
        if(KeyManagerHub.main.GetKey(KeyMode.Enter , Key_Hub.Y))
        {
            num++;
            num %= Equipments.Count;
            Select(num);
        }
    }
}
