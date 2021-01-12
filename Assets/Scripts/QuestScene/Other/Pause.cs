using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private QuestController questController;
    private AudioManager audioManager;
    [SerializeField] private GameObject PausePanel;
    
    void Awake()
    {
        questController = GameObject.Find("QuestController").GetComponent<QuestController>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void PauseClicked()
    {
        audioManager.Button1();
        questController.PauseBattle();
        PausePanel.SetActive(true);
    }
    
    public void ExitClicked()
    {
        audioManager.Button1();
        PausePanel.SetActive(false);
        questController.ResumeBattle();
    }
    
    public void RetireClicked()
    {
        audioManager.Button1();
        ExitClicked();
        questController.GoHomeButtonClicked();
    }
}
