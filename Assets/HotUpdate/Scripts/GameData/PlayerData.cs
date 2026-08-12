using System;
using System.Collections.Generic;
using UnityEngine;

#if minigame
#else
using System.IO;
using System.Text;
#endif

[Serializable]
public class PlayerData
{
    public string playerId;

    public string playerName;

    public string playerPassword;

    public int playerGold;

    public int plantNum;

    public List<int> ownedPlantsId;

    /// <summary>
    /// 主线关卡
    /// </summary>
    public int MainLevel;

    private static string filePath;

    public PlayerData(string playerId, string playerName, string playerPassword, int playerGold, int mainLevel)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.playerPassword = playerPassword;
        this.playerGold = playerGold;
        MainLevel = mainLevel;
    }

    public PlayerData(string playerId, string playerName, string playerPassword, int playerGold, int plantNum,
        int mainLevel, List<int> ownedPlantsId)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.playerPassword = playerPassword;
        this.playerGold = playerGold;
        this.plantNum = plantNum;
        this.ownedPlantsId = ownedPlantsId;
        MainLevel = mainLevel;
    }

    public PlayerData(string playerId, string playerName, string playerPassword, int playerGold)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.playerPassword = playerPassword;
        this.playerGold = playerGold;
    }

    private PlayerData()
    {
    }

    public static PlayerData GetPlayerData(string name)
    {
        PlayerData playerData;
#if minigame
        string json = PlayerPrefs.GetString(name);
        if (json == "")
        {
            playerData = new PlayerData("", name, "", 0, 1, 1, new List<int> { 1 });
        }
        else
        {
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }

#else
        filePath = Application.persistentDataPath + "/" + name;
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            playerData = new PlayerData("", name, "",0,1,1,new List<int>{1});
            WritePlayerData(playerData);
        }
#endif

        return playerData;
    }

    public static void WritePlayerData(PlayerData playerData)
    {
#if minigame
        PlayerPrefs.SetString(MainGameManager.GetInstance().GetCurrentPlayerData().playerName,
            JsonUtility.ToJson(playerData));
#else
        filePath = Application.persistentDataPath + "/" + playerData.playerName;
        File.WriteAllText(filePath,JsonUtility.ToJson(playerData),Encoding.UTF8);
#endif
    }

    public int GetGold()
    {
        return playerGold;
    }

    public void SetGold(int gold)
    {
        playerGold += gold;
    }
}