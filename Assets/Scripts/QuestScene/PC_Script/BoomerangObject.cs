using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Policy;
using UnityEngine;
using Debug = UnityEngine.Debug;

/*
3点でのベジェ曲線の式を利用して
ブーメランっぽい軌道でオブジェクトを投げて攻撃

[ 自身から敵に向かって投げる際 ]
点P1 = prePosi（自身の座標）
点P2 = P2 （敵の座標を中心に、敵との中間座標を何度か回転させた座標
点P3 = lockPosi （敵座標）
*/
public class BoomerangObject : MonoBehaviour
{
    [HideInInspector] public MieScript mieScript;

    Vector3 preLocalPosition; //武器オブジェクトのlocalPostioを保持しとく
    Transform parentTransform; //親オブジェクトを保持しとく
    Vector3 prePosi;
    Vector3 lockPosi;
    Vector3 Position2;

    float distance; //lockPosiとprePosiとの距離を保持
    float ratio; //ベジェ曲線のパラメータ（０～１f）
    float totalTime; //ratioの計算用
    int P2angle = 30; //点P2の座標計算で使用する角度

    bool isThrowing = false;
    bool returnFlag;

    public void Start()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        preLocalPosition = this.transform.localPosition;
        parentTransform = this.transform.parent;
    }

    public void SetBoomerang(Vector3 posi)
    {
        this.transform.parent = null;
        lockPosi = posi;
        prePosi = transform.position;
        distance = (lockPosi - prePosi).magnitude;
        ratio = 0f;
        totalTime = 0;
        isThrowing = true;
        returnFlag = false;
        mieScript.isThrowing = true;

        Position2 = GetP2(prePosi, lockPosi, -30);

        gameObject.GetComponent<BoxCollider>().enabled = true; ;
    }

    void FixedUpdate()
    {
        if (isThrowing)
        {
            ratio = totalTime*50 / distance;
            //ブーメランの位置更新
            this.transform.position = BoomerangPosi(new Vector2(prePosi.x, prePosi.z),
                                                    new Vector2(Position2.x, Position2.z),
                                                    new Vector2(lockPosi.x, lockPosi.z),
                                                    ratio);
            if(ratio >= 1f)
            {
                //手元に帰ってきたら攻撃終了
                if (returnFlag) 
                {
                    isThrowing = false;
                    mieScript.isThrowing = false;
                    this.transform.parent = parentTransform;
                    this.transform.localPosition = preLocalPosition;
                    gameObject.GetComponent<BoxCollider>().enabled = false;
                    return;
                }
                ratio = 0;
                totalTime = 0;
                //向きが逆になる
                Vector3 temp = prePosi;
                prePosi = lockPosi;
                lockPosi = temp;
                Position2 = GetP2(lockPosi, prePosi, 30);

                returnFlag = !returnFlag;
            }
            totalTime += Time.deltaTime;
        }
    }
    
    //ベジェ曲線上の位置を取得
    Vector3 BoomerangPosi(Vector2 P1, Vector2 P2, Vector2 P3, float t)
    {
        Vector2 newPosi = (1 - t) * (1 - t) * P1 + 2 * (1 - t) * t * P2 + t * t * P3;
        return new Vector3(newPosi.x, 3f, newPosi.y);
    }

    Vector3 GetP2(Vector3 P1, Vector3 P3, int angle)
    {
        Vector3 P2 = (P1 + P3) * 0.5f;
        Vector3 P2Sub = P2 - P3;
        float x = P2Sub.x * Mathf.Cos(angle) - P2Sub.z * Mathf.Sin(angle);
        float z = P2Sub.z * Mathf.Cos(angle) + P2Sub.x * Mathf.Sin(angle);

        P2 = P2 + (new Vector3(x, 0, z) - P2Sub);
        P2.y = 3f;
        return P2;
    }
}
