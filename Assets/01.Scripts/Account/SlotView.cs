using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotView : MonoBehaviour
{

    public enum State { Empty, Used, Locked}

    [SerializeField] Button button;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text detailText;
    [SerializeField] Image classIcon;

    public Button Button => button;
    public State Current {  get; private set; }

    public void ShowEmpty(int index) 
    {
        Current = State.Empty;
        titleText.text = $"슬롯 {index + 1}";
        detailText.text = "비어 있음 눌러서 설문 시작";
        SetIcon(null);
    }

    public void ShowUsed(int index, ClassDefinitionSO cls, CharacterData data)
    {
        Current = State.Used;
        string className = cls != null ? cls.displayName : data.classId;
        titleText.text = $"슬롯 {index + 1}";
        detailText.text = $"{className} 재화 {data.currency}";
        SetIcon(cls != null ? cls.icon : null);
    }

    public void ShowLocked(int index) 
    {
        Current = State.Locked;
        titleText.text = $"슬롯 {index + 1}";
        detailText.text = "잠김 DLC 필요";
        SetIcon(null);
    }

    void SetIcon(Sprite sprite) 
    {
        if (classIcon == null) return;
        classIcon.sprite = sprite;
        classIcon.enabled = sprite != null;
    }

}
