using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class QuestClearManager : MonoBehaviour
{
    private DataHolder dataHolder;
    private AudioManager audioManager;

    #region Inspectorで取得するGameObject関連
    [SerializeField] GameObject questFinCamera;
    [SerializeField] GameObject questFinUICamera;
    [SerializeField] GameObject questFinCanvas;
    [SerializeField] GameObject clearText;
    [SerializeField] GameObject goHomeButton;
    [SerializeField] GameObject sceneLoadPanel;
    private CanvasGroup sceneLoadCanvasGroup;
    //クエスト結果用
    [SerializeField] GameObject resultObj;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject titleObj;
    [SerializeField] GameObject timeObj;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] GameObject coinObj;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] GameObject itemScrollView;
    [SerializeField] GameObject dropItemContent;
    [SerializeField] Image dropItemImage;
    [SerializeField] GameObject expObj;
    [SerializeField] GameObject nextExpObj;
    [SerializeField] TextMeshProUGUI nowLevelText;
    [SerializeField] TextMeshProUGUI nextLevelText;
    [SerializeField] TextMeshProUGUI getExpText;
    [SerializeField] TextMeshProUGUI nextExpText;
    [SerializeField] Slider expSlider; //min maxの値はそれぞれ0,1で固定
    #endregion

    Sequence seq = DOTween.Sequence();
    int beforeLevel;
    int sliderValue;

    void Start()
    {
        dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        sceneLoadCanvasGroup = sceneLoadPanel.GetComponent<CanvasGroup>();
        beforeLevel = dataHolder.GetLevel(); //最初のプレイヤーレベルを保持
        expSlider.value = dataHolder.GetExp() / dataHolder.GetNextExp(); //最初のexpSlider.valueの値を設定
    }

    public void QuestClear(string clearTime,int totalExp,int totalCoin,List<ExpItem_Info> expItemList)
    {
        audioManager.QuestClearBGM();

        timeText.text = clearTime;
        coinText.text = totalCoin.ToString();
        getExpText.text = totalExp.ToString();
        nowLevelText.text = beforeLevel.ToString();
        nextLevelText.text = (beforeLevel + 1).ToString();

        foreach (ExpItem_Info item in expItemList)
        {
            Image itemImage = Instantiate(dropItemImage, dropItemContent.transform.position, Quaternion.identity) as Image;
            itemImage.gameObject.transform.SetParent(dropItemContent.transform);
            itemImage.transform.localScale = new Vector3(1f, 1f, 1f);
            itemImage.sprite = item.Icon;
        }

        questFinCanvas.SetActive(true);
        questFinCamera.SetActive(true);
        questFinUICamera.SetActive(true);
        questFinCamera.GetComponent<Animator>().SetTrigger("QuestFin");
        clearText.SetActive(true);
        Invoke("AppearResult", 5.5f);

        #region アカウントデータの更新
        dataHolder.SaveClearData();
        dataHolder.PlusExp(totalExp);
        dataHolder.PlusCoin(totalCoin);
        dataHolder.PlusExpItem(expItemList);
        dataHolder.SaveAccountData();
        #endregion
    }

    void AppearResult()
    {
        clearText.SetActive(false);

        seq = DOTween.Sequence();
        seq.Append(resultObj.transform.DOLocalMoveY(0, 1))
            .Append(resultPanel.GetComponent<Image>().DOFillAmount(1, 1f))
            .InsertCallback(2f, () =>
             {
                 audioManager.System24();
                 titleObj.SetActive(true);
             })
            .InsertCallback(2.5f, () =>
             {
                 audioManager.System24();
                 timeObj.SetActive(true);
             })
            .InsertCallback(3f, () =>
             {
                 audioManager.System24();
                 coinObj.SetActive(true);
             })
            .InsertCallback(3.5f, () =>
            {
                audioManager.System24();
                itemScrollView.SetActive(true);
            })
            .InsertCallback(4f, () =>
             {
                 audioManager.System24();

                 expObj.SetActive(true);
             });
        //beforeLevel ～ （経験値獲得後のレベルー１）までのスライダーのアニメーション設定
        for (int i=beforeLevel;i<dataHolder.GetLevel();i++)
        {
            int j = i;
            seq.Append(expSlider.DOValue(1f, 0.5f, false))
                .AppendCallback(() =>
                {
                    nowLevelText.text = (j + 1).ToString();
                    nextLevelText.text = (j + 2).ToString();
                    expSlider.value = 0;
                });
        }
        //経験値獲得後のレベルのスライダーのアニメーション設定
        float sliderValue = (float)(dataHolder.GetExp()) / dataHolder.GetNextExp();
        seq.Append(expSlider.DOValue(dataHolder.GetExpGageValue(), 1f, false))
            .AppendCallback(() =>
            {
                nextExpText.text = (dataHolder.GetNextExp() - dataHolder.GetExp()).ToString();
                nextExpObj.gameObject.SetActive(true);
            });
        seq.AppendInterval(1f)
            .AppendCallback(() =>
            {
                audioManager.System24();
                goHomeButton.SetActive(true);
            });
        seq.Play();
    }

    void Update()
    {
        if(seq.IsPlaying())
        {
            if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0))
            {
                seq.Complete();
            }
        }
    }

    public void GoHomeButtonClicked()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            sceneLoadPanel.SetActive(true);
            sceneLoadCanvasGroup.alpha = 0f;
        })
        .Append(sceneLoadCanvasGroup.DOFade(1f, 0.5f))
        .AppendCallback(() =>
        {
            SceneManager.LoadScene("Home");
        });
        seq.Play();
    }
}
