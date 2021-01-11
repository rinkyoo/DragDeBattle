using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Common;

public class CharaInfoManager : MonoBehaviour
{
    
    private DataHolder dataHolder;
    
    [SerializeField, Multiline(5)] private string[] str;

    //実装済み全キャラのリスト
    [SerializeField] private List<Chara_Info> AllChara = new List<Chara_Info>();
    //所持しているキャラのリスト
    private List<Chara_Info> charaList = new List<Chara_Info>();
    //編成キャラ情報
    private Chara_Info[][] formationChara = new Chara_Info[Define.ptNum][];
    //サポートキャラのリスト
    private List<Chara_Info> supportList = new List<Chara_Info>();

    void Awake()
    {
        dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
        //編成キャラ用・ジャグ配列の初期化
        for(int i=0;i<Define.ptNum;i++)
        {
            formationChara[i] = new Chara_Info[Define.charaNum];
        }
        
        //全所持キャラ情報のロード
        string path = Application.dataPath + "/Data/CharaData/MyChara/";
        string[] files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
        for(int i=0;i<files.Length;i++)
        {
            charaList.Add(LoadMyChara(files[i]));
        }
        //サポートキャラ情報のロード
        path = Application.dataPath + "/Data/SupportData/";
        files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
        for(int i=0;i<files.Length;i++)
        {
            supportList.Add(LoadSupportInfo(files[i]));
        }
        //編成情報のロード
        for(int i=0;i<Define.ptNum;i++)
        {
            FormationInfo temp = LoadFormationInfo(i+1);
            for(int j=0;j<Define.charaNum-1;j++)
            {
                formationChara[i][j] = charaList.Find(chara => chara.Name == temp.charaName[j]);
            }
        }
    }
    //キャラ情報の保存
    public void SaveCharaInfo(Chara_Info setCharaInfo)
    {
        MyChara setMyChara = new MyChara();
        setMyChara.Name = setCharaInfo.Name;
        setMyChara.Level = setCharaInfo.Level;
        setMyChara.NowEXP = setCharaInfo.NowEXP;
        setMyChara.NextEXP = setCharaInfo.NextEXP;
        setMyChara.HP = setCharaInfo.HP;
        setMyChara.SP = setCharaInfo.SP;
        setMyChara.STR = setCharaInfo.STR;
        setMyChara.VIT = setCharaInfo.VIT;
        setMyChara.AGI = setCharaInfo.AGI;
        setMyChara.SpeedATK = setCharaInfo.SpeedATK;
        setMyChara.RangeATK = setCharaInfo.RangeATK;
        StreamWriter writer;
        string jsonstr = JsonUtility.ToJson(setMyChara);
        string name = setMyChara.Name;
        writer = new StreamWriter(Application.dataPath + "/Data/CharaData/MyChara/MyChara" + name + ".json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }
    /*
    //キャラ情報の保存
    public void SaveCharaInfo(MyChara newChara)
    {
        StreamWriter writer;
        string jsonstr = JsonUtility.ToJson (newChara);
        int id = newChara.ID;
        writer = new StreamWriter(Application.dataPath + "/Data/CharaData/MyChara"+id.ToString()+".json", false);
        writer.Write (jsonstr);
        writer.Flush ();
        writer.Close ();
    }
    //キャラ情報のロード(IDで）
    public MyChara LoadCharaInfo(int i)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader (Application.dataPath + "/Data/CharaData/MyChara"+i.ToString()+".json");
        datastr = reader.ReadToEnd ();
        reader.Close ();
        
        return JsonUtility.FromJson<MyChara> (datastr);
    }
    //キャラ情報ロード（Pathで）
    public MyChara LoadCharaInfo(string path)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader (path);
        datastr = reader.ReadToEnd ();
        reader.Close ();
        
        return JsonUtility.FromJson<CharaInfo> (datastr);
    }
    */

    Chara_Info SetCharaInfoStatus(Chara_Info setCharaInfo, MyChara setMyChara)
    {
        setCharaInfo.Level = setMyChara.Level;
        setCharaInfo.NowEXP = setMyChara.NowEXP;
        setCharaInfo.NextEXP = setMyChara.NextEXP;
        setCharaInfo.HP = setMyChara.HP;
        setCharaInfo.STR = setMyChara.STR;
        setCharaInfo.VIT = setMyChara.VIT;
        setCharaInfo.AGI = setMyChara.AGI;
        setCharaInfo.SpeedATK = setMyChara.SpeedATK;
        setCharaInfo.RangeATK = setMyChara.RangeATK;

        return setCharaInfo;
    }

    //MyCharaをロードし、ステータスを設定（Pathで）
    Chara_Info LoadMyChara(string path)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(path);
        datastr = reader.ReadToEnd();
        reader.Close();

        MyChara setMyChara = JsonUtility.FromJson<MyChara>(datastr);
        Chara_Info setCharaInfo = AllChara.Find(chara => chara.Name == setMyChara.Name);
        return SetCharaInfoStatus(setCharaInfo.Clone(), setMyChara);
    }

    //***************************************************
    //サポートキャラ情報のロード(Pathで）
    Chara_Info LoadSupportInfo(string path)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader (path);
        datastr = reader.ReadToEnd ();
        reader.Close ();

        MyChara setMyChara = JsonUtility.FromJson<MyChara>(datastr);

        Chara_Info setCharaInfo = AllChara.Find(chara => chara.Name == setMyChara.Name);

        return SetCharaInfoStatus(setCharaInfo.Clone(), setMyChara);
    }
    //***************************************************
    //編成情報の保存
    public void SaveFormationInfo(Chara_Info[] charas,int i)
    {
        formationChara[i-1] = charas;
        //編成中のCharaInfoから、nameのみを抽出
        FormationInfo newFormation = new FormationInfo();
        for(int j=0;j<Define.charaNum-1;j++)
        {
            newFormation.charaName[j] = charas[j].Name;
        }
        
        StreamWriter writer;
        string jsonstr = JsonUtility.ToJson (newFormation);
        writer = new StreamWriter(Application.dataPath + "/Data/FormationData/Formation"+i.ToString()+".json", false);
        writer.Write (jsonstr);
        writer.Flush ();
        writer.Close ();
    }
    //編成情報のロード（編成番号で）
    FormationInfo LoadFormationInfo(int i)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader (Application.dataPath + "/Data/FormationData/Formation"+i.ToString()+".json");
        datastr = reader.ReadToEnd ();
        reader.Close ();
        
        return JsonUtility.FromJson<FormationInfo> (datastr);
    }
    //***************************************************
    
    public void AddCharaInfo(Chara_Info setChara)
    {
        SaveCharaInfo(setChara);
        charaList.Add(setChara);
    }
    
    public void SetSupportChara(Chara_Info setChara,int i)
    {
        formationChara[i][6] = setChara;
    }
    
    public List<Chara_Info> GetCharaList()
    {
        return charaList;
    }
    public List<Chara_Info> GetSupportList()
    {
        return supportList;
    }
    public Chara_Info[][] GetFormationChara()
    {
        return formationChara;
    }
    
    public void SetQuestChara(int i)
    {
        dataHolder.SetFormationChara(formationChara[i]);
    }

}
