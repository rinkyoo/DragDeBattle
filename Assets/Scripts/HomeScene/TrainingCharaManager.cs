using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TrainingCharaManager : MonoBehaviour
{
    //編成情報を取得、保存する用
    private CharaInfoManager charaInfoManager;
    //SE用
    AudioManager audioManager;

    private List<Chara_Info> charaList; //所持しているキャラのリスト
    Chara_Info trainingChara; //強化するキャラ

    [SerializeField] GameObject trainingPanel;
    [SerializeField] GameObject charaContent;
    [SerializeField] Button CharaButton; //変更するキャラを選択するボタン

    void Awake()
    {
        charaInfoManager = GameObject.Find("CharaInfoManager").GetComponent<CharaInfoManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        SetCharaList();
    }

    void SetCharaList()
    {
        charaList = charaInfoManager.GetCharaList();

        //所持キャラを全部表示
        foreach (Chara_Info chara in charaList)
        {
            Button charaButton = Instantiate(CharaButton, charaContent.transform.position, Quaternion.identity) as Button;
            charaButton.transform.SetParent(charaContent.transform);
            charaButton.transform.localScale = new Vector3(1f, 1f, 1f);
            charaButton.GetComponent<Image>().sprite = chara.Icon;
            
            charaButton.onClick.AddListener(() =>
            {
                trainingChara = chara;
                gameObject.GetComponent<TrainingApplyManager>().SetPanel(trainingChara); //キャラ強化画面へ移行
            });
        }
    }

    public void SaveCharaInfo(Chara_Info chara)
    {
        charaInfoManager.SaveCharaInfo(chara);
    }

    public void TrainingClicked()
    {
        audioManager.Button1();
        trainingPanel.SetActive(true);

    }
    public void BackHomeClicked()
    {
        audioManager.Button1();
        trainingPanel.SetActive(false);
    }
}
