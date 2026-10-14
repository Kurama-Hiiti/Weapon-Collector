using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;

public class ItemManager : MonoBehaviour
{
    //このアイテムがスロットの何番目かの番号
    public int thisItemListNum;

    //このスクリプトがついているオブジェクトの名前
    private string itemName;

    //アイテム使用ボタンを押してから発動までのすき間時間
    private readonly float delayTime = 0.2f;

    void Start()
    {
        //アイテムの名前を格納
        itemName = name;

        //所持しているアイテムのリスト番号取得(リストの何番目のアイテムか判定する)
        thisItemListNum = Player.instance.Item.IndexOf(gameObject);
    }

    void Update()
    {
        //アイテムが使用された時にリストの順番が変更するので都度番号を取得する
        thisItemListNum = Player.instance.Item.IndexOf(gameObject);
    }

    //アイテム使用関数
    public void UseItem()
    {
        switch (itemName)
        {
            case "MiniPotion":
                //ミニポーション効果
                Invoke(nameof(MiniPotionEffect), delayTime);

                return;
            case "MiddlePotion":
                //ミドルポーション効果
                Invoke(nameof(MiddlePotionEffect), delayTime);
                return;
            case "HighPotion":
                //ハイポーション効果
                Invoke(nameof(HighPotionEffect), delayTime);
                return;
            case "ExtraPotion":
                //エクストラポーション効果
                Invoke(nameof(ExtraPotionEffect), delayTime);
                return;
            case "AttackPotion":
                //力のポーション効果
                Invoke(nameof(AttackPotionEffect), delayTime);
                return;
            case "DefensePotion":
                //防御のポーション効果
                Invoke(nameof(DefensePotionEffect), delayTime);
                return;
            case "AmuletAttackDown":
                //呪いの護符（攻）効果
                Invoke(nameof(AmuletAttackDownEffect), delayTime);
                return;
            case "AmuletDefenseDown":
                //呪いの護符（防）効果
                Invoke(nameof(AmuletDefenseDownEffect), delayTime);
                return;
            case "Mantle":
                //マント効果
                Invoke(nameof(MantleEffect), delayTime);
                return;
            case "HolyAmulet":
                //マント効果
                Invoke(nameof(HolyAmuletEffect), delayTime);
                return;

        }
    }

    //体力回復関数
    private void RecoveryHealth(int recoveryNum)
    {
        //プレイヤーの体力回復
        Player.instance.currentPlayerHealth += recoveryNum;

        GameManager.instance.PlayerHealText(recoveryNum);

        //回復SE
        SoundManager.instance.PlaySE(SoundType.Buff);

        if (Player.instance.playerHealth < Player.instance.currentPlayerHealth)
        {
            Player.instance.currentPlayerHealth = Player.instance.playerHealth;
        }
    }

    //アイテム使用時にリストから削除+アイテムのソート
    private void CommonProcess()
    {
        //アイテム使用時にリストから削除
        Player.instance.Item.Remove(gameObject);

        //オブジェクト削除
        Destroy(gameObject);

        //アイテムのソート
        Player.instance.ItemSort();
    }

    //ミニポーション効果
    private void MiniPotionEffect()
    {
        //プレイヤーの体力10回復
        RecoveryHealth(10);

        //共通関数
        CommonProcess();

    }

    //ミドルポーション効果
    private void MiddlePotionEffect()
    {
        //プレイヤーの体力30回復
        RecoveryHealth(30);

        //共通関数
        CommonProcess();
    }

    //ハイポーション効果
    private void HighPotionEffect()
    {
        //プレイヤーの体力50回復
        RecoveryHealth(50);

        //共通関数
        CommonProcess();
    }

    //エクストラポーション効果
    private void ExtraPotionEffect()
    {
        //回復量計算
        float healNum = Player.instance.playerHealth - Player.instance.currentPlayerHealth;

        GameManager.instance.PlayerHealText(healNum);

        //プレイヤーの体力をMaxにする
        Player.instance.currentPlayerHealth = Player.instance.playerHealth;


        //回復SE
        SoundManager.instance.PlaySE(SoundType.Buff);

        //共通関数
        CommonProcess();
    }

    private void AttackPotionEffect()
    {
        //ステータスバフ攻撃力100％UP(攻撃力2倍)
        Player.instance.StatusBuff(1, 0, 0);

        //強化SE
        SoundManager.instance.PlaySE(SoundType.Buff);

        //力のポーションによるバフ判定
        Player.instance.attackPotionBuff = true;

        //共通関数
        CommonProcess();
    }

    private void DefensePotionEffect()
    {
        //強化SE
        SoundManager.instance.PlaySE(SoundType.Buff);

        //共通関数
        CommonProcess();

        //守護の盾所持時ブロック付与効果量1.5倍
        if (Player.instance.isSkillShield)
        {
            Player.instance.blockNum += Mathf.Round(30 * 1.5f);
            return;
        }

        //ブロック30付与
        Player.instance.blockNum += 30;

    }

    //呪いの護符（攻）
    private void AmuletAttackDownEffect()
    {
        //敵に攻撃力50％ダウン
        EnemyManger.instance.EnemyDebuff(0.5f, 0, 0);

        //デバフ管理
        GameManager.instance.attackDownAmulet = true;

        //デバフSE
        SoundManager.instance.PlaySE(SoundType.DeBuff);

        //共通関数
        CommonProcess();
    }


    //呪いの護符（防）
    private void AmuletDefenseDownEffect()
    {
        //敵に防御力50％ダウン
        EnemyManger.instance.EnemyDebuff(0, 0.5f, 0);

        //デバフ管理
        GameManager.instance.defenseDownAmulet = true;

        //デバフSE
        SoundManager.instance.PlaySE(SoundType.DeBuff);

        //共通関数
        CommonProcess();
    }

    //マント
    private void MantleEffect()
    {
        //回避率50％UP
        Player.instance.StatusBuff(0, 0.5f, 0);

        //強化SE
        SoundManager.instance.PlaySE(SoundType.Buff);

        Player.instance.MantleBuff = true;

        //共通関数
        CommonProcess();
    }

    private void HolyAmuletEffect()
    {

        Player.instance.holyAmuletBuff = true;

        //強化SE
        SoundManager.instance.PlaySE(SoundType.Buff);

        //共通関数
        CommonProcess();
    }
}
