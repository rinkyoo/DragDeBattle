using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName="Wave",menuName="Create_Enemy_Wave")]
public class Enemy_Wave : ScriptableObject
{
    [SerializeField]
    private int AppearTiming;
    public int TIMING{
        get{ return AppearTiming; }
    }
    [SerializeField]
    private int AppearTime;
    public int TIME{
        get{ return AppearTime; }
    }

    [SerializeField]
    private List<Enemy_Info> EnemyInfo;
    public List<Enemy_Info> Info
    {
        get { return EnemyInfo; }
    }
    /*
    [SerializeField]
    private List<string> EnemyName;
    public List<string> NAME{
        get{ return EnemyName; }
    }
    */
    [SerializeField]
    private List<Vector3> EnemyPosi;
    public List<Vector3> Posi{
        get{ return EnemyPosi; }
    }

}
