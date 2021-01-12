using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Net.NetworkInformation;

public class EnemyManager : MonoBehaviour
{
    private CharaEffect charaEffect;

    int nowNum = 0;
    private int inFieldNum = 0;

    //EnemyController[] ec = new EnemyController[];
    List<EnemyController> ecList = new List<EnemyController>();
    
    public GameObject appearParticle;
    GameObject obj;
    GameObject instance;
    
    void Start()
    {
        charaEffect = GameObject.Find("CharaScript").GetComponent<CharaEffect>();
    }

    public void SetEnemy(Enemy_Info enemyInfo,Vector3 posi)
    {
        obj = enemyInfo.Prefab;
        charaEffect.SetEnemyAppearParticle(posi);
        instance = (GameObject)Instantiate(obj,posi,Quaternion.Euler(0,200,0));
        instance.name = "Enemy"+(nowNum+1).ToString();

        //ec[nowNum] = instance.GetComponent<EnemyController>();
        //ec[nowNum].SetStatus(enemyInfo);
        ecList.Add(instance.GetComponent<EnemyController>());
        ecList[nowNum].SetStatus(enemyInfo);

        nowNum++;
        inFieldNum++;
    }
    
    public void LockPC(GameObject pc)
    {
        for(int i = 0; i < nowNum; i++)
        {
            if(ecList[i] != null)
            {
                ecList[i].LockPC(pc);
            }
        }
    }
    //位置関係を無視してロックPCを設定
    public void ForcedLockPC(GameObject pc)
    {
        for (int i = 0; i < nowNum; i++)
        {
            if (ecList[i] != null)
            {
                ecList[i].SetLockPC(pc);
            }
        }
    }
    //ロックPCがフィールド上にいるかの確認
    public void CheckLockPC()
    {
        for (int i = 0; i < nowNum; i++)
        {
            if (ecList[i] != null)
            {
                ecList[i].CheckLockPC();
            }
        }
    }

    public GameObject GetEnemy(string pcName)
    {
        int i = int.Parse(pcName.Replace("Enemy",""))-1;
        if (ecList[i] == null) return null;
        if (ecList[i].IsInField())
        {
            return ecList[i].gameObject;
        }
        else
        {
            return null;
        }
    }

    //全Enemyの中で、１番引数（PC)に距離が近いキャラを返す
    public GameObject GetNearEnemy(Vector3 pcPosi)
    {
        GameObject nearEnemy = null;
        float distance = 1000f;
        for (int i = 0; i < nowNum; i++)
        {
            if (ecList[i] != null)
            {
                float temp = Vector3.Distance(ecList[i].transform.position, pcPosi);
                if (temp < distance)
                {
                    distance = temp;
                    nearEnemy = ecList[i].gameObject;
                }
            }
        }
        return nearEnemy;
    }

    public void Desporn()
    {
        inFieldNum--;
    }
    public int GetInFieldNum()
    {
        return inFieldNum;
    }
}
