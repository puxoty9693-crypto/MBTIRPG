using UnityEngine;


// 유저 고유 ID를 어디서 가져오는지.
public interface IUserIdProvider
{
    string GetOrCreateUserId();
}
