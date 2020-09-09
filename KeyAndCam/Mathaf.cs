using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class Mathaf : MonoBehaviour
{
    /// <summary>
    /// 子がいないオブジェクトを削除する
    /// 
    /// </summary>
    /// <param name="par"></param>
    static public void DestroyInNotChild(Transform par)
    {
        foreach (Transform t in par)
        {
            if (t.childCount <= 0)
            {
                Destroy(t.gameObject);
            }
        }

    }
    /// <summary>
    ///名前の後に_番号をつける(BlenderのCell用）
    /// </summary>
    /// <param name="par"></param>
    static public void ZeroNumName_Blender(Transform par)
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform t in par)
        {
            list.Add(t);
        }
        list = list.OrderBy(b => b.name.Length).ToList();
        foreach (Transform t in list)
        {
            string[] sprName = t.name.Split('_');
            int num = sprName.Length - 1;
            if (num >= 1)
            {
                if (!sprName[num].Contains("0"))
                {
                    t.name += "_cell";
                }
            }
        }
    }
    /// <summary>
    /// 名前をフォルダーとして解釈させ、階層構造を作る（Blenderで破片を作った時
    /// 名前が階層構造になってるけど、自動で親子関係は作ってくれないから
    /// やむをえず、こういう形で自動実装
    /// </summary>
    /// <param name="par"></param>
    static public void NameIsFolder(Transform par)
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform t in par)
        {
            list.Add(t);
        }
        list = list.OrderBy(b => b.name.Length).ToList();
        foreach (Transform t in list)
        {
            NameIsFolder(ref list, t);
        }
    }

    /// <summary>
    /// FromからToへ回転するために必要なZAngleRotate値
    /// </summary>
    /// <param name="from">回転したいトランスフォーム</param>
    /// <param name="to">目標としたい方向のベクトル</param>
    /// <returns></returns>
    static public float GetLookAtRad_XZ(Transform from, Vector3 to)
    {

        Vector3 vel = from.InverseTransformVector(to);

        if (vel.magnitude <= 0f) return 0f;

        return NormalizeRad((Mathf.Atan2(vel.x, vel.z) * 180 / Mathf.PI));
    }


    /// <summary>
    /// FromからToへ回転するために必要なZAngleRotate値
    /// </summary>
    /// <param name="from">回転したいトランスフォーム</param>
    /// <param name="to">目標としたいトランスフォーム</param>
    /// <returns></returns>
    static public float GetLookAtRad_XZ(Transform from, Transform to)
    {
        if (!to) return 0f;

        Vector3 vel = from.InverseTransformPoint(to.position);

        if (vel.magnitude <= 0f) return 0f;

        return NormalizeRad((Mathf.Atan2(vel.x, vel.z) * 180 / Mathf.PI));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="par"></param>
    /// <param name="papa"></param>
    static public void NameIsFolder(ref List<Transform> par, Transform papa)
    {
        foreach (Transform t in par)
        {
            if (t.name.Contains(papa.name) && t != papa)
            {
                t.parent = papa;
            }
        }
    }

    /// <summary>
    /// UnityのNullはC＃のNullとは違うため、場合によってはif nullを通ってしまう、多分!=とかオーバーライドされてる
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public bool isNull(Object obj)
    {
        return (obj != null);
    }
    /// <summary>
    /// t　から 最後のparentまで、T(何かしらのコンポーネント）を探し、あった場合はそのコンポーネントoutし、true
    /// 無ければfalse
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="res"></param>
    static public bool SearchComponentToParent<T>(Transform t, out T res)
    {
        res = default;
        do
        {
            T c = t.GetComponent<T>();
            Debug.Log(c);
            if (isNull(c as Object))
            {
                res = c;
                return true;
            }
            t = t.parent;
        } while (t != null);

        Debug.Log("SCTP Err");
        return false;
    }

    /// <summary>
    /// parから下のすべてのトランスフォームをリストにまとめて返す
    /// </summary>
    /// <param name="SetTrans"></param>
    /// <param name="par"></param>
    static public void SearchBone(ref List<Transform> SetTrans, Transform par)
    {
        foreach (Transform c in par)
        {
            SetTrans.Add(c);
            SearchBone(ref SetTrans, c);
        }
    }//parを起点として、それ以降のすべてのトランスフォームをリストに追加する



    //単位ベクトル生成
    static public Vector3 CreateUINTVector(Vector3 v)
    {
        float len = Mathf.Pow((v.x * v.x) + (v.y * v.y) + (v.z * v.z), 0.5f); //ベクトル長さ

        Vector3 ret;
        ret.x = v.x / len;
        ret.y = v.y / len;
        ret.z = v.z / len;

        return ret;
    }

    /// <summary>
    ///点Pと直線ABから線上最近点を求める
    /// </summary>
    /// <param name="fromPos"></param>
    /// <param name="toPosA"></param>
    /// <param name="toPosB"></param>
    /// <param name="clampFlag">直線を制限するか、無限直線にするか</param>
    /// <returns></returns>
    static public Vector3 NearPosOnLine(Vector3 fromPos, Vector3 toPosA, Vector3 toPosB, bool clampFlag = true)
    {
        Vector3 AB = toPosB - toPosA;
        Vector3 AP = fromPos - toPosA;

        //ABの単位ベクトルを計算
        Vector3 nAB = CreateUINTVector(AB);

        //Aから線上最近点までの距離（ABベクトルの後ろにあるときはマイナス値）
        float distance = Vector3.Dot(nAB, AP);
        if (clampFlag)
        {
            distance = Mathf.Clamp(distance, 0, Vector3.Distance(toPosA, toPosB));
        }


        //線上最近点
        return toPosA + (nAB * distance);
    }
    static public Vector3 NearPosOnLine(Vector3 fromPos, Vector3 toPosA, Vector3 toPosB, out float distance, bool clampFlag = true)
    {
        Vector3 AB = toPosB - toPosA;
        Vector3 AP = fromPos - toPosA;

        //ABの単位ベクトルを計算
        Vector3 nAB = CreateUINTVector(AB);

        //Aから線上最近点までの距離（ABベクトルの後ろにあるときはマイナス値）
        distance = Vector3.Dot(nAB, AP);
        if (clampFlag)
        {
            distance = Mathf.Clamp(distance, 0, Vector3.Distance(toPosA, toPosB));
        }


        //線上最近点
        return toPosA + (nAB * distance);
    }

    //IKに使うためのセット、コア部分から遠ざかる順番に設定しないとダメなの
    //先端用の計算用オブジェはかざりだから使わないでね、作れるデータは作ってあるけど角度の基準がないから、そこが設定できなかった
    public static GameObject[] IKSet(GameObject coreObj, params GameObject[] Obj)
    {
        GameObject[] cObj = new GameObject[Obj.Length];
        for (int i = 0; i < Obj.Length; i++)
        {
            //オブジェ生成
            cObj[i] = new GameObject("cObj" + i);
            //オブジェお親設定
            if (i == 0)
            {
                cObj[i].transform.parent = coreObj.transform;
            }
            else
            {
                cObj[i].transform.parent = Obj[i - 1].transform;
            }
            //オブジェの座標設定
            cObj[i].transform.position = Obj[i].transform.position;

            //オブジェの角度設定
            if (i < Obj.Length - 1)
            {
                cObj[i].transform.LookAt(Obj[i + 1].transform.position, coreObj.transform.forward);
            }
            else
            {
                //				Vector3 toPos = (Obj [i].transform.position-Obj [i - 1].transform.position)+Obj [i].transform.position;
                //				cObj [i].transform.LookAt (toPos, coreObj.transform.forward);
                Vector3 toPos = -coreObj.transform.forward + Obj[i].transform.position;
                cObj[i].transform.LookAt(toPos, coreObj.transform.up);
            }

            //表示用ぶじぇの親を計算用オブジェに設定
            Obj[i].transform.parent = cObj[i].transform;


            //			coreObj.AddComponent<NetMotionLink> ().netId .Objs = cObj [i];

        }

        return cObj;
    }

    //
    // IK　a b c オブジェを tObjにIKする　、bオブジェの回転方向は、とりあえずaオブジェを中心とした所から山なりだけれども、他の設定もつくらないとね
    //



    public enum RadType
    {
        Not,
        Abs,
        Abs_X_Rev,
        Abs_Y_Rev,
        X_Onry,
        X_OnryRev,
        Y_Onry,
        Y_OnryRev
    }


    public static GameObject cCube = null;
    public static void IK(GameObject aObj, GameObject bObj, GameObject cObj, GameObject tObj, float RotPsc, RadType RT = RadType.Not, GameObject pObj = null)
    {
        if (cCube == null) { cCube = new GameObject("calCube"); }

        aObj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        bObj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        float atLen = Vector3.Distance(aObj.transform.position, tObj.transform.position);
        float abLen = Vector3.Distance(aObj.transform.position, bObj.transform.position);
        float bcLen = Vector3.Distance(bObj.transform.position, cObj.transform.position);


        float calic = (((abLen * abLen) + (bcLen * bcLen)) - (atLen * atLen)) / (2 * abLen * bcLen);



        float lenRad = Mathf.Acos(calic);

        if (lenRad > 0f)
        {
            float bRadian = (Mathf.PI - lenRad) * Mathf.Rad2Deg;

            Vector3 atOfs = aObj.transform.position - tObj.transform.position;

            atOfs = Quaternion.Inverse(aObj.transform.parent.rotation) * atOfs;

            float atLenx = atOfs.x;
            float atLenz = atOfs.z;
            float mmLen = Mathf.Abs(atLenx) + Mathf.Abs(atLenz);

            switch (RT)
            {
                case RadType.Not:
                    {
                        atLenx /= mmLen;
                        atLenz /= mmLen;
                    }
                    break;
                case RadType.Abs:
                    {
                        atLenx /= mmLen;
                        atLenz /= mmLen;
                        atLenx = Mathf.Abs(atLenx);
                        atLenz = Mathf.Abs(atLenz);
                    }
                    break;
                case RadType.Abs_X_Rev:
                    {
                        atLenx /= mmLen;
                        atLenz /= mmLen;
                        atLenx = Mathf.Abs(atLenx);
                        atLenz = -Mathf.Abs(atLenz);
                    }
                    break;
                case RadType.Abs_Y_Rev:
                    {
                        atLenx /= mmLen;
                        atLenz /= mmLen;
                        atLenx = -Mathf.Abs(atLenx);
                        atLenz = Mathf.Abs(atLenz);
                    }
                    break;
                case RadType.X_Onry:
                    {
                        atLenz = 1f;
                        atLenx = 0f;
                    }
                    break;
                case RadType.X_OnryRev:
                    {
                        atLenz = -1f;
                        atLenx = 0f;
                    }
                    break;
                case RadType.Y_Onry:
                    {
                        atLenz = 0f;
                        atLenx = 1f;
                    }
                    break;
                case RadType.Y_OnryRev:
                    {
                        atLenz = 0f;
                        atLenx = -1f;
                    }
                    break;
            }

            bObj.transform.localEulerAngles = new Vector3(bRadian * atLenz * RotPsc, bRadian * atLenx * RotPsc, 0f);

        }

        Transform parObj = aObj.transform.parent;

        Transform calpObj = parObj;
        if (pObj != null)
        {
            calpObj = pObj.transform;
        }

        cCube.transform.position = aObj.transform.position;
        cCube.transform.eulerAngles = aObj.transform.eulerAngles;
        cCube.transform.LookAt(cObj.transform.position, calpObj.up);

        aObj.transform.parent = cCube.transform;
        cCube.transform.LookAt(tObj.transform.position, calpObj.forward);

        aObj.transform.parent = parObj;

    }

    public static float deltaClampTime(float x)
    {
        return Mathf.Clamp(Time.deltaTime * x, 0f, 1f);
    }

    /// <summary>
    /// クオータニオンの回転値を-180～180に丸める
    /// </summary>
    /// <param name="rad"></param>
    /// <param name="ofs"></param>
    /// <returns></returns>
    public static Quaternion NormalizeQuater(Quaternion rad, Quaternion ofs)
    {
        Vector3 r = rad.eulerAngles;
        Vector3 o = ofs.eulerAngles;

        r -= o;
        NormalizeRads(r);
        r += o;
        return Quaternion.Euler(r);
    }

    public static float NormalizeRad(float rad, float num = 180f)
    {
        if (rad > num) { rad -= 360; }
        if (rad < -num) { rad += 360; }

        if (rad > num) { rad = NormalizeRad(rad); }
        if (rad < -num) { rad = NormalizeRad(rad); }

        return rad;
    }
    public static Vector3 NormalizeRads(Vector3 Rads, float num = 180f)
    {
        Rads.x = NormalizeRad(Rads.x, num);
        Rads.y = NormalizeRad(Rads.y, num);
        Rads.z = NormalizeRad(Rads.z, num);
        return Rads;
    }
    public static Vector3 NormalizeRads(Vector3 Rads, Vector3 nums)
    {
        Rads.x = NormalizeRad(Rads.x, nums.x);
        Rads.y = NormalizeRad(Rads.y, nums.y);
        Rads.z = NormalizeRad(Rads.z, nums.z);
        return Rads;
    }


    public static float toRad(float angle)
    {
        return angle * 180 / Mathf.PI;
    }

    public static bool Fit(ref float fromNum, float toNum, float speed)
    {
        bool fitFlag = true;
        if (fromNum > toNum) { fitFlag = false; fromNum -= speed; if (fromNum < toNum) { fromNum = toNum; } }
        if (fromNum < toNum) { fitFlag = false; fromNum += speed; if (fromNum > toNum) { fromNum = toNum; } }

        return fitFlag;
    }

    public static bool Fit(ref Vector3 pos, Vector3 toPos, float speed)
    {
        bool fitFlag = true;
        if (pos.x > toPos.x) { fitFlag = false; pos.x -= speed; if (pos.x < toPos.x) { pos.x = toPos.x; } }
        if (pos.x < toPos.x) { fitFlag = false; pos.x += speed; if (pos.x > toPos.x) { pos.x = toPos.x; } }
        if (pos.y > toPos.y) { fitFlag = false; pos.y -= speed; if (pos.y < toPos.y) { pos.y = toPos.y; } }
        if (pos.y < toPos.y) { fitFlag = false; pos.y += speed; if (pos.y > toPos.y) { pos.y = toPos.y; } }
        if (pos.z > toPos.z) { fitFlag = false; pos.z -= speed; if (pos.z < toPos.z) { pos.z = toPos.z; } }
        if (pos.z < toPos.z) { fitFlag = false; pos.z += speed; if (pos.z > toPos.z) { pos.z = toPos.z; } }

        return fitFlag;
    }
    public static bool Fit(ref Vector3 pos, Vector3 toPos, float speed, float min, float max)
    {
        float xspeed = Mathf.SmoothStep(min, max, pos.x) * speed;
        float yspeed = Mathf.SmoothStep(min, max, pos.y) * speed;
        float zspeed = Mathf.SmoothStep(min, max, pos.z) * speed;
        bool fitFlag = true;
        if (pos.x > toPos.x) { fitFlag = false; pos.x -= xspeed; if (pos.x < toPos.x) { pos.x = toPos.x; } }
        if (pos.x < toPos.x) { fitFlag = false; pos.x += xspeed; if (pos.x > toPos.x) { pos.x = toPos.x; } }
        if (pos.y > toPos.y) { fitFlag = false; pos.y -= yspeed; if (pos.y < toPos.y) { pos.y = toPos.y; } }
        if (pos.y < toPos.y) { fitFlag = false; pos.y += yspeed; if (pos.y > toPos.y) { pos.y = toPos.y; } }
        if (pos.z > toPos.z) { fitFlag = false; pos.z -= zspeed; if (pos.z < toPos.z) { pos.z = toPos.z; } }
        if (pos.z < toPos.z) { fitFlag = false; pos.z += zspeed; if (pos.z > toPos.z) { pos.z = toPos.z; } }

        return fitFlag;
    }
    public static bool FitZero(ref Vector3 pos, float speed)
    {
        bool fitFlag = true;
        if (pos.x > 0f) { fitFlag = false; pos.x -= speed; if (pos.x < 0f) { pos.x = 0f; } }
        if (pos.x < 0f) { fitFlag = false; pos.x += speed; if (pos.x > 0f) { pos.x = 0f; } }
        if (pos.y > 0f) { fitFlag = false; pos.y -= speed; if (pos.y < 0f) { pos.y = 0f; } }
        if (pos.y < 0f) { fitFlag = false; pos.y += speed; if (pos.y > 0f) { pos.y = 0f; } }
        if (pos.z > 0f) { fitFlag = false; pos.z -= speed; if (pos.z < 0f) { pos.z = 0f; } }
        if (pos.z < 0f) { fitFlag = false; pos.z += speed; if (pos.z > 0f) { pos.z = 0f; } }

        return fitFlag;
    }

    /// <summary>
    /// すべての子の中から検索
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChildAll(Transform parent, string name)
    {
        foreach (Transform t in parent)
        {
            Transform c = parent.Find(name);
            if (c)
            {
                return c;
            }
            else
            {
                c = FindChildAll(t, name);
                if (c)
                {
                    return c;
                }
            }
        }
        return null;

    }

    public static void DestroyAllChil(GameObject g)
    {
        foreach (Transform n in g.transform)
        {
            Destroy(n.gameObject);
        }
    }
    /// <summary>
    ///画面の中から特定のタグの、いちばん画面中央に近いものを探し出す
    /// </summary>
    /// <param name="tag">タグ</param>
    /// <returns></returns>
    static public Transform SearchCenterObjFromTag(string tag)
    {
        var enemys = GameObject.FindGameObjectsWithTag(tag);

        Transform t = null;
        float minMagnitude = float.MaxValue;

        foreach (GameObject e in enemys)
        {
            var vp = Camera.main.WorldToViewportPoint(e.transform.position);
            if (vp.z > 0f && vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f)
            {
                vp.x -= 0.5f;
                vp.y -= 0.5f;
                vp.z *= 0.01f;
                if (minMagnitude > vp.magnitude)
                {
                    minMagnitude = vp.magnitude;
                    t = e.transform;
                }
            }
        }
        return t;
    }

    //テクスチャを単に書き換えると、それを使っている他のマテリアルにまで影響を与えてしまう
    //故一度テクスチャを複製して、それを使用テクスチャとすれば、それをいくら書き換えても他に影響はない

    /// <summary>
    /// targetのレンダラーに使われているテクスチャを複製する（今後直接書き込むとかする時のため）
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hm"></param>
    /// <returns></returns>
	public static Texture2D textureHookInit(Material material, string textureName)
    {

        //元てくすちゃを取得
        Texture2D texture = (Texture2D)material.GetTexture(textureName);
        if (!texture)
        {
            Debug.LogError("テクスチャ取得しようとしたけど" + textureName + "テクスチャついてないよ！");
            return null;
        }



        //ピクセルの先頭を取得（配列の頭）
        Color[] pTex = texture.GetPixels();

        //空の編集用テクスチャを生成
        Color[] editTexture = (Color[])pTex.Clone();
        //編集用テクスチャに元テクスチャをコピー
        //for (int i = 0; i < pTex.Length; i++)
        //{
        //    editTexture.SetValue(pTex[i], i);
        //}

        //テクスチャマネージャを生成
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        //マネーじゃのステータスを設定
        newTexture.filterMode = texture.filterMode;
        newTexture.SetPixels(editTexture);      //用意いたテクスチャを設定！
        newTexture.Apply();//てくすちゃ完成！


        //レンダラーに作ったテクスチャを設定
        material.SetTexture(textureName, newTexture);


        return newTexture;
    }


    public static Texture2D textureCreateInit(Material material, string textureName, int sx, int sy , TextureWrapMode wrapMode)
    {
        //空の編集用テクスチャを生成
        int len = (int)(sx * sy);
        Color[] editTexture = new Color[len];

        //テクスチャマネージャを生成
        Texture2D newTexture = new Texture2D(sx, sy, TextureFormat.RGBA32, false);
        //マネーじゃのステータスを設定
        newTexture.filterMode = FilterMode.Trilinear;
        newTexture.wrapMode = wrapMode;
        newTexture.SetPixels(editTexture);      //用意いたテクスチャを設定！
        newTexture.Apply();//てくすちゃ完成！


        //レンダラーに作ったテクスチャを設定
        material.SetTexture(textureName, newTexture);


        return newTexture;
    }



    /// <summary>
    /// ヒット情報を元に対象のテクスチャに書き込む、これを使う前には、HookInitで書き込み用テクスチャを初期化しておいたほうがいいぽ
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hm"></param>
    /// <param name="stunpTexture"></param>
    /// <param name="u"></param>
    /// <param name="v"></param>
    static public void DrawTexture_Ray(Texture2D palletTexture, RaycastHit hit, Texture2D stampTexture)
    {

        //０～１のＵＶ座標からピクセル座標ＸＹに変換
        Vector2 uv = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
        DrawTexture(uv, palletTexture, stampTexture);
    }

    /// <summary>
    /// uvの値はこれに渡す前に絶対、０～１の値にまるめて
    /// </summary>
    /// <param name="uv"></param>
    /// <param name="palletTexture"></param>
    /// <param name="stampTexture"></param>
    static public void DrawTexture(Vector2 uv, Texture2D palletTexture, Texture2D stampTexture)
    {

        uv = new Vector2(uv.x * palletTexture.width, uv.y * palletTexture.height);

        float startX = (uv.x - (stampTexture.width / 2f));
        float startY = (uv.y - (stampTexture.height / 2f));

        float stampL = 0f;
        float stampU = 0f;
        float stampR = stampTexture.width;
        float stampD = stampTexture.height;

        //ＵＶの切れ目をまたぐ場合、その先の座標で再帰する

        if (startX < 0)
        {
            stampL += -startX;
            stampR -= -startX;
            startX = 0f;
        }
        if (startY < 0)
        {
            stampU += -startY;
            stampD -= -startY;
            startY = 0f;
        }

        if (startX + stampR > palletTexture.width)
        {
            stampR -= ((startX + stampR) - (palletTexture.width));
        }
        if (startY + stampD > palletTexture.height)
        {
            stampD -= ((startY + stampD) - (palletTexture.height));
        }


        if (stampL < 0f) stampL = 0f;
        if (stampR < 0f) stampR = 0f;
        if (stampU < 0f) stampU = 0f;
        if (stampD < 0f) stampD = 0f;


        Color[] stampColors_A = palletTexture.GetPixels((int)startX, (int)startY, (int)stampR, (int)stampD);
        Color[] stampColors_B = stampTexture.GetPixels((int)stampL, (int)stampU, (int)stampR, (int)stampD);

        for (int i = 0; i < stampColors_A.Length; i++)
        {
            //            stampColors_A[i].a = Mathf.Max(stampColors_A[i].a, stampColors_B[i].a);
            stampColors_A[i] += stampColors_B[i];
        }
        
        palletTexture.SetPixels((int)startX, (int)startY, (int)stampR, (int)stampD, stampColors_A);
        palletTexture.Apply();
    }

    /// <summary>
    /// aとbの値の変化の間にVautLineが含まれているか否か（一定のラインを超えた瞬間）
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="VautLine"></param>
    /// <returns></returns>
	public static bool numVaut(float a, float b, float VautLine)
    {
        if ((a > VautLine && b <= VautLine) || (a <= VautLine && b > VautLine))
        {
            return true;
        }

        return false;

    }

    /// <summary>
    /// Yを軸とした、前回と今回の回転差分を取得する
    /// </summary>
    /// <param name="prevForward"></param>
    /// <param name="currentForward"></param>
    /// <returns></returns>
    float FromToRadianOffsetAxis(Vector3 prevForward, Vector3 currentForward)
    {
        //前回フレームと今回フレームのY軸回転の差異のぶんだけ、アバターを回転させる
        Vector3 prevF = prevForward;
        Vector3 currentF = currentForward;
        float prevMag = prevF.magnitude;
        float currentMag = currentF.magnitude;
        float prevRad = Mathaf.NormalizeRad((Mathf.Atan2(prevF.x, prevF.z) * 180 / Mathf.PI));
        float currentRad = Mathaf.NormalizeRad((Mathf.Atan2(currentF.x, currentF.z) * 180 / Mathf.PI));
        float rad = Mathaf.NormalizeRad(currentRad - prevRad);

        return rad;

    }

    public static Vector3 LerpEuler(Transform a, Transform b, float len)
    {

        Vector3 ae = a.eulerAngles;

        float ang = Quaternion.Angle(a.rotation, b.rotation);
        Quaternion abQ = Quaternion.RotateTowards(a.rotation, b.rotation, ang * len);

        ae += NormalizeRads(abQ.eulerAngles - ae);



        return ae;
    }
}
