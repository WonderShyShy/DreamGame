using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterPool : MonoBehaviour
{
    public static MaterPool Instace;

    public void Awake()
    {
        Instace = this;
        
    }
    
    //Prefabs_Scene3/ComboEff
    public  GameObject ComboEff;
    //Prefabs_Scene3/ScoreEff
    public  GameObject ScoreEff;
    //Prefabs_Scene3/BlockScoreEff
    public  GameObject BlockScoreEff;
    //Prefabs_Scene3/BlockScoreEff1
    public  GameObject BlockScoreEff1;
    //Prefabs_Scene3/BoardEffItem
    public  GameObject BoardEffItem;
    //Prefabs_Scene3/ClearSpecialEffBronze
    public  GameObject ClearSpecialEffBronze;
    //Prefabs_Scene3/ClearSpecialEffBronzeLine
    public  GameObject ClearSpecialEffBronzeLine;
    //Prefabs_Scene3/SpecialEffBronze
    public  GameObject SpecialEffBronze;
    //Prefabs_Scene3/ClearSpecialEffGold
    public  GameObject ClearSpecialEffGold;
    //Prefabs_Scene3/ClearSpecialEffGoldLine
    public  GameObject ClearSpecialEffGoldLine;
    //Prefabs_Scene3/SpecialEffGold
    public  GameObject SpecialEffGold;
    //Prefabs_Scene3/IceEff
    public  GameObject IceEff;
    //Prefabs_Scene3/ToIceEff_0123
    public  GameObject ToIceEff_0;
    public  GameObject ToIceEff_1;
    public  GameObject ToIceEff_2;
    public  GameObject ToIceEff_3;
    //Prefabs_Scene3/LevelUpEff
    public  GameObject LevelUpEff;
    //Prefabs_Scene2/WindEff
    public  GameObject WindEff;
    //Prefabs_Scene3/DeadWarning
    public  GameObject DeadWarning;
    //Prefabs/IceTip
    public  GameObject IceTip;
    //BlockItem
    public  GameObject BlockItem;
    //Prefabs_Scene3/SecondChanceDialog
    public  GameObject SecondChanceDialog;
    //Prefabs_Scene3/GameOverDialog
    public  GameObject GameOverDialog;
    //Prefabs_Scene3/SettingsDialog
    public  GameObject SettingsDialog;
}
