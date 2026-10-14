using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class GameRuleManager : MonoBehaviour
{
    //ゲームのルール説明UIの表示処理

    //メインUIパネル
    [SerializeField]
    private GameObject mainPanel;

    //ゲームルール画像リスト
    [SerializeField]
    private GameObject[] rulePage;

    //ページ番号
    private int pageNum;

    //表示されるページ番号
    private int showPageNum;


    //ページテキスト
    [SerializeField]
    private Text pageNumText;

    //次ページボタン
    [SerializeField]
    private GameObject nextButton;

    //前ページボタン
    [SerializeField]
    private GameObject backButton;



    private void Start()
    {
        pageNum = 0;
        showPageNum = pageNum + 1;
        rulePage[pageNum].SetActive(true);

    }

    private void Update()
    {
        pageNumText.text = showPageNum + "/" + rulePage.Length;

        ButtonManager();
    }


    //ゲームルールボタンクリック時画面遷移関数
    public void MoveGameRulePage()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        mainPanel.transform.localPosition = new Vector3(1920f, 0f, 0f);
    }

    //戻るボタン
    public void BackButton()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        mainPanel.transform.localPosition = Vector3.zero;


        rulePage[pageNum].SetActive(false);
        pageNum = 0;
        rulePage[pageNum].SetActive(true);
        showPageNum = pageNum + 1;

    }


    //次ページへ
    public void NextPage()
    {
        if (pageNum < rulePage.Length - 1)
        {
            //SE
            SoundManager.instance.PlaySE(SoundType.SystemButton);

            rulePage[pageNum].SetActive(false);
            pageNum++;

            rulePage[pageNum].SetActive(true);

            showPageNum = pageNum + 1;
        }
     
        
    }

    //前のページへ
    public void BackPage()
    {
        if (0 < pageNum)
        {
            //SE
            SoundManager.instance.PlaySE(SoundType.SystemButton);

            rulePage[pageNum].SetActive(false);
            pageNum--;

            rulePage[pageNum].SetActive(true);

            showPageNum = pageNum + 1;
        }

        
    }

    //ボタン管理
    private void ButtonManager()
    {
        //最後のページの時は次ページへ遷移するボタンを無効にする
        if (pageNum == rulePage.Length - 1)
        {
            nextButton.GetComponent<Button>().interactable = false;
        }
        else 
        {
            nextButton.GetComponent<Button>().interactable = true;
        }

        //最初のページの時は前ページへ遷移するボタンを無効にする
        if (pageNum == 0)
        {
            backButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            backButton.GetComponent <Button>().interactable = true;
        }
    }

}
