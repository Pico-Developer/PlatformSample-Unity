using UnityEngine;

interface ImeBase
{
    bool Create(ImeDelegateBase pDelegate);
    void GetSize(ref Vector2 size);
    void Draw(Texture2D tex);
    void OnTouch(float x, float y, SGImeMotionEventType type);
    void UpdateData();
    void Show(SGImeInputType typeInput, SGImeTextType typeText);
    void Hide();
}
