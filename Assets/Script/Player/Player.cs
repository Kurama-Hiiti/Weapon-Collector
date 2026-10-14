using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class Player : MonoBehaviour
{
    public static Player instance;

    //初期値ステータス　攻撃力5、体力10
    [Tooltip("初期値")]
    public float originAttack, originHealth;

    //初期ステータス　回避率10%、クリティカル率10%
    [Tooltip("初期値")]
    public float originEvasionRate, originCriticalRate;

    //ブロック値
    public float blockNum;

    //装備している武器の総攻撃力,総体力
    [SerializeField]
    [Tooltip("装備されている武器の合計値")]
    private float weaponAttack, weaponHealth;


    //プレイヤーの総ステータス　攻撃力、体力
    [Tooltip("武器と初期値の合計値")]
    public float playerAttack, playerHealth;

    //プレイヤーの変動するHP
    [Tooltip("プレイヤーの現在のHP")]
    public float currentPlayerHealth;

    //プレイヤーの総ステータス　回避率、クリティカル率
    [Tooltip("バフ込みのステータス")]
    public float playerEvasionRate, playerCriticalRate;

    //スキルバフ
    public bool axBuff, sickleBuff, saberBuff, greatSwordBuff, rapierBuff, spearBuff, tonfaBuff, bowBuff, bowGunBuff, gunBuff, playerSkillBuff;

    //スキルバフターン管理
    public int saberBuffTurnNum = 0, greatSwordBuffTurnNum = 0, rapierBuffTurnNum = 0, shieldBuffTurnNum = 0;

    //アイテムバフ
    public bool attackPotionBuff, MantleBuff, holyAmuletBuff;


    //闘気の数
    [Tooltip("闘気の数")]
    public float specialEnergy;

    //闘気の最大数
    [SerializeField]
    private float maxSpecialEnergy = 50;

    //武器スロット
    [Tooltip("装備されている武器スロット")]
    public List<GameObject> Weapon;

    //アイテムスロット
    [Tooltip("装備されているアイテムスロット")]
    public List<GameObject> Item;


    //固有スキルスロット
    [SerializeField]
    [Tooltip("プレイヤーの闘気使用スキル")]
    private List<GameObject> playerSkillIcon;

    //プレイヤーのモーション管理
    private SPUM_Prefabs playerAnimation;

    //ターンが加算されたかの判定
    public bool turnAddPlayer;

    //初期武器の種類判定
    private bool originMainWeapon = false;
    private bool middleMainWeapon = false;
    private bool highMainWeapon = false;

    //武器の段階判定数
    private int weaponPhaseNum;

    //初期武器のスプライト
    [SerializeField]
    private GameObject mainWeapon1, mainWeapon2, mainWeapon3;

    //メイン武器の配置位置
    [SerializeField]
    private GameObject mainWeaponPos;

    //装備武器の配置位置
    [SerializeField]
    private List<GameObject> weaponPos;

    //アイテムの配置位置
    [SerializeField]
    private List <GameObject> itemPos;


    //HPバー
    [SerializeField]
    private GameObject playerHpBar;

    //HPバーの位置
    [SerializeField]
    private Transform plaeyrHpBarPos;

    //HPバーのイメージ
    [SerializeField]
    private Image playerHpBarImage;

    //プレイヤーが所持している武器アイコンの親オブジェクト
    public GameObject haveItemMass;

    //プレイヤーのバトル時の位置

    public Transform playerBattlePos;

    //プレイヤーのポップ位置
    [SerializeField]
    private Transform playerPopPos;

    //新しいWaveになった時のフラグ
    public bool isNewWave;

    //攻撃ボタン連打防止用数値
    private int attackButtonPushCount = 0;

    //次のWaveに移動するときの一度だけ実行したい関数の判定
    private bool isOneFunc;

    //バフによるステータス変更前のステータス格納変数
    private float beforeAttack, beforeEvasionRate, beforeCriticalRate;

    //特殊スキル武器入手フラグ
    public bool isBangle, isNecklace, isGlasses, isBoots, isFang, isRing, isSkillShield;

    //武器,アイテムスロットがいっぱいの時の変更フラグ
    public bool isWeaponChange, isItemChange;

    //プレイヤーの体力テキスト
    [SerializeField]
    private Text playerHealthText;

    //プレイヤーのブロック表記
    [SerializeField]
    private GameObject blockText;

    //ブロックテキスト表示フラグ
    private bool showBlockText = true;

    //闘気バー
    [SerializeField]
    private GameObject playerEnergyBar;

    //闘気バーの位置
    [SerializeField]
    private Transform plaeyrEnergyBarPos;

    //闘気バーのイメージ
    [SerializeField]
    private Image playerEnergyBarImage;

    //闘気テキスト
    [SerializeField]
    private Text playerEnergyText;

    //バフアイコンリスト
    [SerializeField]
    private GameObject[] buffIcons;

    //無敵バフアイコン
    [SerializeField]
    private GameObject invincibleIcon;

    //バフアイコンの位置
    [SerializeField]
    private GameObject[] buffIconPos;

    //プレイヤーのバフ状況によって表示するアイコンリスト
    [SerializeField]
    private List<GameObject> playerBuffIconList;

    //攻撃エフェクト
    [SerializeField]
    private GameObject attackEffect;

    //所持金(バトル中に獲得した分のみ)
    public int playerMoney;

    //バフ効果時エフェクト
    [SerializeField]
    private GameObject buffEffect;

    //バフエフェクト発生ポジション
    [SerializeField]
    private GameObject buffEffectPos;



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
    }


    void Start()
    {
        //プレイヤーのモーションの関数使用のためコンポーネント取得
        playerAnimation = GetComponent<SPUM_Prefabs>();
        turnAddPlayer = false;
        isOneFunc = false;

        //ステータスを読み取る
        originAttack = PlayerPrefs.GetFloat("originAttack", 5f);

        originHealth = PlayerPrefs.GetFloat("originHealth", 10f);

        originEvasionRate = PlayerPrefs.GetFloat("originEvasionRate", 0.1f);

        originCriticalRate = PlayerPrefs.GetFloat("originCriticalRate", 0.1f);

        specialEnergy = PlayerPrefs.GetFloat("InitialEnergy", 5.0f);


        weaponPhaseNum = PlayerPrefs.GetInt("WeaponPhase", 0);

        //メイン武器の表示
        MainWeaponDisplay();

        //プレイヤーのステータス反映
        PlayerStatas();
        currentPlayerHealth = playerHealth;

        //ゲーム中に獲得したお金をリセット
        playerMoney = 0;

    }

    void Update()
    {
        //HPバーの表示更新
        //[a, b]の範囲内で補間する値 value を生成する線形パラメータ t を計算する
        playerHpBarImage.fillAmount = Mathf.Lerp(playerHpBarImage.fillAmount,
            currentPlayerHealth / playerHealth, Time.deltaTime * 10f);

        //闘気バーの表示更新
        playerEnergyBarImage.fillAmount = Mathf.Lerp(playerEnergyBarImage.fillAmount,
            specialEnergy / maxSpecialEnergy, Time.deltaTime * 10f);

        //体力が0以下の時は0と表示
        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
        }

        //HPテキスト表示
        playerHealthText.text = currentPlayerHealth + "/" + playerHealth;

        //闘気テキスト表示
        playerEnergyText.text = "闘気 " + specialEnergy + "/" + maxSpecialEnergy;

        //無敵アイコン表示非表示関数
        ShowInvincibleBuff();

        if (showBlockText)
        {
            if (0 < blockNum)
            {
                blockText.SetActive(true);

                blockText.GetComponentInChildren<Text>().text = blockNum.ToString();
            }
            else
            {
                blockText.SetActive(false);
            }
        }


        if (isNewWave)
        {
            //プレイヤー移動
            transform.position += (new Vector3(1, 0, 0) * 5.0f) * Time.deltaTime;
            if (playerBattlePos.position.x < transform.position.x)
            {
                isNewWave = false;
                transform.position = playerBattlePos.position;
                //アニメーションをIdle状態に戻す
                playerAnimation.PlayAnimation(0);

                GameManager.instance.battleStart = true;
                isOneFunc = false;
                GameManager.instance.waveNum++;
                
            }

        }


        if (GameManager.instance.playerTurn)
        {
            //ターンに一度だけ実行される
            if (!turnAddPlayer)
            {
                PlayerStartSetMass();
            }

            //プレイヤーターン中にバフアイコン表示（更新）関数
            ShowBuffIcon();
        }

        if (GameManager.instance.nextWave)
        {
            //プレイヤー移動
            transform.position += (new Vector3(1, 0, 0) * 5.0f) * Time.deltaTime;

            if (isOneFunc)
            {
                return;
            }

            if (!isOneFunc)
            {
                //HPバーを消す
                HpBarHidden();

                //走るアニメーション
                playerAnimation.PlayAnimation(1);

                isOneFunc = true;
            }

        }

    }




    //プレイヤーのステータス更新関数(武器が更新された時)
    private void PlayerStatas()
    {

        //装備されている武器のステータスの値を足し合わせる
        for (int i = 0; i < Weapon.Count; i++)
        {
             WeaponManger myWeapon = Weapon[i].GetComponent<WeaponManger>();

            weaponAttack += myWeapon.attack;

            weaponHealth += myWeapon.health;

        }

        //メイン武器の性能によって加算する値を変更する
        if (originMainWeapon)
        {
            //武器による上昇前の値
            float nowPlayerHealth = playerHealth;

            playerAttack = Mathf.Round(originAttack + (weaponAttack * 0.5f));

            playerHealth = Mathf.Round(originHealth + (weaponHealth * 0.5f));

            //ネックレス持っていたら体力最大値1.3倍
            if (isNecklace)
            {
                playerHealth = Mathf.Round(playerHealth * 1.3f);
            }


            //上昇値分だけHP増える
            currentPlayerHealth += (playerHealth - nowPlayerHealth);

            //HP回復した時用
            if (playerHealth < currentPlayerHealth)
            {
                currentPlayerHealth = playerHealth;
            }

        }
        else if (middleMainWeapon)
        {
            float nowPlayerHealth = playerHealth;

            playerAttack = Mathf.Round(originAttack + (weaponAttack * 0.75f));

            playerHealth = Mathf.Round(originHealth + (weaponHealth * 0.75f));

            //ネックレス持っていたら体力最大値1.3倍
            if (isNecklace)
            {
                playerHealth = Mathf.Round(playerHealth * 1.3f);
            }

            currentPlayerHealth += (playerHealth - nowPlayerHealth);

            if (playerHealth < currentPlayerHealth)
            {
                currentPlayerHealth = playerHealth;
            }

        }
        else if (highMainWeapon)
        {
            float nowPlayerHealth = playerHealth;

            playerAttack = originAttack + weaponAttack;

            playerHealth = originHealth + weaponHealth;

            //ネックレス持っていたら体力最大値1.3倍
            if (isNecklace)
            {
                playerHealth = Mathf.Round(playerHealth * 1.3f);
            }

            currentPlayerHealth += (playerHealth - nowPlayerHealth);

            if (playerHealth < currentPlayerHealth)
            {
                currentPlayerHealth = playerHealth;
            }

        }

        weaponAttack = 0;
        weaponHealth = 0;

        //バングル持っていたら攻撃力1.2倍
        if (isBangle)
        {
            playerAttack = Mathf.Round(playerAttack * 1.2f);
        }




        //レンズ持っているときクリティカル率20%UP
        if (isGlasses)
        {
            playerCriticalRate = originCriticalRate + 0.2f;
        }
        else
        {
            playerCriticalRate = originCriticalRate;
        }

        //ブーツ持ってるとき回避率20%UP
        if (isBoots)
        {
            playerEvasionRate = originEvasionRate + 0.2f;
        }
        else
        {
            playerEvasionRate = originEvasionRate;
        }


        //バフ管理用ステータス
        beforeAttack = playerAttack;
        beforeEvasionRate = playerEvasionRate;
        beforeCriticalRate = playerCriticalRate;

    }


    //プレイヤーの攻撃関数
    public void PlayerAttack()
    {
        if (currentPlayerHealth <= 0)
        {
            return;
        }


        if (GameManager.instance.playerTurn)
        {
            attackButtonPushCount++;

            if (attackButtonPushCount == 1)
            {
                //ボタンを押した時のSE
                SoundManager.instance.PlaySE(SoundType.AttackButton);

                Invoke(nameof(PlayerAttackProcess), 0.5f);

                //二回攻撃
                if (tonfaBuff)
                {
                    Invoke(nameof(PlayerDoubleAttack), 1.0f);
                }
            }
            
        }

    }

    //プレイヤーの攻撃の関数の中身（ボタンを押してからディレイを付けるため）
    private void PlayerAttackProcess()
    {
        //ダメージ計算
        GameManager.instance.PlayerToEnemyDamage();

        //闘気加算
        if (specialEnergy < maxSpecialEnergy)
        {
            specialEnergy++;
        }
        

        //攻撃アニメーションを実行
        playerAnimation.PlayAnimation(4);

        //攻撃エフェクト
        Instantiate(attackEffect, EnemyManger.instance.enemyPos.transform.position, Quaternion.identity);

        if (!tonfaBuff)
        {
            //プレイヤーターン終了
            GameManager.instance.playerTurn = false;

            //エネミーターン開始
            GameManager.instance.enemyTurn = true;

            //エネミーのターン加算フラグ
            EnemyManger.instance.turnAddEnemy = false;

            //エネミーの攻撃フラグ
            EnemyManger.instance.enemyAttacked = false;

            //連打回数リセット
            attackButtonPushCount = 0;
        }

    }

    //2回攻撃用関数
    private void PlayerDoubleAttack()
    {
        //ダメージ計算
        GameManager.instance.PlayerToEnemyDamage();

        //闘気加算
        if (specialEnergy < maxSpecialEnergy)
        {
            specialEnergy++;
        }

        //攻撃アニメーションを実行
        playerAnimation.PlayAnimation(4);

        //攻撃エフェクト
        Instantiate(attackEffect, EnemyManger.instance.enemyPos.transform.position, Quaternion.identity);

        //プレイヤーターン終了
        GameManager.instance.playerTurn = false;

        //エネミーターン開始
        GameManager.instance.enemyTurn = true;

        //エネミーのターン加算フラグ
        EnemyManger.instance.turnAddEnemy = false;

        //エネミーの攻撃フラグ
        EnemyManger.instance.enemyAttacked = false;

        //連打回数リセット
        attackButtonPushCount = 0;

        tonfaBuff = false;
    }

    //メイン武器の種類を判定して表示する
    private void MainWeaponDisplay()
    {
        if (weaponPhaseNum == 0)
        {
            originMainWeapon = true;
            middleMainWeapon = false;
            highMainWeapon = false;
        }
        else if (weaponPhaseNum == 1)
        {
            originMainWeapon = false;
            middleMainWeapon = true;
            highMainWeapon = false;
        }
        else if (weaponPhaseNum == 2)
        {
            originMainWeapon = false;
            middleMainWeapon = false;
            highMainWeapon = true;
        }

        if (originMainWeapon)
        {
            //メイン武器の位置決め
            mainWeapon1.transform.position = mainWeaponPos.transform.position;
            //メイン武器をプレイヤーの武器リストに格納
            Weapon.Add(mainWeapon1);
            //武器を表示
            mainWeapon1.SetActive(true);
            
        }
        else if (middleMainWeapon)
        {
            mainWeapon2.transform.position = mainWeaponPos.transform.position;
            Weapon.Add(mainWeapon2);
            mainWeapon2.SetActive(true);

        }
        else if (highMainWeapon)
        {
            mainWeapon3.transform.position = mainWeaponPos.transform.position;
            Weapon.Add(mainWeapon3);
            mainWeapon3.SetActive(true);
        }
    }

    //HPバー表示関数
    public void HpBarDisplay()
    {
        //ここでテキストも表示
        playerHealthText.enabled = true;

        playerHpBar.transform.position = plaeyrHpBarPos.position;

        playerHpBarImage.fillAmount = currentPlayerHealth / playerHealth;

        playerHpBar.SetActive(true);

        //闘気バー,テキストの表示
        playerEnergyText.enabled = true;

        playerEnergyBar.transform.position = plaeyrEnergyBarPos.position;

        playerEnergyBarImage.fillAmount = specialEnergy / maxSpecialEnergy;

        playerEnergyBar.SetActive(true);

        //ブロックテキスト表示フラグ
        showBlockText = true;
    }

    //HPバー非表示関数
    private void HpBarHidden()
    {
        //ここでテキストも非表示
        playerHealthText.enabled = false;
        playerHpBar.SetActive(false);

        //闘気バー,テキストの非表示
        playerEnergyText.enabled = false;
        playerEnergyBar.SetActive(false);

        //ブロックテキスト非表示
        blockText.SetActive(false);
        showBlockText = false;

        //複数ターン有効のバフがある場合はバフアイコンも非表示にする
        for (int i = 0; i < playerBuffIconList.Count; i++)
        {
            playerBuffIconList[i].SetActive(false);
        }

    }

    //入手した武器をスロットに入れる関数
    public void AddWeapon(GameObject addWeapon)
    {
        //メイン武器を考慮して武器を入手できるかの判定
        if (Weapon.Count < weaponPos.Count + 1)
        {
            //リストに追加
            Weapon.Add(addWeapon);

            //追加した武器の親オブジェクトを指定
            addWeapon.transform.parent = haveItemMass.transform;


            //スロットに配置
            for (int i = 0; i < Weapon.Count - 1; i++)
            {
                Weapon[i + 1].transform.position = weaponPos[i].transform.position;

                Weapon[i + 1].SetActive(true);
            }

            //プレイヤーのステータス更新
            Invoke(nameof(PlayerStatas), 0.2f);

            //次のシーンへの遷移フラグ
            GameManager.instance.nextWave = true;
        }
        else
        {
            //武器スロットいっぱいの時は入れ替え処理の後　次のシーンへの遷移フラグ
            isWeaponChange = true;
            GameManager.instance.changeWeapon = addWeapon;
        }

    }

    //入手したアイテムをスロットに入れる関数
    public void AddItem(GameObject addItem)
    {
        if (Item.Count < itemPos.Count)
        {
            //リストに追加
            Item.Add(addItem);

            //追加したアイテムの親オブジェクトを指定
            addItem.transform.parent = haveItemMass.transform;

            for (int i = 0; i < Item.Count; i++) 
            {
                Item[i].transform.position = itemPos[i].transform.position;

                Item[i].SetActive(true);
            }

            //次のシーンへの遷移フラグ
            GameManager.instance.nextWave = true;

        }
        else
        {
            //アイテムスロットいっぱいの時は入れ替え処理の後　次のシーンへの遷移フラグ
            isItemChange = true;
            GameManager.instance.changeItem = addItem;
        }
        


    }

    //初期アイテムゲット
    public void FirstItemGet(GameObject addItem)
    {
        if (Item.Count < itemPos.Count)
        {
            //リストに追加
            Item.Add(addItem);

            //追加したアイテムの親オブジェクトを指定
            addItem.transform.parent = haveItemMass.transform;

            for (int i = 0; i < Item.Count; i++)
            {
                Item[i].transform.position = itemPos[i].transform.position;

                Item[i].SetActive(true);
            }

        }
    }

    //アイテムを使用したときにスロットの位置更新
    public void ItemSort()
    {
        for (int i = 0; i < Item.Count; i++)
        {
            Item[i].transform.position = itemPos[i].transform.position;

            Item[i].SetActive(true);
        }
    }

    //プレイヤーの敗北関数
    public void PlyaerDeath()
    {
        //死亡アニメーションを実行
        playerAnimation.PlayAnimation(2);
    }

    //プレイヤーの攻撃アニメーション関数
    public void PlayerAttackAnimation()
    {
        //攻撃アニメーションを実行
        playerAnimation.PlayAnimation(4);

        //攻撃エフェクト生成
        Instantiate(attackEffect, EnemyManger.instance.enemyPos.transform.position, Quaternion.identity);
    }



    //プレイヤーのバフによるステータス変化関数
    public void StatusBuff(float attackBuffRate, float evasionBuffRate, float criticalBuffRate)
    {
        //攻撃力バフは割合
        playerAttack += playerAttack * attackBuffRate;

        //回避率、クリティカル率は加算
        playerEvasionRate += evasionBuffRate;
        playerCriticalRate += criticalBuffRate;

    }

    //バフエフェクト表示関数
    public void ShowBuffEffect()
    {
        //バフエフェクト
        Instantiate(buffEffect, buffEffectPos.transform.position, Quaternion.identity);

        SoundManager.instance.PlaySE(SoundType.Buff);
    }

    //バフのステータスリセット
    private void BuffStatusReset(ref bool buffFlag)
    {
        if (buffFlag)
        {
            playerAttack = beforeAttack;
            playerEvasionRate = beforeEvasionRate;
            playerCriticalRate = beforeCriticalRate;
            buffFlag = false;
        }

    }

    //刀と細剣のバフ管理
    private void SaberAndRapierBuffProcess()
    {
        if (saberBuff && rapierBuff)
        {
            saberBuffTurnNum--;
            rapierBuffTurnNum--;

            if (0 < saberBuffTurnNum && 0 < rapierBuffTurnNum)
            {
                BuffStatusReset(ref saberBuff);
                BuffStatusReset(ref rapierBuff);
                StatusBuff(0, 0.3f, 0.3f);

                saberBuff = true;
                rapierBuff = true;
            }
            else if (saberBuffTurnNum <= 0 && 0 < rapierBuffTurnNum)
            {
                BuffStatusReset(ref saberBuff);
                BuffStatusReset(ref rapierBuff);
                StatusBuff(0, 0.3f, 0);
                rapierBuff = true;
            }
            else if (0 < saberBuffTurnNum && rapierBuffTurnNum <= 0)
            {
                BuffStatusReset(ref saberBuff);
                BuffStatusReset(ref rapierBuff);
                StatusBuff(0, 0, 0.3f);
                saberBuff = true;
            }
            else
            {
                BuffStatusReset(ref saberBuff);
                BuffStatusReset(ref rapierBuff);
            }


        }
        else if (saberBuff && !rapierBuff)
        {
            if (saberBuff)
            {
                saberBuffTurnNum--;
                if (0 < saberBuffTurnNum)
                {
                    BuffStatusReset(ref saberBuff);
                    StatusBuff(0, 0, 0.3f);
                    saberBuff = true;
                }
                else
                {
                    BuffStatusReset(ref saberBuff);
                }
            }
        }
        else if (!saberBuff && rapierBuff)
        {
            if (rapierBuff)
            {
                rapierBuffTurnNum--;
                if (0 < rapierBuffTurnNum)
                {
                    BuffStatusReset(ref rapierBuff);
                    StatusBuff(0, 0.3f, 0);
                    rapierBuff = true;
                }
                else
                {
                    BuffStatusReset(ref rapierBuff);
                }
            }
        }


    }

    //大剣のブロック付与効果
    private void GreatSwordBuffProcess()
    {
        if (greatSwordBuffTurnNum <= 0) return;

        greatSwordBuffTurnNum--;
        if (0 < greatSwordBuffTurnNum)
        {
            if (isSkillShield)
            {
                blockNum += Mathf.Round(5 * 1.5f);
                return;
            }

            blockNum += 5;
        }
    }

    //木盾のブロック付与効果
    private void ShieldBuffProcess()
    {
        if (shieldBuffTurnNum <= 0) return;

        shieldBuffTurnNum--;
        if (0 < shieldBuffTurnNum)
        {
            if (isSkillShield)
            {
                blockNum += Mathf.Round(8 * 1.5f);
                return;
            }

            blockNum += 8;
        }
    }

    //プレイヤーの吸血の牙の回復関数
    public void FangAbsorption(float damage)
    {
        if (isFang)
        {
            float absorptionNum = Mathf.Round(damage * 0.1f);

            currentPlayerHealth += absorptionNum;

            GameManager.instance.PlayerHealText(absorptionNum);


            if (playerHealth < currentPlayerHealth)
            {
                currentPlayerHealth = playerHealth;
            }
        }
    }

    //守護の盾によるブロックバフ
    private void SkillShieldBuff()
    {
        if (isSkillShield)
        {
            blockNum += 5;
        }
    }

    //バフアイコン表示関数
    private void ShowBuffIcon()
    {
        //攻撃力UP判定
        if (axBuff || sickleBuff || greatSwordBuff || spearBuff || attackPotionBuff || playerSkillBuff)
        {
            for (int i = 0; i < buffIcons.Length; i++)
            {
                if (buffIcons[i].name == "AttackUP")
                {
                    //攻撃力アップバフのアイコンが表示用リストに格納されていなければ格納する
                    if (!playerBuffIconList.Any(obj => obj.name == buffIcons[i].name))
                    {
                        playerBuffIconList.Add(buffIcons[i]);
                    }
                    
                }
            }
        }

        //回避率UP判定
        if (rapierBuff || spearBuff || bowBuff || bowGunBuff || gunBuff || MantleBuff)
        {
            for (int i = 0; i < buffIcons.Length; i++)
            {
                if (buffIcons[i].name == "EvasionRateUP")
                {
                    //攻撃力アップバフのアイコンが表示用リストに格納されていなければ格納する
                    if (!playerBuffIconList.Any(obj => obj.name == buffIcons[i].name))
                    {
                        playerBuffIconList.Add(buffIcons[i]);
                    }

                }
            }
        }

        //クリティカル率UP判定
        if (saberBuff || bowBuff || bowGunBuff || gunBuff)
        {
            for (int i = 0; i < buffIcons.Length; i++)
            {
                if (buffIcons[i].name == "CriticalRateUP")
                {
                    //攻撃力アップバフのアイコンが表示用リストに格納されていなければ格納する
                    if (!playerBuffIconList.Any(obj => obj.name == buffIcons[i].name))
                    {
                        playerBuffIconList.Add(buffIcons[i]);
                    }

                }
            }
        }

        //追撃判定
        if (GameManager.instance.daggerBuff || GameManager.instance.chakramBuff)
        {
            for (int i = 0; i < buffIcons.Length; i++)
            {
                if (buffIcons[i].name == "ExtraAttack")
                {
                    //攻撃力アップバフのアイコンが表示用リストに格納されていなければ格納する
                    if (!playerBuffIconList.Any(obj => obj.name == buffIcons[i].name))
                    {
                        playerBuffIconList.Add(buffIcons[i]);
                    }

                }
            }
        }

        //二回攻撃判定
        if (tonfaBuff)
        {
            for (int i = 0; i < buffIcons.Length; i++)
            {
                if (buffIcons[i].name == "DoubleAttack")
                {
                    //攻撃力アップバフのアイコンが表示用リストに格納されていなければ格納する
                    if (!playerBuffIconList.Any(obj => obj.name == buffIcons[i].name))
                    {
                        playerBuffIconList.Add(buffIcons[i]);
                    }

                }
            }
        }

        //バフアイコンを非表示（リストから除外する）
        RemoveBuffIconList();


        //バフのアイコン表示
        for (int i = 0; i < playerBuffIconList.Count; i++)
        {
            playerBuffIconList[i].transform.position = buffIconPos[i].transform.position;

            playerBuffIconList[i].SetActive(true);
        }

    }

    //バフアイコンリストから除外関数
    private void RemoveBuffIconList()
    {
        //攻撃UPバフアイコンリストからアイコン除外プラス非表示
        if (!axBuff && !sickleBuff && !greatSwordBuff && !spearBuff && !attackPotionBuff && !playerSkillBuff)
        {
            for (int i = 0; i < playerBuffIconList.Count; i++)
            {
                if (playerBuffIconList[i].name == "AttackUP")
                {
                    playerBuffIconList[i].SetActive(false);

                    playerBuffIconList.Remove(playerBuffIconList[i]);

                }
            }
        }


        //回避率UP判定
        if (!rapierBuff && !spearBuff && !bowBuff && !bowGunBuff && !gunBuff && !MantleBuff)
        {

            for (int i = 0; i < playerBuffIconList.Count; i++)
            {
                if (playerBuffIconList[i].name == "EvasionRateUP")
                {
                    playerBuffIconList[i].SetActive(false);

                    playerBuffIconList.Remove(playerBuffIconList[i]);

                }
            }
        }

        //クリティカル率UP判定
        if (!saberBuff && !bowBuff && !bowGunBuff && !gunBuff)
        {

            for (int i = 0; i < playerBuffIconList.Count; i++)
            {
                if (playerBuffIconList[i].name == "CriticalRateUP")
                {
                    playerBuffIconList[i].SetActive(false);

                    playerBuffIconList.Remove(playerBuffIconList[i]);

                }
            }
        }

        //追撃判定
        if (!GameManager.instance.daggerBuff && !GameManager.instance.chakramBuff)
        {

            for (int i = 0; i < playerBuffIconList.Count; i++)
            {
                if (playerBuffIconList[i].name == "ExtraAttack")
                {
                    playerBuffIconList[i].SetActive(false);

                    playerBuffIconList.Remove(playerBuffIconList[i]);

                }
            }
        }

        //二回攻撃判定
        if (!tonfaBuff)
        {
            for (int i = 0; i < playerBuffIconList.Count; i++)
            {
                if (playerBuffIconList[i].name == "DoubleAttack")
                {
                    playerBuffIconList[i].SetActive(false);

                    playerBuffIconList.Remove(playerBuffIconList[i]);

                }
            }
        }

    }

    //無敵アイコン表示
    private void ShowInvincibleBuff()
    {
        if (holyAmuletBuff)
        {
            invincibleIcon.SetActive(true);
        }
        else 
        {
            invincibleIcon.SetActive(false);
        }
    }


    //プレイヤーのターン開始時に実行される関数まとめ
    private void PlayerStartSetMass()
    {
        GameManager.instance.playerTurnNum++;

        //斧によるバフリセット
        BuffStatusReset(ref axBuff);
        //鎌によるバフリセット
        BuffStatusReset(ref sickleBuff);
        //大剣による攻撃力バフリセット
        BuffStatusReset(ref greatSwordBuff);
        //槍による攻撃力バフリセット
        BuffStatusReset(ref spearBuff);
        //弓によるバフリセット
        BuffStatusReset(ref bowBuff);
        //ボウガンによるバフリセット
        BuffStatusReset(ref bowGunBuff);
        //銃によるバフリセット
        BuffStatusReset(ref gunBuff);
        //力のポーションによるバフリセット
        BuffStatusReset(ref attackPotionBuff);
        //幻影のマントによるバフリセット
        BuffStatusReset(ref MantleBuff);
        //プレイヤースキルによる攻撃力バフリセット
        BuffStatusReset(ref playerSkillBuff);


        //大剣のブロック付与判定
        GreatSwordBuffProcess();

        //木盾のブロック付与判定
        ShieldBuffProcess();

        //守護の盾のブロック判定
        SkillShieldBuff();

        //刀と細剣のバフ継続判定
        SaberAndRapierBuffProcess();

        //敵のデバフリセット関数
        EnemyManger.instance.DebuffReset(ref GameManager.instance.swordDebuff);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.axDebuff);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.sickleDebuff);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.whipDebuff);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.bowDebuff);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.bowGunDebuff);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.gunDebuff);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.attackDownAmulet);
        EnemyManger.instance.DebuffReset(ref GameManager.instance.defenseDownAmulet);

        turnAddPlayer = true;
    }


    //プレイヤーのステータス更新
    public void PlayerStatusUpdate()
    {
        //プレイヤーのステータス更新
        Invoke(nameof(PlayerStatas), 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //次のWaveへ行く際にプレイヤーが端へ到達した時に位置を変更する
        if (collision.gameObject.CompareTag("Next"))
        {
            transform.position = playerPopPos.position;

            GameManager.instance.nextWave = false;
        }
    }

}
