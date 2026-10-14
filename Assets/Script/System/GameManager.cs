using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static SoundManager;

public class GameManager : MonoBehaviour
{
    //シングルトン
    public static GameManager instance;

    //ターン管理
    public bool playerTurn, enemyTurn;

    //それぞれのターン数
    public int playerTurnNum, enemyTurnNum;

    //Wave数
    public int waveNum;

    //最大Wave数
    private int maxWaveNum;

    //難易度管理
    public bool nomal, hard, veryHard;

    //難易度による敵のステータス変動率 nomal = 1.0 , hard = 1.2, veryhard = 1.5
    public float diffMultiplier;

    //ステータス変動率
    private readonly float nomalDiffMultiplier = 1.0f;
    private readonly float hardDiffMultiplier = 1.2f;
    private readonly float veryHardDiffMultiplier = 1.5f;


    //戦闘開始、終了フラグ
    public bool battleStart, battleEnd;

    //クリティカル倍率（1.5倍）
    private readonly float criticalMag = 1.5f;

    //防御係数
    private readonly float defenseCoeff = 100;

    //ドロップ率
    [SerializeField]
    private float dropRate = 0.7f;

    //ドロップ武器リスト
    [SerializeField]
    private List<GameObject> dropWeaponList;

    //ドロップ特殊スキルアイテムリスト
    [SerializeField]
    private List<GameObject> dropSkillItemList;

    //ドロップアイテムリスト
    [SerializeField]
    private List<GameObject> dropItemList;

    //ドロップアイテムのカードの位置
    [SerializeField]
    private Transform cardPos;

    //アイテム入手ボタン用パネル
    [SerializeField]
    private GameObject itemGetPanel;

    //ドロップしたかの判定
    private bool isDrop;

    //ドロップしたアイテムを格納
    private GameObject dropItem;

    //アイテムアイコンリスト
    [SerializeField]
    private List<GameObject> iconList;

    //プレイヤーが獲得した武器のカードを格納する（武器のスキル使用時やアイテム使用時に使用する）
    public List<GameObject> playerGotWeaponList;

    //プレイヤーが獲得したアイテムのカードを格納する（武器のスキル使用時やアイテム使用時に使用する）
    public List<GameObject> playerGotItemList;

    //次のWaveへの遷移フラグ
    public bool nextWave;

    //UIの関数を一度だけ呼ぶためのフラグ
    private bool isUIFunction;

    //死亡アニメーションを一度だけ実行のためのフラグ
    private bool isPlaerDeath;


    //スキル使用のためのボタン配置パネル
    [SerializeField]
    private GameObject skillUsePanel;

    //戻るためのボタン配置パネル
    [SerializeField]
    private GameObject backPanel;

    //アイテム使用のボタン配置パネル
    [SerializeField]
    private GameObject itemUsePanel;

    //スキル使用時に表示されているアイコンカードを格納する
    private GameObject viewCard;

    //クリックしたときにアイコンについているスクリプトを格納する　武器用
    private WeaponManager weaponSkill;

    //クリックしたときにアイコンについているスクリプトを格納　アイテム用
    private ItemManager itemEffect;

    //武器によるデバフ管理
    public bool swordDebuff, axDebuff, sickleDebuff, whipDebuff, bowDebuff, bowGunDebuff, gunDebuff;

    //アイテムによるデバフ管理
    public bool attackDownAmulet, defenseDownAmulet;

    //短剣、チャクラムスキルによる追加ダメージフラグ
    public bool daggerBuff, chakramBuff;

    //追撃倍率
    private readonly float daggerRate = 0.2f, chakramRate = 0.3f, ringRate = 0.1f;

    //初期アイテムゲットフラグ
    private bool isFirstItemGet;

    //入れ替える武器,アイテムアイコン
    private GameObject selectWeapon, selectItem;

    //武器,アイテム入れ替え用パネル
    [SerializeField]
    private GameObject weaponChangePanel, itemChangePanel;

    //入れ替え元の武器,アイテムオブジェクト
    public GameObject changeWeapon, changeItem;

    //武器入れ替えテキスト
    [SerializeField]
    private GameObject weaponChangeText, itemChangeText;

    //武器入れ替えテキスト表示フラグ
    private bool isWeaponChangeTextDisplay, isItemChangeTextDisplay;

    //Waveテキスト
    [SerializeField]
    private Text waveText;

    //敵、プレイヤーのダメージテキスト、プレイヤーの回復テキスト
    [SerializeField]
    private GameObject enemyDamageTextCanvas, playerDamageTextCanvas, playerHealTextCanvas;

    //メイン武器の説明オブジェクト
    [SerializeField]
    private GameObject mainWeapon1Detail, mainWeapon2Detail, mainWeapon3Detail;

    //プレイヤースキル使用確認ボタン表示パネル
    [SerializeField]
    private GameObject playerSkillPanel;

    //プレイヤースキル詳細画像
    [SerializeField]
    private GameObject playerSkill1, playerSkill2, playerSkill3, playerSkill4;

    //プレイヤースキル判別のための数字
    private int playerSkillNum;

    //ゲームクリアUI、ゲームオーバーUI
    [SerializeField]
    private GameObject gameClearPanel, gameOverPanel;

    //ドロップしたお金
    public int dropMoney;

    //総所持金
    public int playerAllMoney;

    //獲得したお金テキスト
    [SerializeField]
    private Text moneyText;


    //ステータスボード
    [SerializeField]
    private GameObject statusBoad;

    //ステータステキスト　攻撃力、回避率、クリティカル率
    [SerializeField]
    private Text attackStatusText, evasionRateStatusText, criticalRateStatusText;

