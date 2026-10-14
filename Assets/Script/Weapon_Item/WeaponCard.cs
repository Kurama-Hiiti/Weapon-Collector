using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCard : MonoBehaviour
{
    //このスクリプトがついているアイテムの名前格納　アイテムカードの名前
    private string itemName;

    //アイコンの名前 プレイヤーのスロットに入れるアイコン
    public string iconName;

    //アイテムのタイプ
    public string itemType;


    //アイテムカード名と実際に加わるアイテムアイコンを対応させる
    private readonly Dictionary<string, string> iconNameDic = new()
    {
        {"Weapon_Ax", "Ax"},{"Weapon_Bow", "Bow"}, {"Weapon_BowGun", "BowGun"}, {"Weapon_Chakram", "Chakram"}, {"Weapon_Club", "Club"}, {"Weapon_Dagger", "Dagger"},
        {"Weapon_GreatSword", "GreatSword"}, {"Weapon_Gun", "Gun"}, {"Weapon_Hammer", "Hammer"}, {"Weapon_Rapier", "Rapier"}, {"Weapon_Saber", "Saber"}, {"Weapon_Shield", "Shield"},
        {"Weapon_Sickle", "Sickle"}, {"Weapon_Spear", "Spear"}, {"Weapon_Sword", "Sword"}, {"Weapon_Tonfa", "Tonfa"}, {"Weapon_Whip", "Whip"},
        {"SkillItem_Bangle", "Bangle"}, {"SkillItem_Boots", "Boots"}, {"SkillItem_Fang", "Fang"},
        {"SkillItem_Glasses", "Glasses" }, {"SkillItem_Necklace", "Necklace"}, {"SkillItem_Ring", "Ring"}, {"SkillItem_Shield", "SkillShield"},
        {"Item_AmuletAttackDown","AmuletAttackDown"},{"Item_AmuletDefenseDown", "AmuletDefenseDown"},{"Item_AttackPotion", "AttackPotion"},
        {"Item_DefensePotion", "DefensePotion"},{"Item_ExtraPotion", "ExtraPotion"}, {"Item_HighPotion", "HighPotion"},{"Item_HolyAmulet", "HolyAmulet"},
        {"Item_Mantle", "Mantle"}, {"Item_MiddlePotion", "MiddlePotion"},{"Item_MiniPotion", "MiniPotion"},

    };

    //武器と消費アイテムの判別するための属性付け
    private readonly Dictionary<string, string> itemTypeDic = new()
    {
        {"Weapon_Ax", "Weapon"},{"Weapon_Bow", "Weapon"}, {"Weapon_BowGun", "Weapon"}, {"Weapon_Chakram", "Weapon"}, {"Weapon_Club", "Weapon"}, {"Weapon_Dagger", "Weapon"},
        {"Weapon_GreatSword", "Weapon"}, {"Weapon_Gun", "Weapon"}, {"Weapon_Hammer", "Weapon"}, {"Weapon_Rapier", "Weapon"}, {"Weapon_Saber", "Weapon"}, {"Weapon_Shield", "Weapon"},
        {"Weapon_Sickle", "Weapon"}, {"Weapon_Spear", "Weapon"}, {"Weapon_Sword", "Weapon"}, {"Weapon_Tonfa", "Weapon"}, {"Weapon_Whip", "Weapon"},
        {"SkillItem_Bangle", "Weapon"}, {"SkillItem_Boots", "Weapon"}, {"SkillItem_Fang", "Weapon"},
        {"SkillItem_Glasses", "Weapon" }, {"SkillItem_Necklace", "Weapon"}, {"SkillItem_Ring", "Weapon"}, {"SkillItem_Shield", "Weapon"},
        {"Item_AmuletAttackDown","Item"},{"Item_AmuletDefenseDown", "Item"},{"Item_AttackPotion", "Item"},
        {"Item_DefensePotion", "Item"},{"Item_ExtraPotion", "Item"}, {"Item_HighPotion", "Item"},{"Item_HolyAmulet", "Item"},
        {"Item_Mantle", "Item"}, {"Item_MiddlePotion", "Item"},{"Item_MiniPotion", "Item"},

    };

    void Start()
    {
        //名前取得
        itemName = name;

        //アイコン名取得
        iconName = iconNameDic[itemName];

        //アイテムタイプ取得
        itemType = itemTypeDic[itemName];
    }
}
