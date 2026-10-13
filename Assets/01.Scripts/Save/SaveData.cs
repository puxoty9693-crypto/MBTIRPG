using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using Unity.Android.Gradle.Manifest;

[Serializable]
public class SaveData
{
    public const int CurrentVersion = 1;

    public int saveVersion = CurrentVersion;
    public AccountData account = new AccountData();
    public List<CharacterData> characters = new List<CharacterData>();

    public CharacterData GetCharacter(int slotIndex)
        => characters.Find(c => c.slotIndex == slotIndex);

    public void SetCharacter(CharacterData character) 
    {
        RemoveCharacter(character.slotIndex);
        characters.Add(character);
    }

    public bool RemoveCharacter(int slotIndex)
        => characters.RemoveAll(c => c.slotIndex == slotIndex) > 0;

    public void EnsureValid() 
    {
        account ??= new AccountData();
        characters ??= new List<CharacterData>();
        foreach (CharacterData c in characters) 
        {
            c.surveyAnswers ??= new List<int>();
            c.upgrades ??= new List<UpgradeEntry>();
        }
    }
}

// 계정 단위 데이터
[Serializable]
public class AccountData 
{
    public string userId = "";
}


// 캐릭터 단위 데이터 - 슬롯마다 재화가 따로
[Serializable]
public class CharacterData 
{
    public int slotIndex;
    public string classId;
    public List<int> surveyAnswers = new List<int>();
    public string createdAt;

    public int currency;
    public List<UpgradeEntry> upgrades = new List<UpgradeEntry>();

    public static CharacterData Create(int slotIndex, string classId, List<int> answers) 
    {
        return new CharacterData
        {
            slotIndex = slotIndex,
            classId = classId,
            surveyAnswers = answers != null ? new List<int>(answers) : new List<int>(),
            createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public int GetUpgradeLevel(string upgradeId) 
    {
        UpgradeEntry e = upgrades.Find(u => u.id == upgradeId);
        return e != null ? e.level : 0;
    }

    public void SetUpgradeLevel(string upgradeId, int level) 
    {
        UpgradeEntry e = upgrades.Find(u => u.id == upgradeId);
        if (e == null) upgrades.Add(new UpgradeEntry { id = upgradeId, level = level });
        else e.level = level;
    }
}

[Serializable]
public class UpgradeEntry 
{
    public string id;
    public int level;
}

