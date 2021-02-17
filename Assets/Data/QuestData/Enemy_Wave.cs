using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Enemy_Wave
{
    [SerializeField]
    private int AppearTiming;
    public int TIMING
    {
        get { return AppearTiming; }
    }
    [SerializeField]
    private int AppearTime;
    public int TIME
    {
        get { return AppearTime; }
    }

    [SerializeField]
    private List<Enemy_Info> EnemyInfo;
    public List<Enemy_Info> Info
    {
        get { return EnemyInfo; }
    }

    [SerializeField]
    private List<Vector3> EnemyPosi;
    public List<Vector3> Posi
    {
        get
        {
            //想定されているフィールド座標の外に設定されていたら修正
            for(int i=0;i<EnemyPosi.Count;i++)
            {
                Vector3 temp = EnemyPosi[i];
                temp.y = 0;
                if (EnemyPosi[i].x < 0) temp.x = 0;
                if (EnemyPosi[i].x > 40f) temp.x = 40f;
                if (EnemyPosi[i].z < 10f) temp.z = 10f;
                if (EnemyPosi[i].z > 140f) temp.z = 140f;
                EnemyPosi[i] = temp;
            }
            return EnemyPosi;
        }
    }

}