using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;

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

    //ボタンとキャラ情報のセット＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
    public class ButtonAndChara
    {
        public Button button;
        public Chara_Info chara;
        public ButtonAndChara(Button b,Chara_Info c)
        {
            button = b;
            chara = c;
        }
    }
    List<ButtonAndChara> buttonAndChara = new List<ButtonAndChara>(); //全所持キャラ用リスト
    List<ButtonAndChara> setButtonChara = new List<ButtonAndChara>(); //表示用リスト

    [SerializeField] Toggle dirToggle;
    [SerializeField] TextMeshProUGUI dirText;

    //フィルター用トグル
    [SerializeField] Toggle allAttackToggle;
    [SerializeField] Toggle kinkyoriToggle;
    [SerializeField] Toggle enkyoriToggle;
    [SerializeField] Toggle alllevelToggle;
    [SerializeField] Toggle level10Toggle;
    [SerializeField] Toggle level20Toggle;
    //フィルター判定用変数
    string attackTypeFiter = "All";
    int levelFilter = 0;

    //ソート用トグル
    [SerializeField] Toggle idToggle;
    [SerializeField] Toggle levelToggle;
    [SerializeField] Toggle strToggle;
    [SerializeField] Toggle vitToggle;

    [SerializeField] GameObject sortFilterPanel;
    //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊

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

            /*
            ソートやフィルター機能の追加用（テスト）
             */
            buttonAndChara.Add(new ButtonAndChara(charaButton, chara));
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

    /*
     * 以下、ソート、フィルターのための機能
     */
    //表示キャラを更新
    void UpdateButtonCharaIndex()
    {
        //フィルターの条件に沿ったキャラのみ抽出
        setButtonChara = buttonAndChara
                        .Where(x => x.chara.AttackType == attackTypeFiter || attackTypeFiter == "All")
                        .Where(x => x.chara.Level >= levelFilter)
                        .ToList();
        foreach(Transform t in charaContent.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach(ButtonAndChara setBC in setButtonChara)
        {
            setBC.button.transform.SetAsLastSibling();
            setBC.button.transform.gameObject.SetActive(true);
        }
    }

    #region フィルター用トグルの設定
    //攻撃タイプのフィルター設定
    public void OnAllAttackToggleChanged()
    {
        if (allAttackToggle.isOn) attackTypeFiter = "All";
        UpdateButtonCharaIndex();
    }
    public void OnKinkyoriToggleChanged()
    {
        if (kinkyoriToggle.isOn) attackTypeFiter = "近距離";
        UpdateButtonCharaIndex();
    }
    public void OnEnkyoriAttackToggleChanged()
    {
        if (enkyoriToggle.isOn) attackTypeFiter = "遠距離";
        UpdateButtonCharaIndex();
    }
    //レベル制限でのフィルター設定
    public void OnAllLevelToggleChanged()
    {
        if (alllevelToggle.isOn) levelFilter = 0;
        UpdateButtonCharaIndex();
    }
    public void OnLevel10ToggleChanged()
    {
        if (level10Toggle.isOn) levelFilter = 10;
        UpdateButtonCharaIndex();
    }
    public void OnLevel20ToggleChanged()
    {
        if (level20Toggle.isOn) levelFilter = 20;
        UpdateButtonCharaIndex();
    }
    #endregion

    #region ソート用トグルの設定
    //キャラIDでの並び替え用トグル
    public void OnIDToggleChanged()
    {
        if (idToggle.isOn)
        {
            //IDでソート（降順で）
            buttonAndChara = buttonAndChara.OrderByDescending(x => x.chara.ID).ToList();
            //昇順状態だったら反転
            if (!dirToggle.isOn) buttonAndChara.Reverse();
            UpdateButtonCharaIndex();
        }
    }
    //キャラLevelでの並び替え用トグル
    public void OnLevelToggleChanged()
    {
        if (levelToggle.isOn)
        {
            buttonAndChara = buttonAndChara.OrderByDescending(x => x.chara.Level).ToList();
            if (!dirToggle.isOn) buttonAndChara.Reverse();
            UpdateButtonCharaIndex();
        }
    }
    //キャラSTRでの並び替え用トグル
    public void OnSTRToggleChanged()
    {
        if (strToggle.isOn)
        {
            buttonAndChara = buttonAndChara.OrderByDescending(x => x.chara.STR).ToList();
            if (!dirToggle.isOn) buttonAndChara.Reverse();
            UpdateButtonCharaIndex();
        }
    }
    //キャラVITでの並び替え用トグル
    public void OnVITToggleChanged()
    {
        if (vitToggle.isOn)
        {
            buttonAndChara = buttonAndChara.OrderByDescending(x => x.chara.VIT).ToList();
            if (!dirToggle.isOn) buttonAndChara.Reverse();
            UpdateButtonCharaIndex();
        }
    }
    #endregion

    //昇順降順の切り替え用トグル
    public void OnToggleChanged()
    {
        dirText.text = dirToggle.isOn ? "降順" : "昇順";
        buttonAndChara.Reverse();
        UpdateButtonCharaIndex();
    }

    public void SortFilterButtonClicked()
    {
        sortFilterPanel.SetActive(true);
    }
    public void SortFilterBackButtonClicked()
    {
        sortFilterPanel.SetActive(false);
    }
}
