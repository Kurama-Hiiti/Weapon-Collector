using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SoundManager : MonoBehaviour
{
    //static化
    public static SoundManager instance;

    public enum SoundType {
        Attack, AttackButton, Buff, CriticalTakeDamage, CriticalHit ,Damage, DeBuff, EnemyDown,
        GameClear, GameOver, Miss, SystemButton, UpGradeButton, Drop, Select ,NotSelect
    }

    //SEの配列
    [SerializeField]
    private AudioClip[] se;

    //SE用
    private AudioSource audioSource;

    //メインBGM
    [SerializeField]
    private AudioSource mainBGM;

    //ゲームクリアBGM
    [SerializeField]
    private AudioSource gameClearBGM;

    //ゲームオーバーBGM
    [SerializeField]
    private AudioSource gameOverBGM;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }


    //SEを鳴らす関数
    public void PlaySE(SoundType type)
    {
        int index = (int)type;

        if (index >= 0 && index < se.Length)
        {
            audioSource.PlayOneShot(se[index]);
        }

    }

    //クリアBGM再生処理
    public void GameClearBGM()
    {
        mainBGM.Stop();
        Invoke(nameof(GameClearBGMPlay), 0.5f);
    }

    private void GameClearBGMPlay()
    {
        gameClearBGM.Play();
    }


    //ゲームオーバーBGM再生処理
    public void GameOverBGM()
    {
        mainBGM.Stop();
        Invoke(nameof(GameOverBGMPlay), 0.5f);
    }


    private void GameOverBGMPlay()
    {
        gameOverBGM.Play();
    }
}
