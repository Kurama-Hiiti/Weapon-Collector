using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager instance;

    //エネミーのスプライトパターン
    [SerializeField]
    private List<GameObject> enemySprite;

    //エネミーの攻撃力
    public float enemyAttack;

    //エネミーのMax体力
    [SerializeField]
    private float enemyMaxHealth;

    //エネミーの現在の体力
    public float currentEnemyHealth;

    //エネミーの回避率
    public float enemyEvasionRate;

    //エネミーのクリティカル率
    public float enemyCriticalRate;

    //エネミーの防御力
    public float enemyDefense;


    //エネミーのベース攻撃力
    private float baseAttack = 5f;

    //エネミーのベース体力
    private float baseHealth = 15f;

    //エネミーのベース回避率
    private readonly float baseEvasionRate = 0.05f;

    //エネミーのベースクリティカル率
    private readonly float baseCriticalRate = 0.1f;

    //エネミーのベース防御力
    private float baseDefense = 5f;

    //攻撃力、体力、防御力の成長率
    private readonly float growthRate = 0.2f;

    //回避率成長率
    private readonly float evasionGrowthRate = 0.015f;

    //クリティカル率成長率
    private readonly float criticalGrowthRate = 0.02f;

    //ステータスのランダム補正値
    private float randAttack, randHealth, randDefense;

    //エネミーの表示位置
    public GameObject enemyPos, bossEnemyPos;

    //ターンが加算されたかの判定
    public bool turnAddEnemy;

    //エネミーの攻撃判定
    public bool enemyAttacked;

    //エネミーの攻撃までのスピード
    [SerializeField]
    private float attackTime;

    //生成したスプライト
    private GameObject spawnEnemy;

    //HPバー
    [SerializeField]
    private GameObject enemyHpBar;

    //HPバーの位置
    [SerializeField]
    private Transform enemyHpBarPos;

    //HPバーのイメージ
    [SerializeField]
    private Image enemyHpBarImage;

    //デバフ前のステータス保存用変数
    private float originAttack, originDefense, originEvasionRate;

    //スタン判定
    public bool isStun;

    //エネミーの体力テキスト
    [SerializeField]
    private Text enemyHealthText;

    //デバフアイコン配列
    [SerializeField]
    private GameObject[] enemyDebuffIcons;

    //デバフアイコン位置
    [SerializeField]
    private GameObject[] debuffIconPos;

    //デバフリスト
    [SerializeField]
    private List<GameObject> enemyDebuffList;

    //攻撃のエフェクトの発生ポジション
    [SerializeField]
    private GameObject effectPos;

    //攻撃エフェクト
    [SerializeField]
    private GameObject attackEffect;

    //倒れた時のエフェクト
    [SerializeField]
    private GameObject deathEffect;

    //デバフエフェクト
    [SerializeField]
    private GameObject deBuffEffect;


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
        turnAddEnemy = true;
        enemyAttacked = false;

    }

    void Update()
    {

        //HPバーの表示更新
        //[a, b]の範囲内で補間する値 value を生成する線形パラメータ t を計算する
        enemyHpBarImage.fillAmount = Mathf.Lerp(enemyHpBarImage.fillAmount,
            currentEnemyHealth / enemyMaxHealth, Time.deltaTime * 10f);

        enemyHealthText.text = currentEnemyHealth + "/" + enemyMaxHealth;

        //デバフ時にアイコン表示
        ShowDebuffIcon();


        //バトル開始時一度だけエネミーのステータスが決定される
        if (GameManager.instance.battleStart)
        {
            //10の倍数Waveにボススポーン
            if ((GameManager.instance.waveNum) % 10 == 0)
            {
                BossEnemyStatus();

                BossEnemySpawn();

            }
            else
            {
                EnemyStatus();
                //敵を配置する
                EnemySpawn();
            }


        }

        if (GameManager.instance.enemyTurn)
        {
            //ターンが一度だけ加算される
            if (!turnAddEnemy)
            {
                GameManager.instance.enemyTurnNum++;

                turnAddEnemy = true;
            }

            //敵の攻撃
            if (!enemyAttacked)
            {
                if (currentEnemyHealth <= 0)
                {
                    return;
                }
                else 
                {
                    Invoke(nameof(EnemyAttack), attackTime);
                    enemyAttacked = true;
                }

            }

        }


    }

    //エネミーのステータス関数
    private void EnemyStatus()
    {
        //攻撃力のランダム性
        randAttack = Random.Range(0, 4) - 2;

        //攻撃力
        enemyAttack = Mathf.Round(baseAttack * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * GameManager.instance.diffMultiplier) + randAttack;

        //体力のランダム性
        randHealth = Random.Range(0,10) - 5;

        //体力
        float helthCalNum = (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2));
        enemyMaxHealth = Mathf.Round(baseHealth * helthCalNum * GameManager.instance.diffMultiplier) + randHealth;

        //現在の体力更新
        currentEnemyHealth = enemyMaxHealth;

        //防御力のランダム性
        randDefense = Random.Range(0, 6) - 3;

        //防御力
        float defenseCalNum = Mathf.Sqrt((1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)));
        enemyDefense = Mathf.Round(baseDefense * defenseCalNum * GameManager.instance.diffMultiplier) + randDefense;

        //回避率
        enemyEvasionRate = Mathf.Clamp((baseEvasionRate + evasionGrowthRate * Mathf.Log(GameManager.instance.waveNum, 2)),baseEvasionRate,0.1f);

        //クリティカル率
        enemyCriticalRate = Mathf.Clamp((baseCriticalRate + criticalGrowthRate * Mathf.Log(GameManager.instance.waveNum, 2)), baseEvasionRate, 0.2f);

        //デバフ前のステータス保存
        originAttack = enemyAttack;
        originDefense = enemyDefense;
        originEvasionRate = enemyEvasionRate;

        if (UIManager.instance.sceneName == "GameMainNomal")
        {
            //5～15コインドロップ
            GameManager.instance.dropMoney = Random.Range(5, 15);
        }
        else if (UIManager.instance.sceneName == "GameMainHard")
        {
            //10～20コインドロップ
            GameManager.instance.dropMoney = Random.Range(10, 20);
        }
        else if (UIManager.instance.sceneName == "GameMainVeryHard")
        {
            //15～25コインドロップ
            GameManager.instance.dropMoney = Random.Range(15, 25);
        }
        

    }

    private void BossEnemyStatus()
    {
        //基礎値更新
        BaseStatusChange();



        //攻撃力のランダム性
        randAttack = Random.Range(0, 4) - 2;

        //攻撃力
        enemyAttack = Mathf.Round(baseAttack * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * GameManager.instance.diffMultiplier * 1.5f) + randAttack;

        //体力のランダム性
        randHealth = Random.Range(0, 10) - 5;

        //体力
        float helthCalNum = (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2));
        enemyMaxHealth = Mathf.Round(baseHealth * helthCalNum * GameManager.instance.diffMultiplier * 2.5f) + randHealth;

        //現在の体力更新
        currentEnemyHealth = enemyMaxHealth;

        //防御力のランダム性
        randDefense = Random.Range(0, 6) - 3;

        //防御力
        float defenseCalNum = Mathf.Sqrt((1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)) * (1 + growthRate * Mathf.Log(GameManager.instance.waveNum, 2)));
        enemyDefense = Mathf.Round(baseDefense * defenseCalNum * GameManager.instance.diffMultiplier * 1.2f) + randDefense;

        //回避率
        enemyEvasionRate = Mathf.Clamp((baseEvasionRate + evasionGrowthRate * Mathf.Log(GameManager.instance.waveNum, 2)), baseEvasionRate, 0.1f);

        //クリティカル率
        enemyCriticalRate = Mathf.Clamp((baseCriticalRate + criticalGrowthRate * Mathf.Log(GameManager.instance.waveNum, 2)), baseEvasionRate, 0.2f);

        //デバフ前のステータス保存
        originAttack = enemyAttack;
        originDefense = enemyDefense;
        originEvasionRate = enemyEvasionRate;

        //ドロップするお金

        GameManager.instance.dropMoney = (int)(GameManager.instance.waveNum * 2 * GameManager.instance.diffMultiplier);
    }


    //ボス到達時ステータスの基礎値更新
    private void BaseStatusChange()
    {
        if (GameManager.instance.waveNum == 10)
        {
            baseAttack = 10;
            baseDefense = 12;
            baseHealth = 30;

        }
        else if (GameManager.instance.waveNum == 20)
        {
            baseAttack = 14;
            baseDefense = 14;
            baseHealth = 40;


        }
        else if (GameManager.instance.waveNum == 30)
        {
            baseAttack = 16;
            baseDefense = 18;
            baseHealth = 50;


        }
        else if (GameManager.instance.waveNum == 40)
        {
            baseAttack = 19;
            baseDefense = 24;
            baseHealth = 58;

        }
        else if (GameManager.instance.waveNum == 50)
        {
            baseAttack = 22;
            baseDefense = 28;
            baseHealth = 65;

        }
    }




    //敵のステータスデバフ関数
    public void EnemyDebuff(float attackDebuffRate, float defenseDebuffRate, float evasionDebuffRate)
    {

        //ステータスデバフ
        enemyAttack *= (1 - attackDebuffRate);

        enemyDefense *= (1 - defenseDebuffRate);

        enemyEvasionRate *= (1 - evasionDebuffRate);

        //デバフエフェクト
        Instantiate(deBuffEffect,spawnEnemy.transform.position, Quaternion.identity);

        SoundManager.instance.PlaySE(SoundType.DeBuff);
    }

    //デバフリセット関数
    public void DebuffReset(ref bool deBuffFlag)
    {
        if (deBuffFlag)
        {
            //デバフ前のステータスに戻す
            enemyAttack = originAttack;
            enemyDefense = originDefense;
            enemyEvasionRate = originEvasionRate;
            deBuffFlag = false;
        }
    }


    //敵の配置関数
    private void EnemySpawn()
    {
        //スプライト番号
        int spriteNum = Random.Range(0, 39);

        spawnEnemy = Instantiate(enemySprite[spriteNum], enemyPos.transform.position, Quaternion.identity);

        //HPバー表示
        enemyHpBar.transform.position = enemyHpBarPos.position;

        enemyHpBarImage.fillAmount = currentEnemyHealth / enemyMaxHealth;

        enemyHpBar.SetActive(true);

        //体力テキスト表示
        enemyHealthText.enabled = true;

    }

    //ボス配置関数
    private void BossEnemySpawn()
    {
        //スプライト番号
        int spriteNum = Random.Range(0, 39);

        spawnEnemy = Instantiate(enemySprite[spriteNum], bossEnemyPos.transform.position, Quaternion.identity);

        spawnEnemy.transform.localScale = new Vector3(2, 2, 1);

        //HPバー表示
        enemyHpBar.transform.position = enemyHpBarPos.position;

        enemyHpBarImage.fillAmount = currentEnemyHealth / enemyMaxHealth;

        enemyHpBar.SetActive(true);

        //体力テキスト表示
        enemyHealthText.enabled = true;
    }


    //敵の削除(倒したとき)
    public void EnemyDelete()
    {
        DebuffReset(ref GameManager.instance.swordDebuff);
        DebuffReset(ref GameManager.instance.axDebuff);
        DebuffReset(ref GameManager.instance.sickleDebuff);
        DebuffReset(ref GameManager.instance.whipDebuff);
        DebuffReset(ref GameManager.instance.bowDebuff);
        DebuffReset(ref GameManager.instance.bowGunDebuff);
        DebuffReset(ref GameManager.instance.gunDebuff);
        DebuffReset(ref GameManager.instance.attackDownAmulet);
        DebuffReset(ref GameManager.instance.defenseDownAmulet);

        //デバフアイコン非表示
        for (int i = 0; i < enemyDebuffList.Count; i++)
        {
            enemyDebuffList[i].SetActive(false);

        }
        enemyDebuffList.Clear();

        if (spawnEnemy != null)
        {
            //死亡時エフェクト
            Instantiate(deathEffect, spawnEnemy.transform.position, Quaternion.identity);

            //SE
            SoundManager.instance.PlaySE(SoundType.EnemyDown);

            Destroy(spawnEnemy);

            //HPバー非表示
            enemyHpBar.SetActive(false);

            //体力テキスト非表示
            enemyHealthText.enabled = false;
        }



    }


    //敵の攻撃関数
    private void EnemyAttack()
    {
        if (!isStun)
        {
            //ダメージ計算
            GameManager.instance.EnemyToPlayerDamage();

            //攻撃エフェクト
            Instantiate(attackEffect,effectPos.transform.position,Quaternion.identity);


        }
        else
        {
            //スタン
            isStun = false;
        }


        //エネミーのターン終了
        GameManager.instance.enemyTurn = false;

        //プレイヤーターン加算フラグ
        Player.instance.turnAddPlayer = false;

        //プレイヤーターン開始
        GameManager.instance.playerTurn = true;

    }


    //デバフアイコン表示関数
    private void ShowDebuffIcon()
    {
        //攻撃力ダウン判定
        if (GameManager.instance.swordDebuff || GameManager.instance.sickleDebuff || GameManager.instance.whipDebuff || GameManager.instance.bowDebuff||
            GameManager.instance.bowGunDebuff || GameManager.instance.gunDebuff || GameManager.instance.attackDownAmulet)
        {
            for (int i = 0; i < enemyDebuffIcons.Length; i++)
            {
                if (enemyDebuffIcons[i].name == "AttackDown")
                {
                    //攻撃力ダウンアイコンが無ければリストに追加
                    if (!enemyDebuffList.Any(obj => obj.name == enemyDebuffIcons[i].name))
                    {
                        enemyDebuffList.Add(enemyDebuffIcons[i]);
                    }
                }
            }
        }

        //防御ダウン判定
        if (GameManager.instance.swordDebuff || GameManager.instance.axDebuff || GameManager.instance.defenseDownAmulet)
        {
            for (int i = 0; i < enemyDebuffIcons.Length; i++)
            {
                if (enemyDebuffIcons[i].name == "DefenseDown")
                {
                    //防御ダウンアイコンが無ければリストに追加
                    if (!enemyDebuffList.Any(obj => obj.name == enemyDebuffIcons[i].name))
                    {
                        enemyDebuffList.Add(enemyDebuffIcons[i]);
                    }
                }
            }
        }

        //回避ダウン判定
        if (GameManager.instance.whipDebuff)
        {
            for (int i = 0; i < enemyDebuffIcons.Length; i++)
            {
                if (enemyDebuffIcons[i].name == "EvasionRateDown")
                {
                    //回避ダウンアイコンが無ければリストに追加
                    if (!enemyDebuffList.Any(obj => obj.name == enemyDebuffIcons[i].name))
                    {
                        enemyDebuffList.Add(enemyDebuffIcons[i]);
                    }
                }
            }
        }

        //スタン判定
        if (isStun)
        {
            for (int i = 0; i < enemyDebuffIcons.Length; i++)
            {
                if (enemyDebuffIcons[i].name == "Stan")
                {
                    //スタンアイコンが無ければリストに追加
                    if (!enemyDebuffList.Any(obj => obj.name == enemyDebuffIcons[i].name))
                    {
                        enemyDebuffList.Add(enemyDebuffIcons[i]);
                    }
                }
            }
        }

        //非表示判定関数
        RemoveDebuffIconList();


        //アイコン表示
        if (0 < enemyDebuffList.Count)
        {
            for (int i = 0; i < enemyDebuffList.Count; i++)
            {
                enemyDebuffList[i].transform.position = debuffIconPos[i].transform.position;

                enemyDebuffList[i].SetActive(true);
            }
        }

    }


    //デバフアイコン非表示判定
    private void RemoveDebuffIconList()
    {
        if (0 == enemyDebuffList.Count)
        {
            return;
        }

        //攻撃力ダウン判定
        if (!GameManager.instance.swordDebuff && !GameManager.instance.sickleDebuff && !GameManager.instance.whipDebuff && !GameManager.instance.bowDebuff &&
            !GameManager.instance.bowGunDebuff && !GameManager.instance.gunDebuff && !GameManager.instance.attackDownAmulet)
        {
            for (int i = 0; i < enemyDebuffList.Count; i++) 
            {
                if (enemyDebuffList[i].name == "AttackDown")
                {
                    enemyDebuffList[i].SetActive(false);

                    enemyDebuffList.Remove(enemyDebuffList[i]);
                }
            }
        }

        //防御ダウン判定
        if (!GameManager.instance.swordDebuff && !GameManager.instance.axDebuff && !GameManager.instance.defenseDownAmulet)
        {
            for (int i = 0; i < enemyDebuffList.Count; i++)
            {
                if (enemyDebuffList[i].name == "DefenseDown")
                {
                    enemyDebuffList[i].SetActive(false);

                    enemyDebuffList.Remove(enemyDebuffList[i]);
                }
            }
        }

        //回避ダウン判定
        if (!GameManager.instance.whipDebuff)
        {
            for (int i = 0; i < enemyDebuffList.Count; i++)
            {
                if (enemyDebuffList[i].name == "EvasionRateDown")
                {
                    enemyDebuffList[i].SetActive(false);

                    enemyDebuffList.Remove(enemyDebuffList[i]);
                }
            }
        }

        //スタン判定
        if (!isStun)
        {
            for (int i = 0; i < enemyDebuffList.Count; i++)
            {
                if (enemyDebuffList[i].name == "Stan")
                {
                    enemyDebuffList[i].SetActive(false);

                    enemyDebuffList.Remove(enemyDebuffList[i]);
                }
            }

        }


    }
}
