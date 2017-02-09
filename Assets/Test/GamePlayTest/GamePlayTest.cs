//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.IO;
using Game.Knight;

public class GamePlayTest : MonoBehaviour
{
    public string ConfigPath = "Assets/Test/GamePlayTest/game_play_config_test.txt";

    void Start()
    {
        string rConfigText = File.ReadAllText(this.ConfigPath);
        GamePlayComponentParser rParser = new GamePlayComponentParser(rConfigText);
        rParser.Parser();
    }
}
