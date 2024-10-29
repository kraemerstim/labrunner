using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button mainMenuButton;
    
    // Start is called before the first frame update
    void Start()
    {
        
        var highscore= PlayerPrefs.GetFloat($"Highscore_{SceneData.size}", 0);
        var score = SceneData.secondsPassed;
        if (highscore == 0 || score < highscore)
        {
            highscore = score;
            PlayerPrefs.SetFloat($"Highscore_{SceneData.size}", highscore);
            PlayerPrefs.Save();
        }
        timeText.text = $"{score:0.00} Sekunden";
        highscoreText.text = $"Aktueller Rekord: {highscore:0.00} Sekunden";
    }

    private void Awake()
    {
        restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        
        quitButton.onClick.AddListener(() => Application.Quit());
    }
}
