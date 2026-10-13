using UnityEngine;

[CreateAssetMenu(fileName = "Question_", menuName = "MBTI/Survey Question")]
public class SurveyQuestionSO : ScriptableObject
{
    [Tooltip("이 문항이 판단하는 성향 축")]
    public SurveyAxis axis;

    [TextArea(2, 4)] public string question;  // 질문 문장
    public string optionA;                    // 왼쪽 버튼
    public string optionB;                    // 오른쪽 버튼

    // 매칭할 떄 이 문항의 중요도. 사거리·속성·스타일 = 2, 운용·방식 = 1
    [Min(1)] public int weight = 1;
}
