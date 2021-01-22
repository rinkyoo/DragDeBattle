using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //BGM
    [SerializeField] AudioClip homeBGM;
    [SerializeField] AudioClip questBGM;
    [SerializeField] AudioClip questClearBGM;

    //共通
    [SerializeField] AudioClip button1;

    //ホームシーン
    [SerializeField] AudioClip training;
    [SerializeField] AudioClip levelUp;

    //クエストシーン
    [SerializeField] AudioClip swing;
    [SerializeField] AudioClip damage1;
    [SerializeField] AudioClip bomb;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip water;
    [SerializeField] AudioClip pcDie;
    [SerializeField] AudioClip lastEnemyDie;
    [SerializeField] AudioClip questFailed;
    [SerializeField] AudioClip system24;
    [SerializeField] AudioClip levelGage;
    [SerializeField] AudioClip skillPanel;

    AudioSource[] audioSource;

    private static bool firstStart = true;

    //オブジェクトが重複しないように
    void Awake()
    {
        if (firstStart)
        {
            DontDestroyOnLoad(this);
            firstStart = false;
            audioSource = GetComponents<AudioSource>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PauseBGM()
    {
        audioSource[0].Pause();
    }
    public void UnPauseBGM()
    {
        audioSource[0].UnPause();
    }
    public void HomeBGM()
    {
        audioSource[0].Stop();
        audioSource[0].clip = homeBGM;
        audioSource[0].Play();
    }
    public void QuestBGM()
    {
        audioSource[0].Stop();
        audioSource[0].clip = questBGM;
        audioSource[0].Play();
    }
    public void QuestClearBGM()
    {
        audioSource[0].Stop();
        audioSource[0].clip = questClearBGM;
        audioSource[0].Play();
    }

    public void Button1()
    {
        audioSource[1].PlayOneShot(button1);
    }

    public void Training()
    {
        audioSource[1].PlayOneShot(training);
    }
    public void LevelUp()
    {
        audioSource[1].PlayOneShot(levelUp);
    }

    public void Swing()
    {
        audioSource[1].PlayOneShot(swing);
    }
    public void Damage1()
    {
        audioSource[1].PlayOneShot(damage1);
    }
    public void Bomb()
    {
        audioSource[1].PlayOneShot(bomb);
    }
    public void Shoot()
    {
        audioSource[1].PlayOneShot(shoot);
    }
    public void Water()
    {
        audioSource[1].PlayOneShot(water);
    }
    public void PCDie()
    {
        audioSource[1].PlayOneShot(pcDie);
    }
    public void LastEnemyDie()
    {
        audioSource[1].PlayOneShot(lastEnemyDie);
    }
    public void QuestFailed()
    {
        audioSource[1].PlayOneShot(questFailed);
    }
    public void System24()
    {
        audioSource[1].PlayOneShot(system24);
    }
    public void LevelGage()
    {
        audioSource[1].PlayOneShot(levelGage);
    }
    public void SkillPanel()
    {
        audioSource[1].PlayOneShot(skillPanel);
    }
}
