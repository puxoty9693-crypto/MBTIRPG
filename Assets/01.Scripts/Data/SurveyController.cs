using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SurveyController : MonoBehaviour
{
    //데이터
    [SerializeField] SurveyDatabaseSO survey;
    [SerializeField] ClassDatabaseSO classDb;

    [SerializeField] GameObject surveyPannel;
    [SerializeField] TMP_Text progressText;
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text optionAText;
    [SerializeField] Button optionAButton;
    [SerializeField] TMP_Text optionBText;
    [SerializeField] Button optionBButton;
    [SerializeField] Button backButton;

    //결과 화면
    [SerializeField] GameObject resultPannel;
    [SerializeField] Image classIcon;
    [SerializeField] TMP_Text classNameText;
    [SerializeField] TMP_Text summaryText;
    [SerializeField] TMP_Text passiveText;
    [SerializeField] TMP_Text skillText;
    [SerializeField] TMP_Text ultimateText;
    [SerializeField] Button startButton;

    // 다음 씬
    [SerializeField] string nextSceneName = "";

    readonly List<int> answers = new List<int>();
    ClassDefinitionSO result;

    public event Action<ClassDefinitionSO, IReadOnlyList<int>> OnConfirmed;

    private void Awake()
    {
        optionAButton.onClick.AddListener(() => Choose(Choice.A));
        optionBButton.onClick.AddListener(() => Choose(Choice.B));
        if (backButton != null) backButton.onClick.AddListener(Back);
        startButton.onClick.AddListener(Confirm);
    }

    private void Start()
    {
        if (survey == null || classDb  == null || survey.Count == 0) 
        {
            Debug.LogError("[Survey] 컨트롤러에 데이터베이스랑 클래스 데이터베이스를 연결하세요.", this);
            return;
        }
        Begin();
    }

    


    //-----------설문-----------
    public void Begin() 
    {
        answers.Clear();
        result = null;
        resultPannel.SetActive(false);
        surveyPannel.SetActive(true);
        ShowQuestion();
    }

    void ShowQuestion() 
    {
        int index = answers.Count;
        SurveyQuestionSO q = survey.questions[index];

        progressText.text = $"{index + 1} / {survey.Count}";
        questionText.text = q.question;
        optionAText.text = q.optionA;
        optionBText.text = q.optionB;

        if (backButton != null) backButton.gameObject.SetActive(index > 0);
    }

    void Choose(Choice choice) 
    {
        answers.Add((int)choice);

        if (answers.Count >= survey.Count) Finish();
        else ShowQuestion();
    }

    void Back() 
    {
        if (answers.Count == 0) return;
        answers.Remove(answers.Count - 1);
        ShowQuestion();
    }

    //-----------결과-------------
    void Finish() 
    {
        result = ClassMatcher.Match(answers, survey, classDb);
        if (result == null) 
        {
            Debug.LogError("[Survey] 클래스를 정하지 못 했습니다. 데이터를 확인하세요.", this);
            return;
        }

        GameLog.Log("Survey", $"답변 {string.Join(",", answers)} -> {result.displayName}");
        ShowResult(result);
    }

    void ShowResult(ClassDefinitionSO c) 
    {
        surveyPannel.SetActive(false);
        resultPannel.SetActive(true);

        classNameText.text = c.displayName;
        summaryText.text = c.summary;
        passiveText.text = $"패시브 {c.passiveText}";
        skillText.text = $"스킬 {c.skillText}";
        ultimateText.text = $"궁극기 {c.ultimateText}";

        if (classIcon != null) 
        {
            classIcon.sprite = c.icon;
            classIcon.enabled = c.icon != null;
        }
    }

    void Confirm() 
    {
        if (result == null) return;

        int slot = GameSession.SelectedSlot;
        SaveManager.Data.SetCharacter(CharacterData.Create(slot, result.id, answers));
        SaveManager.Save();
        GameLog.Log("Survey", $"슬롯 {slot}에 {result.displayName} 저장");

        OnConfirmed?.Invoke(result, answers);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
