using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FootSEManager : MonoBehaviour
{
    static FootSEManager main;

    [System.SerializableAttribute]
    public class Clips
    {
        [SerializeField]
        public List<AudioClip> clips;
        [SerializeField]
        public VisualEffect effect;
    }

    [SerializeField]
    List<Clips> footSes;

    [SerializeField]
    float waitTime = 0.1f;

    static float prevTime;


    private void Start()
    {
        if (!main) main = this;
    }

    static public void PlayClip(int track , Vector3 pos , Quaternion rotation , float volume)
    {
        float nowTime = Time.time;
        if (nowTime - prevTime < main.waitTime) return;
        prevTime = nowTime;

        track  %= main.footSes.Count;

        int r = Random.Range(0, main.footSes[track].clips.Count);

        AudioSource.PlayClipAtPoint(main.footSes[track].clips[r], pos , volume);

        if(main.footSes[track].effect)
        {
            VisualEffect ve = Instantiate(main.footSes[track].effect);
            ve.transform.position = pos;
            ve.transform.rotation = rotation;

            Destroy(ve.gameObject, 0.5f);
        }
    }


}
