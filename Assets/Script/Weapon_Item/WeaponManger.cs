using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class WeaponManger : MonoBehaviour
{
    //武器のステータス
    public float attack, health;

    //武器スキルクールタイム
    public int coolTime;

    //このスクリプトがついている武器の名前格納
    private string weaponName;

    //スキル発動ボタンを押してからスキル発動までのすき間時間
    private float delayTime = 0.5f;

    //この武器かプレイヤーの武器リストの何番目に入っているかの番号
    public int thisWeaponListNum;

    //テキスト用Canvas
    [SerializeField]
    private GameObject coolTimeText;

    //テキストコンポーネント格納用変数
    private Text ctText;

    //武器スキル使用フラグ
    private bool usedSkill;

    //武器のリキャストタイム減少フラグ
    private bool isCount;

    //各武器の攻撃力ステータス
    private Dictionary<string, float> weaponAttackDic = new Dictionary<string, float>() 
    {

        {"MainWeapon_1", 5 }, {"MainWeapon_2", 10 }, {"MainWeapon_3" , 15},
        {"Sword", 5}, {"Ax", 7}, {"Dagger", 2}, {"Sickle", 10}, {"Saber", 6}, {"GreatSword", 12},
        {"Rapier", 4}, {"Spear", 3}, {"Hammer", 8}, {"Club", 3}, {"Shield", 1}, {"Tonfa", 2 }, {"Whip", 5},
        {"Chakram", 3}, {"Bow", 3}, {"BowGun", 5}, {"Gun", 8}, {"Bangle",0}, {"Boots",0}, {"Fang",0}, {"Glasses",0},
        {"Necklace",0}, {"Ring",0}, {"SkillShield",0},

    };

    //各武器の体力ステータス
    private Dictionary<string, float> weaponHealthDic = new Dictionary<string, float>()
    {

        {"MainWeapon_1", 10}, {"MainWeapon_2", 15}, {"MainWeapon_3" , 20},
        {"Sword", 6}, {"Ax", 5}, {"Dagger", 3}, {"Sickle", 1}, {"Saber", 4}, {"GreatSword", 0},
        {"Rapier", 8}, {"Spear", 6}, {"Hammer", 2}, {"Club", 3}, {"Shield", 10}, {"Tonfa", 5 }, {"Whip", 3},
        {"Chakram", 4}, {"Bow", 4}, {"BowGun", 3}, {"Gun", 2}, {"Bangle",0}, {"Boots",0}, {"Fang",0}, {"Glasses",0},
        {"Necklace",0}, {"Ring",0}, {"SkillShield",0},

    };

    //各武器のリキャストタイム
    private Dictionary<string, int> weaponSkillCoolTimeDic = new Dictionary<string, int>()
    {

        {"MainWeapon_1", 0}, {"MainWeapon_2", 0}, {"MainWeapon_3" , 0},
        {"Sword", 3}, {"Ax", 4}, {"Dagger", 5}, {"Sickle", 6}, {"Saber", 5}, {"GreatSword", 5},
        {"Rapier", 6}, {"Spear", 4}, {"Hammer", 3}, {"Club", 2}, {"Shield", 7}, {"Tonfa", 8 }, {"Whip", 5},
        {"Chakram", 6}, {"Bow", 2}, {"BowGun", 3}, {"Gun", 4}, {"Bangle",0}, {"Boots",0}, {"Fang",0}, {"Glasses",0},
        {"Necklace",0}, {"Ring",0}, {"SkillShield",0},

    };


    private void Awake()
    {
        weaponName = name;

        if (name == "MainWeapon_1" || name == "MainWeapon_2" || name == "MainWeapon_3")
        {
            attack = weaponAttackDic[weaponName];
            health = weaponHealthDic[weaponName];
        }
        else
        {
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponName = name;

        attack = weaponAttackDic[weaponName];
        health = weaponHealthDic[weaponName];

        thisWeaponListNum = Player.instance.Weapon.IndexOf(gameObject);

        

        if (coolTimeText != null)
        {
            
            coolTimeText.SetActive(false);

            //テキストのコンポーネント格納
            ctText = coolTimeText.GetComponentInChildren<Text>();


        }

        SkillWeaponGet();
    }

    // Update is called once per frame
    void Update()
    {
        //この武器アイコンがリストの何番目に配置されているのかを常に反映
        thisWeaponListNum = Player.instance.Weapon.IndexOf(gameObject);

        //武器入れ替え選択時クールタイム表記を消す
        if (Player.instance.isWeaponChange)
        {
            if (coolTimeText != null)
            {
                coolTimeText.SetActive(false);
            }
            
        }

        if (usedSkill && GameManager.instance.playerTurn)
        {

            coolTimeText.SetActive(true);

            if (!isCount)
            {
                coolTime--;
                ctText.text = coolTime.ToString();
                isCount = true;
            }

        }

        if (coolTime <= 0 && usedSkill)
        {
            usedSkill = false;
            //テキスト非表示
            coolTimeText.SetActive(false);
        }

        if (GameManager.instance.enemyTurn || GameManager.instance.battleEnd)
        {
            isCount = false;
        }


    }

    //スキル使用のための武器種の判別関数
    public void UseWeaponSkill()
    {
        switch (weaponName)
        {
            case "Sword":
                //ソードスキル関数
                Invoke("SwordSkill",delayTime);
                CoolTimeProcess();
                return;

            case "Ax":
                //斧スキル関数
                Invoke("AxSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Dagger":
                //短剣スキル関数
                Invoke("DaggerSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Sickle":
                //鎌スキル関数
                Invoke("SickleSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Saber":
                //刀スキル関数
                Invoke("SaberSkill", delayTime);
                CoolTimeProcess();
                return;

            case "GreatSword":
                //大剣スキル関数
                Invoke("GreatSwordSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Rapier":
                //細剣スキル関数
                Invoke("RapierSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Spear":
                //槍スキル関数
                Invoke("SpearSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Hammer":
                //ハンマースキル関数
                Invoke("HammerSkill", delayTime);
                CoolTimeProcess();
                return;
                
            case "Club":
                //こん棒スキル関数
                Invoke("ClubSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Shield":
                //木盾スキル関数
                Invoke("ShieldSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Tonfa":
                //トンファースキル関数
                Invoke("TonfaSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Whip":
                //ムチスキル関数
                Invoke("WhipSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Chakram":
                //チャクラムスキル関数
                Invoke("ChakramSkill", delayTime);
                CoolTimeProcess();
                return;

            case "Bow":
                //弓スキル関数
                Invoke("BowSkill", delayTime);
                CoolTimeProcess();
                return;

            case "BowGun":
                //ボウガンスキル関数
                Invoke("BowGunSkill", delayTime);
                CoolTimeProcess();
                return;


            case "Gun":
                //銃スキル関数
                Invoke("GunSkill", delayTime);
                CoolTimeProcess();
                return;


        }
    }

    //クールタイム処理関数
    private void CoolTimeProcess()
    {
        //クールタイム表示
        coolTimeText.SetActive(true);
        coolTime = weaponSkillCoolTimeDic[weaponName];
        ctText.text = coolTime.ToString();
        //スキル使用ターンはターン数減少しない
        isCount = true;
        //スキルの使用フラグ
        usedSkill = true;
    }

    //ソードスキル関数
    private void SwordSkill()
    {
        //スキル内容 主人公の攻撃力分のダメージを与える敵の攻防ダウン（10％）
        GameManager.instance.PlayerToEnemySkillDamage(1);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();

        //デバフ
        EnemyManger.instance.EnemyDebuff(0.1f, 0.1f, 0);


        //剣によるデバフ判定
        GameManager.instance.swordDebuff = true;

    }


    //斧スキル関数
    private void AxSkill()
    {
        //ステータスバフ攻撃力20％UP
        Player.instance.StatusBuff(0.2f, 0, 0);

        //敵に防御力30％ダウン
        EnemyManger.instance.EnemyDebuff(0, 0.3f, 0);


        //斧によるバフ判定
        Player.instance.axBuff = true;

        //斧によるデバフ判定
        GameManager.instance.axDebuff = true;

        //バフエフェクト
        Player.instance.ShowBuffEffect();
    }


    //短剣スキル関数
    private void DaggerSkill()
    {

        if (!GameManager.instance.daggerBuff)
        {
            //バフエフェクト
            Player.instance.ShowBuffEffect();
        }

        //短剣スキルバフフラグ　　短剣スキルは重複しない
        GameManager.instance.daggerBuff = true;


    }


    //チャクラムスキル関数
    private void ChakramSkill()
    {
        if (!GameManager.instance.chakramBuff)
        {
            //バフエフェクト
            Player.instance.ShowBuffEffect();
        }

        //チャクラムスキルバフフラグ　　チャクラムスキルは重複しない
        GameManager.instance.chakramBuff = true;

    }

    //鎌スキル関数
    private void SickleSkill()
    {
        //ステータスバフ攻撃力100％UP
        Player.instance.StatusBuff(1, 0, 0);

        //敵に攻撃力30％ダウン
        EnemyManger.instance.EnemyDebuff(0.3f, 0, 0);

        //鎌によるバフ判定
        Player.instance.sickleBuff = true;

        //鎌によるデバフ判定
        GameManager.instance.sickleDebuff = true;

        //バフエフェクト
        Player.instance.ShowBuffEffect();
    }

    //刀スキル
    private void SaberSkill()
    {
        //クリティカル率30%UP
        if (!Player.instance.saberBuff)
        {
            Player.instance.StatusBuff(0, 0, 0.3f);

            //バフエフェクト
            Player.instance.ShowBuffEffect();
        }

        //刀によるバフ判定
        Player.instance.saberBuff = true;

        //刀のバフ効果ターン バフ効果が無い場合は3ターンに設定　バフ効果がある場合3ターン延長
        if (Player.instance.saberBuffTurnNum <= 0)
        {
            Player.instance.saberBuffTurnNum = 3;
        }
        else if (0 < Player.instance.saberBuffTurnNum)
        {
            Player.instance.saberBuffTurnNum += 3;
        }
    }

    //大剣スキル
    private void GreatSwordSkill()
    {
        //ステータスバフ攻撃力50%UP ステータスバフは1ターン
        Player.instance.StatusBuff(0.5f, 0, 0);

        //バフエフェクト
        Player.instance.ShowBuffEffect();

        //大剣によるバフ判定
        Player.instance.greatSwordBuff = true;

        //大剣のバフ効果ターン バフ効果が無い場合は2ターンに設定　バフ効果がある場合2ターン延長
        if (Player.instance.greatSwordBuffTurnNum <= 0)
        {
            //守護の盾所持時ブロック付与効果量1.5倍
            if (Player.instance.isSkillShield)
            {
                Player.instance.greatSwordBuffTurnNum = 5;
                Player.instance.blockNum += Mathf.Round(5 * 1.5f);
                return;

            }


            Player.instance.greatSwordBuffTurnNum = 2;
            Player.instance.blockNum += 5;
        }
        else if (0 < Player.instance.greatSwordBuffTurnNum)
        {
            Player.instance.greatSwordBuffTurnNum += 2;
        }

    }

    //細剣スキル
    private void RapierSkill()
    {
        //主人公の攻撃力分の0.5倍ダメージを与える
        GameManager.instance.PlayerToEnemySkillDamage(0.5f);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();


        //回避率30%UP
        if (!Player.instance.rapierBuff)
        {
            Player.instance.StatusBuff(0, 0.3f, 0);

            //バフエフェクト
            Player.instance.ShowBuffEffect();
        }

        //細剣によるバフ判定
        Player.instance.rapierBuff = true;

        //細剣のバフ効果ターン バフ効果が無い場合は3ターンに設定　バフ効果がある場合3ターン延長
        if (Player.instance.rapierBuffTurnNum <= 0)
        {
            Player.instance.rapierBuffTurnNum = 3;
        }
        else if (0 < Player.instance.rapierBuffTurnNum)
        {
            Player.instance.rapierBuffTurnNum += 3;
        }
    }

    //槍スキル
    private void SpearSkill()
    {
        //ステータスバフ攻撃力10%UP ステータスバフは1ターン
        Player.instance.StatusBuff(0.1f, 0.1f, 0);

        //主人公の攻撃力分の0.5倍ダメージを与える
        GameManager.instance.PlayerToEnemySkillDamage(0.5f);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();

        //槍によるバフ判定
        Player.instance.spearBuff = true;

        //バフエフェクト
        Player.instance.ShowBuffEffect();
    }


    //ハンマースキル
    private void HammerSkill()
    {
        //主人公の攻撃力分の0.8倍ダメージを与える
        GameManager.instance.PlayerToEnemySkillDamage(0.8f);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();

        float stunRate = Random.Range(0.0f, 1.0f);

        //50%の確率でスタンにする
        if (stunRate <= 0.5f)
        {
            EnemyManger.instance.isStun = true;

            SoundManager.instance.PlaySE(SoundType.DeBuff);
        }
    }

    //こん棒スキル
    private void ClubSkill()
    {
        //主人公の攻撃力分の0.5倍ダメージを与える
        GameManager.instance.PlayerToEnemySkillDamage(0.5f);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();

        float stunRate = Random.Range(0.0f, 1.0f);

        //50%の確率でスタンにする
        if (stunRate <= 0.3f)
        {
            EnemyManger.instance.isStun = true;

            SoundManager.instance.PlaySE(SoundType.DeBuff);
        }
    }

    //木盾スキル
    private void ShieldSkill()
    {
        //主人公の攻撃力分の0.25倍ダメージを与える
        GameManager.instance.PlayerToEnemySkillDamage(0.25f);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();


        float stunRate = Random.Range(0.0f, 1.0f);

        //50%の確率でスタンにする
        if (stunRate <= 0.2f)
        {
            EnemyManger.instance.isStun = true;

            SoundManager.instance.PlaySE(SoundType.DeBuff);
        }

        //木盾によるのブロックバフ効果ターン バフ効果が無い場合は5ターンに設定　バフ効果がある場合5ターン延長
        if (Player.instance.shieldBuffTurnNum <= 0)
        {
            //守護の盾所持時ブロック付与効果量1.5倍
            if (Player.instance.isSkillShield)
            {
                Player.instance.shieldBuffTurnNum = 3;
                Player.instance.blockNum += Mathf.Round(8 * 1.5f);
                return;

            }

            Player.instance.shieldBuffTurnNum = 3;
            Player.instance.blockNum += 8;

        }
        else if (0 < Player.instance.shieldBuffTurnNum)
        {
            Player.instance.shieldBuffTurnNum += 3;
        }

    }

    //トンファースキル
    private void TonfaSkill()
    {
        Player.instance.tonfaBuff = true;

        //バフエフェクト
        Player.instance.ShowBuffEffect();
    }

    //ムチスキル
    private void WhipSkill()
    {
        //主人公の攻撃力分の0.75倍ダメージを与える
        GameManager.instance.PlayerToEnemySkillDamage(0.75f);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();

        //敵に攻撃力20％ダウン回避率30%ダウン
        EnemyManger.instance.EnemyDebuff(0.2f, 0, 0.3f);

        GameManager.instance.whipDebuff = true;

    }

    //弓スキル
    private void BowSkill()
    {
        //敵に攻撃力10％ダウン
        EnemyManger.instance.EnemyDebuff(0.1f, 0, 0);

        GameManager.instance.bowDebuff = true;


        //ステータスバフ　クリティカル率、回避率10%UP
        Player.instance.StatusBuff(0, 0.1f, 0.1f);

        Player.instance.bowBuff = true;

        //バフエフェクト
        Player.instance.ShowBuffEffect();
    }

    //ボウガンスキル
    private void BowGunSkill()
    {
        //敵に攻撃力15％ダウン
        EnemyManger.instance.EnemyDebuff(0.15f, 0, 0);

        GameManager.instance.bowGunDebuff = true;


        //ステータスバフ　クリティカル率20%、回避率15%UP
        Player.instance.StatusBuff(0, 0.15f, 0.2f);

        Player.instance.bowGunBuff = true;

        //バフエフェクト
        Player.instance.ShowBuffEffect();
    }

    //銃スキル
    private void GunSkill()
    {
        //主人公の攻撃力分の0.5倍ダメージを与える
        GameManager.instance.PlayerToEnemySkillDamage(0.5f);

        //攻撃モーション
        Player.instance.PlayerAttackAnimation();

        //敵に攻撃力10％ダウン
        EnemyManger.instance.EnemyDebuff(0.1f, 0, 0);

        GameManager.instance.gunDebuff = true;


        //ステータスバフ　クリティカル率30%、回避率20%UP
        Player.instance.StatusBuff(0, 0.2f, 0.3f);

        Player.instance.gunBuff = true;

        //バフエフェクト
        Player.instance.ShowBuffEffect();
    }

    //特殊スキル武器入手時の関数
    private void SkillWeaponGet()
    {
        switch (weaponName)
        {
            case "Bangle":
                Player.instance.isBangle = true;
                return;

            case "Boots":
                Player.instance.isBoots = true;
                return;

            case "Fang":
                Player.instance.isFang = true;
                return;

            case "Glasses":
                Player.instance.isGlasses = true;
                return;

            case "Necklace":
                Player.instance.isNecklace = true;
                return;

            case "Ring":
                Player.instance.isRing = true;
                return;

            case "SkillShield":
                Player.instance.isSkillShield = true;
                return;
        }
    }

    private void OnDestroy()
    {
        switch (weaponName)
        {
            case "Bangle":
                Player.instance.isBangle = false;
                return;

            case "Boots":
                Player.instance.isBoots = false;
                return;

            case "Fang":
                Player.instance.isFang = false;
                return;

            case "Glasses":
                Player.instance.isGlasses = false;
                return;

            case "Necklace":
                Player.instance.isNecklace = false;
                return;

            case "Ring":
                Player.instance.isRing = false;
                return;

            case "SkillShield":
                Player.instance.isSkillShield = false;
                return;
        }
    }
}
