using UnityEngine;
using System;
using System.Collections.Specialized;

public class LocalGuidUserIdProvider : IUserIdProvider
{
    const string PrefsKey = "MBTI_UserId";

    public string GetOrCreateUserId()
    {
        string id = PlayerPrefs.GetString(PrefsKey, "");
        AccountData account = SaveManager.Data.account;

        if (string.IsNullOrEmpty(id))
        {
            if (!string.IsNullOrEmpty(account.userId))
            {
                id = account.userId;
                GameLog.Log("Account", $"세이브에 남아 있던 ID로 복구: {id}");
            }
            else 
            {
                id = Guid.NewGuid().ToString("N");
                GameLog.Log("Account", $"새 고유 ID 발급: {account.userId}");
            }

            PlayerPrefs.SetString(PrefsKey, id);
            PlayerPrefs.Save();
            
        }
        if (account.userId != id) 
        {
            account.userId = id;
            SaveManager.Save();
        }

        return id;
    }
}
