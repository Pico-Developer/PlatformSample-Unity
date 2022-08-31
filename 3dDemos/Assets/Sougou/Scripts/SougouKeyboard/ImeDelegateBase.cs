using UnityEngine;
using System;

public abstract class ImeDelegateBase : MonoBehaviour
{
    public abstract void OnIMEShow(Vector2 vSize);
    public abstract void OnIMEHide();
    public abstract void OnIMECommit(string strCommit);
    public abstract void OnIMEKey(SGImeKey key);
    public abstract void OnIMEError(SGImeError nType, string strErr);
}
