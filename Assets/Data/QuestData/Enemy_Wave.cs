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
        get { return EnemyPosi; }
    }

}