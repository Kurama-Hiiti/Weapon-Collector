using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SoundManager;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    //現在のシーン
    private Scene scene;

    //シーン名
    public string sceneName;

    //遷移時間
    [SerializeField]
    private float transitionTime;

    //フェードアウト時の画像
    [SerializeField]
    private Image feedOutImage;

    //フェードアウト用パネル
    [SerializeField]
    private GameObject feedOutPanel;

    //フェードアウト時の画像のα値
    private float alpha = 0.0f;

    //フェードアウトのための経過時間
    private float elapsedTime = 0.0f;

    //ゲームスタート時に表示されているボタンを含んだパネル
    [SerializeField]
    private GameObject startPanel;

    //ゲーム開始ボタンを含んだパネル
    [SerializeField]
    private GameObject gameStartPanel;

    //ゲーム中のメニューボタンで開かれるメニューパネル
    [SerializeField]
    private GameObject menuPanel;

    //高難易度挑戦可能フラグ
    private bool isPlayHard, isPlayVeryHard;

    //高難易度挑戦可能フラグ管理用数値
    private int playHard, playVeryHard;

    //高難易度挑戦ボタン
    [SerializeField]
    private GameObject hardButton, veryHardButton;


    private void Awake()
    {
        //シングルトン化
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        feedOutPanel.SetActive(false);

        scene = SceneManager.GetActiveScene();

        sceneName = scene.name;

    }

    //リトライ
    public void RetryGame()
    {
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        StartCoroutine(FeedOut());
        Invoke(nameof(Retry), transitionTime);
    }

    private void Retry()
    {
        SceneManager.LoadScene(sceneName);
    }


    //ゲームメインシーンへの遷移
    //ノーマル
    public void GameMainNomal()
    {
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        StartCoroutine(FeedOut());
        Invoke(nameof(MainNomal), transitionTime);
    }

    private void MainNomal()
    {
        SceneManager.LoadScene("GameMainNomal");
    }

    //ハード
    public void GameMainHard()
    {
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        StartCoroutine(FeedOut());
        Invoke(nameof(MainHard), transitionTime);
    }

    private void MainHard()
    {
        SceneManager.LoadScene("GameMainHard");
    }

    //ベリーハード
    public void GameMainVeryHard()
    {
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        StartCoroutine(FeedOut());
        Invoke(nameof(MainVeryHard), transitionTime);
    }

    private void MainVeryHard()
    {
        SceneManager.LoadScene("GameMainVeryHard");
    }

    //タイトルシーンへの遷移
    public void TitleScene()
    {
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        //ゲームの途中でタイトルに戻った場合のお金の加算
        GameManager.instance.GetMoney();

        StartCoroutine(FeedOut());
        Invoke(nameof(Title), transitionTime);
    }

    private void Title()
    {
        SceneManager.LoadScene("Title");
    }

    public void NextWave()
    {
        StartCoroutine(FeedOut());
        Invoke(nameof(NextWaveStart), transitionTime + 2.0f);
    }

    private void NextWaveStart()
    {
        feedOutImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        Player.instance.isNewWave = true;
        feedOutPanel.SetActive(false);

    }

    //フェードアウト用コルーチン
    private IEnumerator FeedOut()
    {
        feedOutPanel.SetActive(true);

        //経過時間
        elapsedTime = 0;

        //経過時間に応じて画像を暗転させる
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            alpha = elapsedTime / transitionTime;

            feedOutImage.color = new Color(0.0f, 0.0f, 0.0f, alpha);

            yield return null;
        }

    }


    //メニュー画面表示
    public void ShowMenuPanel()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        menuPanel.SetActive(true);
    }

    //メニュー画面非表示
    public void HiddenMenuPanel()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        menuPanel.SetActive(false);
    }



    //タイトルシーンでの関数

    //ゲーム開始ボタン表示関数
    public void ShowGameStartPanel()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        startPanel.SetActive(false);
        gameStartPanel.SetActive(true);

        //高難易度挑戦可能判定
        PlayHighLevelJudge();
    }

    //バックボタン
    public void BackButton()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);
        startPanel.SetActive(true);

        gameStartPanel.SetActive(false);
    }


    //高難易度挑戦可能判定関数
    private void PlayHighLevelJudge()
    {
        playHard = PlayerPrefs.GetInt("PlayHard", 0);

        playVeryHard = PlayerPrefs.GetInt("PlayVeryHard",0);

        if (playHard != 0)
        {
            isPlayHard = true;
        }
        else
        {
            isPlayHard = false;
        }

        if (playVeryHard != 0)
        {
            isPlayVeryHard = true;
        }
        else
        {
            isPlayVeryHard = false;
        }

        //ボタンアクティブ判定
        HighLevelButtonActiveCheck();

    }

    //ハード、ベリーハードボタンアクティブ判定
    private void HighLevelButtonActiveCheck()
    {
        if (isPlayHard)
        {
            hardButton.GetComponent<Button>().interactable = true;

            hardButton.GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f ,1.0f);
        }
        else 
        {
            hardButton.GetComponent<Button>().interactable = false;

            float colorNum = (200f / 255f);

            hardButton.GetComponentInChildren<Text>().color = new Color(colorNum, colorNum, colorNum, 0.5f);
        }

        if (isPlayVeryHard)
        {
            veryHardButton.GetComponent<Button>().interactable = true;

            veryHardButton.GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else 
        {
            veryHardButton.GetComponent<Button>().interactable = false;

            float colorNum = (200f / 255f);

            veryHardButton.GetComponentInChildren<Text>().color = new Color(colorNum, colorNum, colorNum, 0.5f);
        }

    }

}