    //ゲーム終了時お金加算フラグ
    private bool isGetMoney;

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
        //初期化
        playerTurn = false;
        enemyTurn = false;
        battleEnd = false;
        isDrop = false;
        nextWave = false;
        isPlaerDeath = false;
        isGetMoney = false;
        playerTurnNum = 0;
        enemyTurnNum = 0;
        waveNum = 1;

        //所持金取得
        playerAllMoney = PlayerPrefs.GetInt("PlayerGetMoney", 0);

        //難易度別の設定定義処理
        if (UIManager.instance.sceneName == "GameMainNomal")
        {
            battleStart = true;
            maxWaveNum = 30;
            Nomal();
        }
        else if (UIManager.instance.sceneName == "GameMainHard")
        {
            battleStart = true;
            maxWaveNum = 40;
            Hard();
        }
        else if (UIManager.instance.sceneName == "GameMainVeryHard")
        {
            battleStart = true;
            maxWaveNum = 50;
            VeryHard();
        }

    }

    void Update()
    {
        if (UIManager.instance.sceneName == "GameMainNomal" || UIManager.instance.sceneName == "GameMainHard" || UIManager.instance.sceneName == "GameMainVeryHard")
        {
            //Wave数テキスト表示
            if (waveText != null)
            {
                waveText.text = "Wave " + waveNum + " / " + maxWaveNum;
            }


            //プレイヤーのターンの時のみクリック判定あり
            if (playerTurn)
            {
                //武器、アイテム、スキルのクリック処理
                ItemClick();

                //メイン武器とプレイヤーへカーソルを合わせた際に詳細表示処理
                ShowMainWeaponDetailAndStatus();

                //獲得したお金のテキスト表記
                moneyText.text = ":" + Player.instance.playerMoney;
            }

            //武器入れ替え処理
            if (Player.instance.isWeaponChange)
            {
                if (!isWeaponChangeTextDisplay)
                {
                    //武器入れ替え用テキスト表示する
                    weaponChangeText.SetActive(true);
                    isWeaponChangeTextDisplay = true;
                }

                //武器変更のための変更したい武器の選択処理
                WeaponChange();
            }

            //アイテム入れ替え処理
            if (Player.instance.isItemChange)
            {
                if (!isItemChangeTextDisplay)
                {
                    //アイテム入れ替え用テキスト表示
                    itemChangeText.SetActive(true);
                    isItemChangeTextDisplay = true;
                }

                //アイテム変更のための変更したいアイテムの選択処理
                ItemChange();

            }


            //ゲームメインへ遷移時バトルスタートフラグが立ち、プレイヤーのターンが開始される
            if (battleStart)
            {
                battleEnd = false;
                battleStart = false;
                isDrop = false;
                playerTurn = true;
                Player.instance.HpBarDisplay();
                isUIFunction = false;

                if (!isFirstItemGet)
                {
                    //初期装備取得処理
                    FirstItem();
                    isFirstItemGet = true;
                }

            }

            //プレイヤー敗北時
            if (Player.instance.currentPlayerHealth <= 0)
            {
                if (!isPlaerDeath)
                {
                    //プレイヤーの死亡アニメーション実行処理
                    Player.instance.PlyaerDeath();
                    playerTurn = false;
                    isPlaerDeath = true;

                    SoundManager.instance.GameOverBGM();

                    //ゲームオーバーUIを表示
                    Invoke(nameof(ShowGameOverPanel), 0.5f);

                    //取得したお金の追加処理
                    GetMoney();

                    //ステータスボード非表示
                    HiddenStatusBoad();

                    //メイン武器詳細非表示
                    mainWeapon1Detail.SetActive(false);
                    mainWeapon2Detail.SetActive(false);
                    mainWeapon3Detail.SetActive(false);

                }

            }

            //勝利（エネミーを倒したとき）
            if (EnemyManager.instance.currentEnemyHealth <= 0)
            {
                EnemyManager.instance.EnemyDelete();
                playerTurn = false;
                enemyTurn = false;
                battleEnd = true;
                //プレイヤーターン加算フラグ
                Player.instance.turnAddPlayer = false;
                EnemyManager.instance.isStun = false;

                //ステータスボード非表示
                HiddenStatusBoad();

                //メイン武器詳細非表示
                mainWeapon1Detail.SetActive(false);
                mainWeapon2Detail.SetActive(false);
                mainWeapon3Detail.SetActive(false);

            }


            //戦闘終了時
            if (battleEnd)
            {
                Player.instance.holyAmuletBuff = false;

                //ラストWave時
                if (waveNum == maxWaveNum)
                {
                    Invoke(nameof(ShowGameClearPanel), 0.5f);

                    if (!isGetMoney)
                    {

                        SoundManager.instance.GameClearBGM();

                        //所持金加算
                        Player.instance.playerMoney += dropMoney;

                        //獲得したお金のテキスト表記
                        moneyText.text = ":" + Player.instance.playerMoney;

                        GetMoney();

                        isGetMoney = true;

                        if (UIManager.instance.sceneName == "GameMainNomal")
                        {
                            PlayerPrefs.SetInt("PlayHard", 1);
                        }
                        else if (UIManager.instance.sceneName == "GameMainHard")
                        {
                            PlayerPrefs.SetInt("PlayVeryHard",1);
                        }


                    }


                }
                else
                {
                    if (!isDrop)
                    {
                        //所持金加算
                        Player.instance.playerMoney += dropMoney;

                        //獲得したお金のテキスト表記
                        moneyText.text = ":" + Player.instance.playerMoney;

                        Invoke(nameof(ItemDrop), 0.5f);
                        isDrop = true;
                    }

                    if (nextWave)
                    {
                        if (!isUIFunction)
                        {
                            //フェードアウト関数呼ぶ
                            UIManager.instance.NextWave();
                            isUIFunction = true;
                        }

                    }
                }



            }



        }

    }

    //ゲームクリアパネル表示
    private void ShowGameClearPanel()
    {
        gameClearPanel.SetActive(true);
    }

    //ゲームオーバーパネル表示
    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        //SE
        SoundManager.instance.PlaySE(SoundType.GameOver);
    }

    //プレイヤーからエネミーへのダメージ処理関数
    public void PlayerToEnemyDamage()
    {
        //ヒット確認
        float hitCheck = Random.Range(0.0f, 1.0f);

        if (EnemyManager.instance.enemyEvasionRate < hitCheck)
        {
            //クリティカル確認
            float criticalCheck = Random.Range(0.0f, 1.0f);

            if (criticalCheck < Player.instance.playerCriticalRate)
            {
                //クリティカル(1.5倍)ダメ―ジ計算　→　敵のHPからプレイヤー攻撃力分減らす（敵の防御力の補正をかける）
                //防御率
                float defenseRate = 1 - (EnemyManager.instance.enemyDefense / (EnemyManager.instance.enemyDefense + defenseCoeff));
                //ダメージ計算
                float damage = Mathf.Round((Player.instance.playerAttack * criticalMag) * defenseRate);

                EnemyManager.instance.currentEnemyHealth -= damage;

                //ダメージテキスト
                PlayerToEnemyCriticalDamageText(damage);

                //吸血の牙での回復
                Player.instance.FangAbsorption(damage);

                //追撃ダメージ処理
                ExtraDamage(damage, daggerRate, ref daggerBuff);
                ExtraDamage(damage, chakramRate, ref chakramBuff);
                ExtraDamage(damage, ringRate, ref Player.instance.isRing);

                SoundManager.instance.PlaySE(SoundType.CriticalHit);


            }
            else
            {
                //ダメ―ジ計算　→　敵のHPからプレイヤー攻撃力分減らす（敵の防御力の補正をかける）
                //防御率
                float defenseRate = 1 - (EnemyManager.instance.enemyDefense / (EnemyManager.instance.enemyDefense + defenseCoeff));
                //ダメージ計算
                float damage = Mathf.Round(Player.instance.playerAttack * defenseRate);

                EnemyManager.instance.currentEnemyHealth -= damage;

                //ダメージテキスト
                PlayerToEnemyDamageText(damage);

                //吸血の牙での回復
                Player.instance.FangAbsorption(damage);

                //追撃ダメージ処理
                ExtraDamage(damage, daggerRate, ref daggerBuff);
                ExtraDamage(damage, chakramRate, ref chakramBuff);
                ExtraDamage(damage, ringRate, ref Player.instance.isRing);

                SoundManager.instance.PlaySE(SoundType.Attack);


            }

        }
        else 
        {
            SoundManager.instance.PlaySE(SoundType.Miss);

            //ミステキスト
            PlayerMissText();

            daggerBuff = false;
            chakramBuff = false;
        }



    }


    //プレイヤーからの敵へのダメージ関数（スキルダメージ用）
    public void PlayerToEnemySkillDamage(float attackRate)
    {
        //ダメ―ジ計算　→　敵のHPからプレイヤー攻撃力分減らす（敵の防御力の補正をかける）
        //防御率
        float defenseRate = 1 - (EnemyManager.instance.enemyDefense / (EnemyManager.instance.enemyDefense + defenseCoeff));
        //ダメージ計算
        float damage = Mathf.Round((Player.instance.playerAttack * attackRate) * defenseRate);

        EnemyManager.instance.currentEnemyHealth -= damage;

        PlayerToEnemyDamageText(damage);

        SoundManager.instance.PlaySE(SoundType.Attack);
    }

    //追撃ダメージ用関数(元のダメージ、追撃割合、スキルのバフフラグ)
    public void ExtraDamage(float damage, float extraDamageRate, ref bool skillFlag)
    {
        if (skillFlag)
        {
            float extraDamage = Mathf.Round(damage * extraDamageRate);

            EnemyManager.instance.currentEnemyHealth -= extraDamage;

            //吸血の牙での回復（追撃にも適応）
            Player.instance.FangAbsorption(extraDamage);

            PlayerToEnemyExtraDamageText(extraDamage, skillFlag);

            //衝撃の指輪の効果の場合はフラグをfalseにしない
            if (skillFlag == Player.instance.isRing)
            {
                return;
            }

            skillFlag = false;
            
        }
    }

    //エネミーからプレイヤーへのダメージ処理関数
    public void EnemyToPlayerDamage()
    {
        //プレイヤーの無敵処理
        if (Player.instance.holyAmuletBuff)
        {
            //無効化テキスト
            EnmeyMissText("Dodge");

            Player.instance.holyAmuletBuff = false;

            SoundManager.instance.PlaySE(SoundType.Miss);

            return;
        }

        //ヒット確認
        float hitCheck = Random.Range(0.0f, 1.0f);

        if (Player.instance.playerEvasionRate < hitCheck)
        {
            //クリティカル確認
            float criticalCheck = Random.Range(0.0f, 1.0f);

            if (criticalCheck < EnemyManager.instance.enemyCriticalRate)
            {
                //クリティカル(1.5倍)ダメ―ジ計算
                //ダメージ計算
                float damage = Mathf.Round(EnemyManager.instance.enemyAttack * criticalMag);


                //ダメージテキスト
                EnemyToPlayerCriticalDamageText(damage);

                //SE
                SoundManager.instance.PlaySE(SoundType.CriticalTakeDamage);

                //プレイヤーにブロックがあるかの判定
                if (Player.instance.blockNum > 0)
                {
                    //ブロック値がある時はブロック値からダメージ計算
                    Player.instance.blockNum -= damage;

                   
                    if (Player.instance.blockNum < 0)
                    {
                        //ブロックの値のUIを消す
                        //ここに来る時はblockDamageが負の数になっているのでプレイヤーのHPにダメージを足し合わせる
                        Player.instance.currentPlayerHealth += Player.instance.blockNum;
                        Player.instance.blockNum = 0;

                    }

                }
                else
                {
                    Player.instance.currentPlayerHealth -= damage;
                }


            }
            else
            {
                //ダメージ計算(クリティカルなし)
                float damage = Mathf.Round(EnemyManager.instance.enemyAttack);

                //ダメージテキスト
                EnemyToPlayerDamageText(damage);

                //SE
                SoundManager.instance.PlaySE(SoundType.Damage);

                //プレイヤーにブロックがあるかの判定
                if (Player.instance.blockNum > 0)
                {
                    //ブロック値がある時はブロック値からダメージ計算
                    Player.instance.blockNum -= damage;
                    
                    if (Player.instance.blockNum < 0)
                    {
                        //ブロックの値のUIを消す
                        //ここに来る時はblockDamageが負の数になっているのでプレイヤーのHPにダメージを足し合わせる
                        Player.instance.currentPlayerHealth += Player.instance.blockNum;
                        Player.instance.blockNum = 0;

                    }


                }
                else
                {
                    Player.instance.currentPlayerHealth -= damage;

                }


            }


        }
        else
        {

            //ミステキスト
            EnmeyMissText("Miss");

            SoundManager.instance.PlaySE(SoundType.Miss);

        }
    }

    //アイテムのドロップ関数
    private void ItemDrop()
    {

        //アイテムドロップ
        float itemDropRate = Random.Range(0.0f, 1.0f);

        if (itemDropRate <= dropRate) //70%
        {
            //武器ドロップ
            int dropWeaponNum = Random.Range(0, dropWeaponList.Count);
            dropItem = Instantiate(dropWeaponList[dropWeaponNum], cardPos.position, Quaternion.identity);

            dropItem.name = dropWeaponList[dropWeaponNum].name;

        }
        else
        {
            float skillDropRate = Random.Range(0.0f, 1.0f);
            if (dropRate < skillDropRate) //30%
            {
                //スキルアイテムドロップ
                int dropSkillNum = Random.Range(0, dropSkillItemList.Count);

                dropItem = Instantiate(dropSkillItemList[dropSkillNum], cardPos.position, Quaternion.identity);

                dropItem.name = dropSkillItemList[dropSkillNum].name;

            }
            else
            {
                //消費アイテムドロップ
                int dropItemNum = Random.Range(0, dropItemList.Count);

                dropItem = Instantiate(dropItemList[dropItemNum], cardPos.position, Quaternion.identity);

                dropItem.name = dropItemList[dropItemNum].name;

            }
        }

        itemGetPanel.SetActive(true);

        SoundManager.instance.PlaySE(SoundType.Drop);
    }

    //初期ミニポーション取得
    private void FirstItem()
    {

        dropItem = Instantiate(dropItemList[9], cardPos.position, Quaternion.identity);

        dropItem.name = dropItemList[9].name;

        GameObject getItem = Instantiate(iconList[33]);

        getItem.name = iconList[33].name;

        getItem.SetActive(false);

        Player.instance.FirstItemGet(getItem);

        //入手したアイテムリストへ格納
        playerGotItemList.Add(dropItem);

        //ドロップアイテムを子要素にする
        dropItem.transform.parent = this.transform;

        //表示しているアイテムカードを非表示
        dropItem.SetActive(false);

    }

    //ドロップアイテムをプレイヤーのリストへの格納関数 ボタンに付ける
    public void PlayerAddItem()
    {
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        //ドロップアイテムについているスクリプトを格納
        WeaponCard card = dropItem.GetComponent<WeaponCard>();

        //カードについている名前とアイコン名を判定する
        for (int i = 0; i < iconList.Count; i++)
        {
            if (iconList[i].name == card.iconName)
            {
                //武器かアイテムを判定する
                if (card.itemType == "Weapon")
                {
                    GameObject getItem = Instantiate(iconList[i]);

                    //プレファブを生成したらCloneがつくのでそれを無くす
                    getItem.name = iconList[i].name;

                    getItem.SetActive(false);

                    //入手したアイテムをスロットに入れる
                    Player.instance.AddWeapon(getItem);

                    //入手した武器リストへ格納
                    playerGotWeaponList.Add(dropItem);
                }
                else if (card.itemType == "Item")
                {
                    GameObject getItem = Instantiate(iconList[i]);

                    getItem.name = iconList[i].name;

                    getItem.SetActive(false);

                    Player.instance.AddItem(getItem);

                    //入手したアイテムリストへ格納
                    playerGotItemList.Add(dropItem);
                }
            }
        }

        //ドロップアイテムを子要素にする
        dropItem.transform.parent = this.transform;

        //表示しているアイテムカードを非表示
        dropItem.SetActive(false);

        //ボタンを表示しているパネル非表示
        itemGetPanel.SetActive(false);

    }


    //ドロップアイテムを入手しない ボタンに付ける
    public void PlayerCancelItem()
    {
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        //表示しているアイテムカードを消す
        Destroy(dropItem);

        //ボタンを表示しているパネル非表示
        itemGetPanel.SetActive(false);

        nextWave = true;

    }


    //クリック判定関数
    private void ItemClick()
    {
        //左クリック
        if (Input.GetMouseButtonDown(0))
        {
            // UI上をクリックしたら何もしない
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //レイを飛ばしてあたったコライダーを判定する
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

            //とばしたレイに何かヒットしたかの判定
            if (hit2d.collider)
            {

                //武器アイコンクリック時
                if (hit2d.collider.CompareTag("Weapon"))
                {

                    //武器アイコンについているWeaponMangerを格納
                    weaponSkill = hit2d.collider.gameObject.GetComponent<WeaponManager>();


                    for (int i = 0; i < playerGotWeaponList.Count; i++)
                    {
                        SoundManager.instance.PlaySE(SoundType.Select);

                        if (i == weaponSkill.thisWeaponListNum - 1)
                        {
                            //武器に対応したカードを表示
                            playerGotWeaponList[i].SetActive(true);

                            //今表示されているカードを一時的に格納
                            viewCard = playerGotWeaponList[i];

                            //スキル使用パネル表示
                            skillUsePanel.SetActive(true);
                        }
                    }

                }


                //スキル武器クリック時
                if (hit2d.collider.CompareTag("SkillWeapon"))
                {
                    SoundManager.instance.PlaySE(SoundType.Select);

                    //武器アイコンについているWeaponMangerを格納
                    weaponSkill = hit2d.collider.gameObject.GetComponent<WeaponManager>();

                    for (int i = 0; i < playerGotWeaponList.Count; i++)
                    {

                        if (i == weaponSkill.thisWeaponListNum - 1)
                        {
                            //武器に対応したカードを表示
                            playerGotWeaponList[i].SetActive(true);

                            //今表示されているカードを一時的に格納
                            viewCard = playerGotWeaponList[i];

                            //戻る専用パネル表示
                            backPanel.SetActive(true);

                        }
                    }
                }

                //アイテムアイコンクリック時
                if (hit2d.collider.CompareTag("Item"))
                {
                    SoundManager.instance.PlaySE(SoundType.Select);

                    //アイテムアイコンについているスクリプトを格納
                    itemEffect = hit2d.collider.gameObject.GetComponent<ItemManager>();

                    for (int i = 0; i < playerGotItemList.Count; i++)
                    {

                        if (i == itemEffect.thisItemListNum)
                        {
                            //武器、アイテムに対応したカードを表示
                            playerGotItemList[i].SetActive(true);

                            //今表示されているカードを一時的に格納
                            viewCard = playerGotItemList[i];

                            //消費アイテム使用パネル表示
                            itemUsePanel.SetActive(true);

                        }
                    }
                }

                //プレイヤースキルクリック時
                if (hit2d.collider.CompareTag("PlayerSkill"))
                {
                    SoundManager.instance.PlaySE(SoundType.Select);

                    string skillName = hit2d.collider.gameObject.name;

                    //プレイヤースキル,スキル使用確認画面表示
                    ShowPlayerSkill(skillName);
                }

            }

        }

    }

    //プレイヤースキル,スキル使用確認画面表示関数
    private void ShowPlayerSkill(string skill)
    {

        if (skill == "SpecialSkill_AttackUP")
        {
            //スキル使用確認画面表示
            playerSkillPanel.SetActive(true);
            playerSkill1.SetActive(true);
            playerSkillNum = 1;

        }
        else if (skill == "SpecialSkill_Block")
        {
            //スキル使用確認画面表示
            playerSkillPanel.SetActive(true);
            playerSkill2.SetActive(true);
            playerSkillNum = 2;
        }
        else if (skill == "SpecialSkill_Attack")
        {
            //スキル使用確認画面表示
            playerSkillPanel.SetActive(true);
            playerSkill3.SetActive(true);
            playerSkillNum = 3;
        }
        else if (skill == "SpecialSkill_Heal")
        {
            //スキル使用確認画面表示
            playerSkillPanel.SetActive(true);
            playerSkill4.SetActive(true);
            playerSkillNum = 4;
        }
    }

    //プレイヤースキル使用関数（ボタンに付ける）
    public void UsePlayerSlill()
    {
        //スキル使用可能判定（闘気が5以上の時使用可能）
        if (5 <= Player.instance.specialEnergy)
        {
            HiddenPlayerSkill();
            //ボタンクリック音
            SoundManager.instance.PlaySE(SoundType.SystemButton);

            Invoke(nameof(PlayerSkillProcess), 0.5f);

        }
        else
        {
            //使用不可ＳＥ
            SoundManager.instance.PlaySE(SoundType.NotSelect);

        }
        
    }

    //プレイヤースキル使用

    private void PlayerSkillProcess()
    {
        if (playerSkillNum == 1)
        {
            //攻撃力バフスキル 消費した闘気×5%の値加算
            Player.instance.playerSkillBuff = true;

            //加算する攻撃力計算
            float attackBuffRate = Player.instance.specialEnergy * 0.05f;

            //ステータス更新処理
            Player.instance.StatusBuff(attackBuffRate, 0, 0);

            SoundManager.instance.PlaySE(SoundType.Buff);

            //闘気を0にする
            Player.instance.specialEnergy = 0;

        }
        else if (playerSkillNum == 2)
        {
            //ブロック付与 （消費する闘気の二倍のブロックを付与する）
            float grantBlockNum = Player.instance.specialEnergy * 2;

            //ブロックの値加算
            Player.instance.blockNum += grantBlockNum;

            SoundManager.instance.PlaySE(SoundType.Buff);

            //闘気を0にする
            Player.instance.specialEnergy = 0;

        }
        else if (playerSkillNum == 3)
        {
            //強攻撃 消費する闘気の五倍のダメージ
            float damage = Player.instance.specialEnergy * 5;

            //ダメージ計算
            EnemyManager.instance.currentEnemyHealth -= damage;

            //ダメージテキスト表示
            PlayerToEnemyDamageText(damage);

            //攻撃モーション
            Player.instance.PlayerAttackAnimation();

            SoundManager.instance.PlaySE(SoundType.Attack);

            //闘気を0にする
            Player.instance.specialEnergy = 0;

        }
        else if (playerSkillNum == 4)
        {
            //体力回復 消費する闘気の二倍の値回復
            float healNum = Player.instance.specialEnergy * 2;

            //回復処理
            Player.instance.currentPlayerHealth += healNum;

            //回復したテキスト表示
            PlayerHealText(healNum);

            //体力の最大値を超える場合は最大体力値にする
            if (Player.instance.playerHealth < Player.instance.currentPlayerHealth)
            {
                Player.instance.currentPlayerHealth = Player.instance.playerHealth;
            }

            SoundManager.instance.PlaySE(SoundType.Buff);

            //闘気を0にする
            Player.instance.specialEnergy = 0;
        }
    }

    //表示しているスキル使用ボタン、スキル詳細画像を非表示にする
    public void HiddenPlayerSkill()
    {
        playerSkillPanel.SetActive(false);
        playerSkill1.SetActive(false);
        playerSkill2.SetActive(false);
        playerSkill3.SetActive(false);
        playerSkill4.SetActive(false);
    }

    //メイン武器の詳細表示,プレイヤーのステータス表示
    private void ShowMainWeaponDetailAndStatus()
    {
        //レイを飛ばしてあたったコライダーを判定する
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

        // UIが有れば反応しない
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (hit2d.collider)
        {
            if (hit2d.collider.gameObject.name == "MainWeapon_1")
            {
                mainWeapon1Detail.SetActive(true);
            }
            else if (hit2d.collider.gameObject.name == "MainWeapon_2")
            {
                mainWeapon2Detail.SetActive(true);
            }
            else if (hit2d.collider.gameObject.name == "MainWeapon_3")
            {
                mainWeapon3Detail.SetActive(true);
            }
            else if(hit2d.collider.CompareTag("Player"))
            {
                //ステータスボード表示
                ShowStatusBoad();
            }
            else
            {
                mainWeapon1Detail.SetActive(false);
                mainWeapon2Detail.SetActive(false);
                mainWeapon3Detail.SetActive(false);

                //ステータスボード非表示
                HiddenStatusBoad();

            }
        }
        else
        {
            mainWeapon1Detail.SetActive(false);
            mainWeapon2Detail.SetActive(false);
            mainWeapon3Detail.SetActive(false);

            //ステータスボード非表示
            HiddenStatusBoad();
        }

    }

    //ステータスボード表示関数
    private void ShowStatusBoad()
    {
        statusBoad.SetActive(true);

        //攻撃力を整数化,回避率とクリティカル率を％表示

        float attackTextNum = Mathf.Round(Player.instance.playerAttack);

        float evasionRateNum = Mathf.Round(Player.instance.playerEvasionRate * 100);

        float criticalRateNum = Mathf.Round(Player.instance.playerCriticalRate * 100);

        if (100 < evasionRateNum)
        {
            evasionRateNum = 100;
        }

        if (100 < criticalRateNum)
        {
            criticalRateNum = 100;
        }

        attackStatusText.text = "攻撃力:" + attackTextNum;

        evasionRateStatusText.text = "回避率:" + evasionRateNum + "％";

        criticalRateStatusText.text = "クリティカル率:" + criticalRateNum + "％";

        attackStatusText.enabled = true;
        evasionRateStatusText.enabled = true;
        criticalRateStatusText.enabled = true;

    }

    //ステータスボード非表示
    private void HiddenStatusBoad()
    {
        statusBoad.SetActive(false);

        attackStatusText.enabled = false;

        evasionRateStatusText.enabled = false;

        criticalRateStatusText.enabled = false;
    }

    //武器チェンジ関数
    private void WeaponChange()
    {
        //左クリック
        if (Input.GetMouseButtonDown(0))
        {

            // UI上をクリックしたら何もしない
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            //レイを飛ばしてあたったコライダーを判定する
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

            //とばしたレイに何かヒットしたかの判定
            if (hit2d.collider)
            {

                //武器アイコンクリック時
                if (hit2d.collider.CompareTag("Weapon") || hit2d.collider.CompareTag("SkillWeapon"))
                {
                    //選択した武器オブジェクト
                    selectWeapon = hit2d.collider.gameObject;

                    //選択した武器アイコンについているWeaponMangerを格納
                    weaponSkill = selectWeapon.GetComponent<WeaponManager>();

                    SoundManager.instance.PlaySE(SoundType.Select);

                    //選択した武器と所持している武器の整合性を取る
                    for (int i = 0; i < playerGotWeaponList.Count; i++)
                    {
                        if (i == weaponSkill.thisWeaponListNum - 1)
                        {
                            //武器に対応したカードを表示
                            playerGotWeaponList[i].SetActive(true);

                            //今表示されているカードを一時的に格納
                            viewCard = playerGotWeaponList[i];

                            //武器入れ替え用パネル表示
                            weaponChangePanel.SetActive(true);

                            //武器入れ替えテキスト非表示
                            weaponChangeText.SetActive(false);
                        }
                    }

                }

            }

        }
    }

    //武器入れ替え用関数　ボタンに付ける
    public void Change()
    {

        //SE
        SoundManager.instance.PlaySE(SoundType.SystemButton);

        for (int i = 0; Player.instance.Weapon.Count > i; i++) 
        {
            if (Player.instance.Weapon[i] == selectWeapon)
            {
                //プレイヤーの武器リストへ格納
                Player.instance.Weapon[i] = changeWeapon;

                //アイコンのオブジェクトの親要素を指定
                changeWeapon.transform.parent = Player.instance.haveItemMass.transform;

                //アイコンのオブジェクトの場所を入れ替える場所へ指定
                changeWeapon.transform.position = selectWeapon.transform.position;

                //武器アイコン表示
                changeWeapon.SetActive(true);

                //入れ替えるアイテムのカードのリスト番号を取得
                int changeWeaponCardNum =  playerGotWeaponList.IndexOf(viewCard);

                //リストの最後尾の番号取得
                int lastIndex = playerGotWeaponList.Count - 1;

                //交換するカードがplayerGotWeaponListの最後尾に格納されているのでそれと入れ替える
                playerGotWeaponList[changeWeaponCardNum] = playerGotWeaponList[lastIndex];

                //入れ替え前のカードをリストから除外
                playerGotWeaponList.Remove(viewCard);

                //最後尾のデータを除外
                playerGotWeaponList.RemoveAt(lastIndex);

                //表示しているカードを削除
                Destroy(viewCard);

                //入れ替え先の武器アイコン削除
                Destroy(selectWeapon);

            }
            
        }

        //武器入れ替え用パネル非表示
        weaponChangePanel.SetActive(false);

        //次のシーンへ遷移フラグ
        nextWave = true;

        //武器入れ替えフラグを変更
        Player.instance.isWeaponChange = false;

        //武器入れ替えテキスト非表示
        weaponChangeText.SetActive(false);

        //プレイヤーのステータス更新
        Player.instance.PlayerStatusUpdate();

        //武器入れ替えテキスト表示フラグ変更
        isWeaponChangeTextDisplay = false;
    }

    //アイテム入れ替え関数
    private void ItemChange()
    {
        //左クリック
        if (Input.GetMouseButtonDown(0))
        {
            // UI上をクリックしたら何もしない
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //レイを飛ばしてあたったコライダーを判定する
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

            //とばしたレイに何かヒットしたかの判定
            if (hit2d.collider)
            {

                //武器アイコンクリック時
                if (hit2d.collider.CompareTag("Item"))
                {
                    SoundManager.instance.PlaySE(SoundType.Select);

                    selectItem = hit2d.collider.gameObject;

                    //アイテムアイコンについているスクリプトを格納
                    itemEffect = hit2d.collider.gameObject.GetComponent<ItemManager>();

                    for (int i = 0; i < playerGotItemList.Count; i++)
                    {
                        if (i == itemEffect.thisItemListNum)
                        {

                            //アイテムに対応したカードを表示
                            playerGotItemList[i].SetActive(true);

                            //今表示されているカードを一時的に格納
                            viewCard = playerGotItemList[i];

                            //アイテム入れ替えパネル表示
                            itemChangePanel.SetActive(true);

                            //アイテム入れ替えテキスト非表示
                            itemChangeText.SetActive(false);
                        }
                    }

                }

            }

        }
    }

    public void ItemChangeExecute()
    {
        for (int i = 0; Player.instance.Item.Count > i; i++)
        {
            if (Player.instance.Item[i] == selectItem)
            {
                //プレイヤーのアイテムリストへ格納
                Player.instance.Item[i] = changeItem;

                //アイコンのオブジェクトの親要素を指定
                changeItem.transform.parent = Player.instance.haveItemMass.transform;

                //アイコンのオブジェクトの場所を入れ替える場所へ指定
                changeItem.transform.position = selectItem.transform.position;

                //アイテムアイコン表示
                changeItem.SetActive(true);


                ////入れ替えるアイテムのカードのリスト番号を取得
                int changeItemCardNum = playerGotItemList.IndexOf(viewCard);

                //リストの最後尾の番号取得
                int lastIndex = playerGotItemList.Count - 1;

                //交換するカードがplayerGotItemListの最後尾に格納されているのでそれと入れ替える
                playerGotItemList[changeItemCardNum] = playerGotItemList[lastIndex];

                //入れ替え前のカードをリストから除外
                playerGotItemList.Remove(viewCard);

                //最後尾のデータを除外
                playerGotItemList.RemoveAt(lastIndex);

                //表示しているカードを削除
                Destroy(viewCard);

                //入れ替え先の武器アイコン削除
                Destroy(selectItem);
            }
        }

        //アイテム入れ替え用パネル非表示
        itemChangePanel.SetActive(false);

        //次のシーンへ遷移フラグ
        nextWave = true;

        //武器入れ替えフラグを変更
        Player.instance.isItemChange = false;

        //アイテム入れ替えテキスト非表示
        itemChangeText.SetActive(false);

        //アイテム入れ替えテキスト表示フラグ変更
        isItemChangeTextDisplay = false;

    }


    //スキル使用関数 ボタンに付ける
    public void WeaponSkillActivation()
    {
        weaponSkill.UseWeaponSkill();

        //スキル使用パネル非表示
        skillUsePanel.SetActive(false);

        //表示されている武器カード非表示
        viewCard.SetActive(false);

        SoundManager.instance.PlaySE(SoundType.SystemButton);
    }

    //アイテム使用関数　ボタンに付ける
    public void ItemActivation()
    {
        //アイテムスキル発動
        itemEffect.UseItem();

        //消費アイテム使用パネル非表示
        itemUsePanel.SetActive(false);

        //表示されているアイテムカードをリストから除外
        playerGotItemList.Remove(viewCard);

        //表示されているアイテムカード削除
        Destroy(viewCard);

        SoundManager.instance.PlaySE(SoundType.SystemButton);
    }


    //スキルを使用しない　ボタンに付ける
    public void Back()
    {
        //カード非表示
        viewCard.SetActive(false);

        //スキル使用パネル非表示
        skillUsePanel.SetActive(false);

        //戻る専用パネル非表示
        backPanel.SetActive(false);

        //消費アイテム使用パネル非表示
        itemUsePanel.SetActive(false);

        //武器入れ替え用パネル非表示
        weaponChangePanel.SetActive(false);

        //アイテム入れ替え用パネル非表示
        itemChangePanel.SetActive(false);

        //戻るボタンを押したらもう一度テキスト表示（武器変更テキスト）
        isWeaponChangeTextDisplay = false;

        //戻るボタンを押したらもう一度テキスト表示（アイテム変更テキスト）
        isItemChangeTextDisplay = false;

        SoundManager.instance.PlaySE(SoundType.SystemButton);

    }


    //敵へのダメージテキスト表示関数
    private void PlayerToEnemyDamageText(float damage)
    {
        GameObject damageText = Instantiate(enemyDamageTextCanvas, EnemyManager.instance.enemyPos.transform.position, Quaternion.identity);

        damageText.GetComponentInChildren<Text>().text = damage.ToString();

    }

    //敵へのクリティカルダメージテキスト表示関数
    private void PlayerToEnemyCriticalDamageText(float damage)
    {
        GameObject damageText = Instantiate(enemyDamageTextCanvas, EnemyManager.instance.enemyPos.transform.position, Quaternion.identity);

        damageText.GetComponentInChildren<Text>().text = "Critical\n" + damage;

    }

    //敵への追撃ダメージテキスト表示関数
    private void PlayerToEnemyExtraDamageText(float damage, bool skillFlag)
    {
        if (skillFlag == daggerBuff)//短剣
        {
            GameObject damageText = Instantiate(enemyDamageTextCanvas, EnemyManager.instance.enemyPos.transform.position + new Vector3(-0.5f,0.5f,0), Quaternion.identity);

            damageText.GetComponentInChildren<Text>().text = damage.ToString();

            damageText.GetComponentInChildren<Text>().fontSize = 50;
        }else if (skillFlag == chakramBuff)//チャクラム
        {
            GameObject damageText = Instantiate(enemyDamageTextCanvas, EnemyManager.instance.enemyPos.transform.position + new Vector3(-0.5f, 1, 0), Quaternion.identity);

            damageText.GetComponentInChildren<Text>().text = damage.ToString();
            damageText.GetComponentInChildren<Text>().fontSize = 50;
        }else if (skillFlag == Player.instance.isRing)//指輪
        {
            GameObject damageText = Instantiate(enemyDamageTextCanvas, EnemyManager.instance.enemyPos.transform.position + new Vector3(-0.5f, 1.5f, 0), Quaternion.identity);

            damageText.GetComponentInChildren<Text>().text = damage.ToString();
            damageText.GetComponentInChildren<Text>().fontSize = 50;
        }

    }

    //敵へのミステキスト表示関数
    private void PlayerMissText()
    {
        GameObject damageText = Instantiate(enemyDamageTextCanvas, EnemyManager.instance.enemyPos.transform.position, Quaternion.identity);

        damageText.GetComponentInChildren<Text>().text = "Miss";

        damageText.GetComponentInChildren<Text>().color = Color.white;

    }

    //プレイヤーのへのダメージテキスト
    private void EnemyToPlayerDamageText(float damage)
    {
        GameObject damageText = Instantiate(playerDamageTextCanvas, Player.instance.playerBattlePos.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        damageText.GetComponentInChildren<Text>().text = damage.ToString();
    }

    //プレイヤーのへのクリティカルダメージテキスト
    private void EnemyToPlayerCriticalDamageText(float damage)
    {
        GameObject damageText = Instantiate(playerDamageTextCanvas, Player.instance.playerBattlePos.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        damageText.GetComponentInChildren<Text>().text = "Critical\n" + damage;
    }

    //敵のプレイヤーへの攻撃ミスor無敵時関数
    private void EnmeyMissText(string text)
    {
        GameObject missText = Instantiate(playerDamageTextCanvas, Player.instance.playerBattlePos.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        missText.GetComponentInChildren<Text>().text = text;

        missText.GetComponentInChildren<Text>().color = Color.white;
    }

    //プレイヤーの回復時テキスト
    public void PlayerHealText(float healNum)
    {
        GameObject healText = Instantiate(playerHealTextCanvas, Player.instance.playerBattlePos.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        healText.GetComponentInChildren<Text>().text = healNum.ToString();
    }

    //バトル終了時(クリアor失敗)にこのバトル中に獲得したお金を確定する
    public void GetMoney()
    {
        //Playerが所持しているバトル中に獲得したなお金の値を総所持金へ加算
        playerAllMoney += Player.instance.playerMoney;

        //獲得した総所持金をセーブ
        PlayerPrefs.SetInt("PlayerGetMoney", playerAllMoney);

        //バトル中に獲得したお金の値を初期化
        Player.instance.playerMoney = 0;
    }


    //難易度決定関数
    public void Nomal()
    {
        nomal = true;
        hard = false;
        veryHard = false;

        diffMultiplier = nomalDiffMultiplier;
    }

    public void Hard()
    {
        nomal = false;
        hard = true;
        veryHard = false;

        diffMultiplier = hardDiffMultiplier;
    }

    public void VeryHard()
    {
        nomal = false;
        hard = false;
        veryHard = true;

        diffMultiplier = veryHardDiffMultiplier;
    }



}
