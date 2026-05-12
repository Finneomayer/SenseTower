using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionDialogUI : MonoBehaviour
{
    public static QuestionDialogUI Instance { get; private set; }
    private Button yesBtn;
    private Button noBtn;
    private TMP_Text question;

    private void Awake()
    {
        Instance = this;
        yesBtn = transform.Find("YesBtn").GetComponent<Button>();
        noBtn = transform.Find("NoBtn").GetComponent<Button>();
        question = transform.Find("Text").GetComponent<TMP_Text>();

        Hide();
    }
    
    public Task<bool> GetAnswerToYesOrNoQuestion(string questionText)
    {
        return Task.Run(() =>
        {
            question.text = questionText;
            gameObject.SetActive(true);
            bool? answer = null;

            yesBtn.onClick.AddListener(() =>
            {
                answer = true;
            });
            noBtn.onClick.AddListener(() =>
            {
                answer = false;
            });

            while (!answer.HasValue) { }

            Hide();
            yesBtn.onClick.RemoveAllListeners();
            noBtn.onClick.RemoveAllListeners();

            return answer.Value;
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
