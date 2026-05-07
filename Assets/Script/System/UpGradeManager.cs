using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class UpGradeManager : MonoBehaviour
{
    //メインUIパネル
    [SerializeField]
    private GameObject mainPanel;

    //お金のテキスト
    [SerializeField]
    private Text moenyText;


    //武器アップグレード関係

    //アップグレード段階のテキスト
    [SerializeField]
    private Text weaponUpGradePhaseText;

    //必要費用テキスト
    [SerializeField]
    private Text weaponMoney;

    //必要費用数値
    private int phaseOneWeaponMoney, phaseTwoWeaponMoney;

    //武器アップグレードボタン
    [SerializeField]
    private GameObject weaponUpGradeButton;

    //現在の武器のアップグレードフェーズ
    private int weaponUpGradePhaseNum;

    //武器のアップグレード上限
    private int maxWeaponUpGradePhaseNum = 2;



    //攻撃力アップ関係
    //アップグレード段階テキスト
    [SerializeField]
    private Text attackUpGradePhaseText;

    //費用テキスト
    [SerializeField]
    private Text attackMoney;

    //攻撃力アップボタン
    [SerializeField]
    private GameObject attackUpGradeButton;

    //攻撃力のアップグレード必要費用
    private int attackUpGradePhaseOne, attackUpGradePhaseTwo, attackUpGradePhaseThree, attackUpGradePhaseFour, attackUpGradePhaseFive;

    //攻撃力のアップグレード段階
    private int attackUpGradePhaseNum;

    //攻撃力のアップグレード上限
    private int maxAttackUpGradePhaseNum = 5;


    //体力アップグレード関係

    //体力アップグレード段階テキスト
    [SerializeField]
    private Text healthUpGradePhaseText;

    //体力アップ費用テキスト
    [SerializeField]
    private Text healthMoneyText;

    //体力アップグレードボタン
    [SerializeField]
    private GameObject healthUpGradeButton;

    //体力のアップグレード費用
    private int healthUpGradePhaseOne, healthUpGradePhaseTwo, healthUpGradePhaseThree;

    //体力の(現在の)アップグレード段階
    private int healthUpGradePhaseNum;

    //体力のアップグレード上限
    private int maxHealthUpGradePhaseNum = 3;


    //回避率アップグレード関係

    //回避率アップグレード段階テキスト
    [SerializeField]
    private Text evasionRateUpGradePhaseText;

    //回避率アップグレード費用テキスト
    [SerializeField]
    private Text evasionRateMoneyText;

    //回避率のアップグレードボタン
    [SerializeField]
    private GameObject evasionRateUpGradeButton;

    //回避率のアップグレード費用
    private int evasionRateUpGradePhaseOne, evasionRateUpGradePhaseTwo, evasionRateUpGradePhaseThree, evasionRateUpGradePhaseFour, evasionRateUpGradePhaseFive;

    //回避率の（現在）アップグレード段階
    private int evasionRateUpGradePhaseNum;

    //回避率のアップグレード上限
    private int maxEvasionRateUpGradePhaseNum = 5;


    //クリティカル率アップグレード関係

    //クリティカル率アップグレード段階テキスト
    [SerializeField]
    private Text criticalRateUpGradePhaseText;

    //クリティカル率アップグレード費用テキスト
    [SerializeField]
    private Text criticalRateMoneyText;

    //クリティカル率アップグレードボタン
    [SerializeField]
    private GameObject criticalRateUpGradeButton;

    //クリティカル率アップグレード費用
    private int criticalRateUpGradePhaseOne, criticalRateUpGradePhaseTwo, criticalRateUpGradePhaseThree, criticalRateUpGradePhaseFour, criticalRateUpGradePhaseFive;

    //クリティカル率の（現在）アップグレード段階
    private int criticalRateUpGradePhaseNum;

    //クリティカル率のアップグレード上限
    private int maxCriticalRateUpGradePhaseNum = 5;


    //初期闘気アップグレード関係

    //闘気アップグレード段階テキスト
    [SerializeField]
    private Text energyUpGradePhaseText;

    //闘気アップグレード費用テキスト
    [SerializeField]
    private Text energyMoneyText;

    //闘気アップグレードボタン
    [SerializeField]
    private GameObject energyUpGradeButton;

    //闘気アップグレード費用
    private int energyUpGradePhaseOne, energyUpGradePhaseTwo, energyUpGradePhaseThree, energyUpGradePhaseFour, energyUpGradePhaseFive;

    //闘気の（現在）アップグレード段階
    private int energyUpGradePhaseNum;

    //闘気のアップグレード上限
    private int maxEnergyUpGradePhaseNum = 5;



    // Start is called before the first frame update
    void Start()
    {
        UpGradeMoneyMass();
    }

    // Update is called once per frame
    void Update()
    {
        //所持金表示
        moenyText.text = GameManager.instance.playerAllMoney.ToString();

        //ボタン管理関数
        WeaponUpgradeManagement();
        AttackUpGradeManagement();
        HealthUpGradeManagement();
        EvasionRateUpgradeManagement();
        CriticalRateUpGradeManagement();
        EnergyGradeManagement();




    }




    //アップグレードボタンクリック時画面遷移関数
    public void MoveUpGradePage()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        mainPanel.transform.localPosition = new Vector3(-1920f, 0f, 0f);
    }

    //メインパネルへ戻る
    public void BackPanel()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        mainPanel.transform.localPosition = Vector3.zero;
    }

    //アップグレードの必要費用まとめ関数
    private void UpGradeMoneyMass()
    {
        //武器アップグレード費用
        phaseOneWeaponMoney = 1000;
        phaseTwoWeaponMoney = 1500;

        //攻撃力のアップグレード費用
        attackUpGradePhaseOne = 200;
        attackUpGradePhaseTwo = 250;
        attackUpGradePhaseThree = 300;
        attackUpGradePhaseFour = 350;
        attackUpGradePhaseFive = 400;

        //体力のアップグレード費用
        healthUpGradePhaseOne = 300;
        healthUpGradePhaseTwo = 500;
        healthUpGradePhaseThree = 700;

        //回避率のアップグレード費用
        evasionRateUpGradePhaseOne = 100;
        evasionRateUpGradePhaseTwo = 150;
        evasionRateUpGradePhaseThree = 200;
        evasionRateUpGradePhaseFour = 250;
        evasionRateUpGradePhaseFive = 300;

        //クリティカル率のアップグレード費用
        criticalRateUpGradePhaseOne = 150;
        criticalRateUpGradePhaseTwo = 200;
        criticalRateUpGradePhaseThree = 250;
        criticalRateUpGradePhaseFour = 300;
        criticalRateUpGradePhaseFive = 350;

        //闘気アップグレード費用
        energyUpGradePhaseOne = 100;
        energyUpGradePhaseTwo = 150;
        energyUpGradePhaseThree = 200;
        energyUpGradePhaseFour = 250;
        energyUpGradePhaseFive = 300;



    }



    //武器のアップグレードボタン、テキスト管理関数
    private void WeaponUpgradeManagement()
    {
        weaponUpGradePhaseNum = PlayerPrefs.GetInt("WeaponUpGradePhase", 0);

        weaponUpGradePhaseText.text = weaponUpGradePhaseNum + "/" + maxWeaponUpGradePhaseNum;

        //現在のアップグレード段階を判定
        if (weaponUpGradePhaseNum == 0)
        {
            weaponMoney.text = phaseOneWeaponMoney.ToString();

            //現在の所持金が必要費用に達していない場合
            if (GameManager.instance.playerAllMoney < phaseOneWeaponMoney)
            {
                weaponUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                weaponUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (weaponUpGradePhaseNum == 1)
        {
            weaponMoney.text = phaseTwoWeaponMoney.ToString();

            if (GameManager.instance.playerAllMoney < phaseTwoWeaponMoney)
            {
                weaponUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                weaponUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (weaponUpGradePhaseNum == 2)
        {
            weaponMoney.text = "0";
            weaponUpGradeButton.GetComponent<Button>().interactable = false;

        }

    }

    //武器アップグレード関数
    public void WeaponUpGrade()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.UpGradeButton);

        if (weaponUpGradePhaseNum == 0)
        {

            PlayerPrefs.SetInt("WeaponUpGradePhase", 1);

            GameManager.instance.playerAllMoney -= phaseOneWeaponMoney;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetInt("WeaponPhase", 1);
        }
        else if (weaponUpGradePhaseNum == 1)
        {

            PlayerPrefs.SetInt("WeaponUpGradePhase", 2);

            GameManager.instance.playerAllMoney -= phaseTwoWeaponMoney;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetInt("WeaponPhase", 2);
        }

    }


    //攻撃力のアップグレードボタンテキスト管理
    private void AttackUpGradeManagement()
    {
        attackUpGradePhaseNum = PlayerPrefs.GetInt("AttackUpGradePhase", 0);

        attackUpGradePhaseText.text = attackUpGradePhaseNum + "/" + maxAttackUpGradePhaseNum;

        //現在のアップグレード段階を判定
        if (attackUpGradePhaseNum == 0)
        {
            attackMoney.text = attackUpGradePhaseOne.ToString();

            if (GameManager.instance.playerAllMoney < attackUpGradePhaseOne)
            {
                attackUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                attackUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (attackUpGradePhaseNum == 1)
        {
            attackMoney.text = attackUpGradePhaseTwo.ToString();

            if (GameManager.instance.playerAllMoney < attackUpGradePhaseTwo)
            {
                attackUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                attackUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (attackUpGradePhaseNum == 2)
        {
            attackMoney.text = attackUpGradePhaseThree.ToString();

            if (GameManager.instance.playerAllMoney < attackUpGradePhaseThree)
            {
                attackUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                attackUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (attackUpGradePhaseNum == 3)
        {
            attackMoney.text = attackUpGradePhaseFour.ToString();

            if (GameManager.instance.playerAllMoney < attackUpGradePhaseFour)
            {
                attackUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                attackUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (attackUpGradePhaseNum == 4)
        {
            attackMoney.text = attackUpGradePhaseFive.ToString();

            if (GameManager.instance.playerAllMoney < attackUpGradePhaseFive)
            {
                attackUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                attackUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (attackUpGradePhaseNum == 5)
        {
            attackMoney.text = "0";
            attackUpGradeButton.GetComponent<Button>().interactable = false;
        }
    }

    //攻撃力アップグレード関数
    public void AttackUpGrade()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.UpGradeButton);

        if (attackUpGradePhaseNum == 0)
        {

            PlayerPrefs.SetInt("AttackUpGradePhase", 1);

            GameManager.instance.playerAllMoney -= attackUpGradePhaseOne;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originAttack",7f);

        }
        else if (attackUpGradePhaseNum == 1)
        {

            PlayerPrefs.SetInt("AttackUpGradePhase", 2);

            GameManager.instance.playerAllMoney -= attackUpGradePhaseTwo;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originAttack", 9f);
        }
        else if (attackUpGradePhaseNum == 2)
        {

            PlayerPrefs.SetInt("AttackUpGradePhase", 3);

            GameManager.instance.playerAllMoney -= attackUpGradePhaseThree;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originAttack", 11f);
        }
        else if (attackUpGradePhaseNum == 3)
        {

            PlayerPrefs.SetInt("AttackUpGradePhase", 4);

            GameManager.instance.playerAllMoney -= attackUpGradePhaseFour;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originAttack", 13f);
        }
        else if (attackUpGradePhaseNum == 4)
        {

            PlayerPrefs.SetInt("AttackUpGradePhase", 5);

            GameManager.instance.playerAllMoney -= attackUpGradePhaseFive;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originAttack", 15f);
        }

    }



    //体力のアップグレードボタンの管理
    private void HealthUpGradeManagement()
    {
        healthUpGradePhaseNum = PlayerPrefs.GetInt("HealthUpGradePhase", 0);

        healthUpGradePhaseText.text = healthUpGradePhaseNum + "/" + maxHealthUpGradePhaseNum;

        if (healthUpGradePhaseNum == 0)
        {
            healthMoneyText.text = healthUpGradePhaseOne.ToString();

            if (GameManager.instance.playerAllMoney < healthUpGradePhaseOne)
            {
                healthUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                healthUpGradeButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (healthUpGradePhaseNum == 1)
        {
            healthMoneyText.text = healthUpGradePhaseTwo.ToString();

            if (GameManager.instance.playerAllMoney < healthUpGradePhaseTwo)
            {
                healthUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                healthUpGradeButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (healthUpGradePhaseNum == 2)
        {
            healthMoneyText.text = healthUpGradePhaseThree.ToString();

            if (GameManager.instance.playerAllMoney < healthUpGradePhaseThree)
            {
                healthUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                healthUpGradeButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (healthUpGradePhaseNum == 3)
        {
            healthMoneyText.text = "0";

            healthUpGradeButton.GetComponent<Button>().interactable = false;

        }

    }

    //体力のアップグレード
    public void HealthUpGrade()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.UpGradeButton);

        if (healthUpGradePhaseNum == 0)
        {

            PlayerPrefs.SetInt("HealthUpGradePhase", 1);

            GameManager.instance.playerAllMoney -= healthUpGradePhaseOne;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originHealth", 15);

        }
        else if (healthUpGradePhaseNum == 1)
        {

            PlayerPrefs.SetInt("HealthUpGradePhase", 2);

            GameManager.instance.playerAllMoney -= healthUpGradePhaseTwo;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originHealth", 20);
        }
        else if (healthUpGradePhaseNum == 2)
        {

            PlayerPrefs.SetInt("HealthUpGradePhase", 3);

            GameManager.instance.playerAllMoney -= healthUpGradePhaseThree;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originHealth", 25);
        }


    }


    //回避率のアップグレードボタン管理
    private void EvasionRateUpgradeManagement()
    {
        evasionRateUpGradePhaseNum = PlayerPrefs.GetInt("EvasionRateUpGradePhase", 0);

        evasionRateUpGradePhaseText.text = evasionRateUpGradePhaseNum + "/" + maxEvasionRateUpGradePhaseNum;

        //現在のアップグレード段階を判定
        if (evasionRateUpGradePhaseNum == 0)
        {
            evasionRateMoneyText.text = evasionRateUpGradePhaseOne.ToString();

            if (GameManager.instance.playerAllMoney < evasionRateUpGradePhaseOne)
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (evasionRateUpGradePhaseNum == 1)
        {
            evasionRateMoneyText.text = evasionRateUpGradePhaseTwo.ToString();

            if (GameManager.instance.playerAllMoney < evasionRateUpGradePhaseTwo)
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (evasionRateUpGradePhaseNum == 2)
        {
            evasionRateMoneyText.text = evasionRateUpGradePhaseThree.ToString();

            if (GameManager.instance.playerAllMoney < evasionRateUpGradePhaseThree)
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (evasionRateUpGradePhaseNum == 3)
        {
            evasionRateMoneyText.text = evasionRateUpGradePhaseFour.ToString();

            if (GameManager.instance.playerAllMoney < evasionRateUpGradePhaseFour)
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (evasionRateUpGradePhaseNum == 4)
        {
            evasionRateMoneyText.text = evasionRateUpGradePhaseFive.ToString();

            if (GameManager.instance.playerAllMoney < evasionRateUpGradePhaseFive)
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                evasionRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (evasionRateUpGradePhaseNum == 5)
        {
            evasionRateMoneyText.text = "0";
            evasionRateUpGradeButton.GetComponent<Button>().interactable = false;
        }
    }

    //回避率のアップグレード
    public void EvasionRateUpGrade()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.UpGradeButton);

        if (evasionRateUpGradePhaseNum == 0)
        {

            PlayerPrefs.SetInt("EvasionRateUpGradePhase", 1);

            GameManager.instance.playerAllMoney -= evasionRateUpGradePhaseOne;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originEvasionRate", 0.12f);

        }
        else if (evasionRateUpGradePhaseNum == 1)
        {

            PlayerPrefs.SetInt("EvasionRateUpGradePhase", 2);

            GameManager.instance.playerAllMoney -= evasionRateUpGradePhaseTwo;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originEvasionRate", 0.14f);
        }
        else if (evasionRateUpGradePhaseNum == 2)
        {

            PlayerPrefs.SetInt("EvasionRateUpGradePhase", 3);

            GameManager.instance.playerAllMoney -= evasionRateUpGradePhaseThree;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originEvasionRate", 0.16f);
        }
        else if (evasionRateUpGradePhaseNum == 3)
        {

            PlayerPrefs.SetInt("EvasionRateUpGradePhase", 4);

            GameManager.instance.playerAllMoney -= evasionRateUpGradePhaseFour;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originEvasionRate", 0.18f);
        }
        else if (evasionRateUpGradePhaseNum == 4)
        {

            PlayerPrefs.SetInt("EvasionRateUpGradePhase", 5);

            GameManager.instance.playerAllMoney -= evasionRateUpGradePhaseFive;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originEvasionRate", 0.2f);
        }
    }


    //クリティカル率のアップグレードボタン管理
    private void CriticalRateUpGradeManagement()
    {
        criticalRateUpGradePhaseNum = PlayerPrefs.GetInt("CriticalRateUpGradePhase", 0);

        criticalRateUpGradePhaseText.text = criticalRateUpGradePhaseNum + "/" + maxCriticalRateUpGradePhaseNum;

        //現在のアップグレード段階を判定
        if (criticalRateUpGradePhaseNum == 0)
        {
            criticalRateMoneyText.text = criticalRateUpGradePhaseOne.ToString();

            if (GameManager.instance.playerAllMoney < criticalRateUpGradePhaseOne)
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (criticalRateUpGradePhaseNum == 1)
        {
            criticalRateMoneyText.text = criticalRateUpGradePhaseTwo.ToString();

            if (GameManager.instance.playerAllMoney < criticalRateUpGradePhaseTwo)
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (criticalRateUpGradePhaseNum == 2)
        {
            criticalRateMoneyText.text = criticalRateUpGradePhaseThree.ToString();

            if (GameManager.instance.playerAllMoney < criticalRateUpGradePhaseThree)
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (criticalRateUpGradePhaseNum == 3)
        {
            criticalRateMoneyText.text = criticalRateUpGradePhaseFour.ToString();

            if (GameManager.instance.playerAllMoney < criticalRateUpGradePhaseFour)
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (criticalRateUpGradePhaseNum == 4)
        {
            criticalRateMoneyText.text = criticalRateUpGradePhaseFive.ToString();

            if (GameManager.instance.playerAllMoney < criticalRateUpGradePhaseFive)
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                criticalRateUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (criticalRateUpGradePhaseNum == 5)
        {
            criticalRateMoneyText.text = "0";
            criticalRateUpGradeButton.GetComponent<Button>().interactable = false;
        }
    }


    //クリティカル率のアップグレード
    public void CriticalRateUpGrade()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.UpGradeButton);

        if (criticalRateUpGradePhaseNum == 0)
        {
            PlayerPrefs.SetInt("CriticalRateUpGradePhase", 1);

            GameManager.instance.playerAllMoney -= criticalRateUpGradePhaseOne;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originCriticalRate", 0.13f);

        }
        else if (criticalRateUpGradePhaseNum == 1)
        {
            //SE
            //SoundManager.instance.PlaySE();

            PlayerPrefs.SetInt("CriticalRateUpGradePhase", 2);

            GameManager.instance.playerAllMoney -= criticalRateUpGradePhaseTwo;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originCriticalRate", 0.16f);
        }
        else if (criticalRateUpGradePhaseNum == 2)
        {

            PlayerPrefs.SetInt("CriticalRateUpGradePhase", 3);

            GameManager.instance.playerAllMoney -= criticalRateUpGradePhaseThree;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originCriticalRate", 0.19f);
        }
        else if (criticalRateUpGradePhaseNum == 3)
        {

            PlayerPrefs.SetInt("CriticalRateUpGradePhase", 4);

            GameManager.instance.playerAllMoney -= criticalRateUpGradePhaseFour;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originCriticalRate", 0.22f);
        }
        else if (criticalRateUpGradePhaseNum == 4)
        {

            PlayerPrefs.SetInt("CriticalRateUpGradePhase", 5);

            GameManager.instance.playerAllMoney -= criticalRateUpGradePhaseFive;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("originCriticalRate", 0.25f);
        }
    }



    //闘気アップグレードボタン管理
    private void EnergyGradeManagement()
    {
        energyUpGradePhaseNum = PlayerPrefs.GetInt("EnergyUpGradePhase", 0);

        energyUpGradePhaseText.text = energyUpGradePhaseNum + "/" + maxEnergyUpGradePhaseNum;

        //現在のアップグレード段階を判定
        if (energyUpGradePhaseNum == 0)
        {
            energyMoneyText.text = energyUpGradePhaseOne.ToString();

            if (GameManager.instance.playerAllMoney < energyUpGradePhaseOne)
            {
                energyUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                energyUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (energyUpGradePhaseNum == 1)
        {
            energyMoneyText.text = energyUpGradePhaseTwo.ToString();

            if (GameManager.instance.playerAllMoney < energyUpGradePhaseTwo)
            {
                energyUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                energyUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (energyUpGradePhaseNum == 2)
        {
            energyMoneyText.text = energyUpGradePhaseThree.ToString();

            if (GameManager.instance.playerAllMoney < energyUpGradePhaseThree)
            {
                energyUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                energyUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (energyUpGradePhaseNum == 3)
        {
            energyMoneyText.text = energyUpGradePhaseFour.ToString();

            if (GameManager.instance.playerAllMoney < energyUpGradePhaseFour)
            {
                energyUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                energyUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (energyUpGradePhaseNum == 4)
        {
            energyMoneyText.text = energyUpGradePhaseFive.ToString();

            if (GameManager.instance.playerAllMoney < energyUpGradePhaseFive)
            {
                energyUpGradeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                energyUpGradeButton.GetComponent<Button>().interactable = true;
            }

        }
        else if (energyUpGradePhaseNum == 5)
        {
            energyMoneyText.text = "0";
            energyUpGradeButton.GetComponent<Button>().interactable = false;
        }
    }


    //闘気アップグレード
    public void EnergyUpGrade()
    {
        //SE
        SoundManager.instance.PlaySE(SoundType.UpGradeButton);

        if (energyUpGradePhaseNum == 0)
        {

            PlayerPrefs.SetInt("EnergyUpGradePhase", 1);

            GameManager.instance.playerAllMoney -= energyUpGradePhaseOne;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("InitialEnergy", 7);

        }
        else if (energyUpGradePhaseNum == 1)
        {

            PlayerPrefs.SetInt("EnergyUpGradePhase", 2);

            GameManager.instance.playerAllMoney -= energyUpGradePhaseTwo;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("InitialEnergy", 9);
        }
        else if (energyUpGradePhaseNum == 2)
        {

            PlayerPrefs.SetInt("EnergyUpGradePhase", 3);

            GameManager.instance.playerAllMoney -= energyUpGradePhaseThree;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("InitialEnergy", 11);
        }
        else if (energyUpGradePhaseNum == 3)
        {

            PlayerPrefs.SetInt("EnergyUpGradePhase", 4);

            GameManager.instance.playerAllMoney -= energyUpGradePhaseFour;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("InitialEnergy", 13);
        }
        else if (energyUpGradePhaseNum == 4)
        {

            PlayerPrefs.SetInt("EnergyUpGradePhase", 5);

            GameManager.instance.playerAllMoney -= energyUpGradePhaseFive;

            PlayerPrefs.SetInt("PlayerGetMoney", GameManager.instance.playerAllMoney);

            PlayerPrefs.SetFloat("InitialEnergy", 15);
        }
    }

}
