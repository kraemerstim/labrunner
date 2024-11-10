using DefaultNamespace;
using MazeGenerator.Labyrinth;
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
        var highscoreString =
            $"Highscore_{SceneData.Size}_{(SceneData.LabType == LabyrinthBase.LabType.Remember ? "R" : "S")}";
        var highscore = PlayerPrefs.GetFloat(highscoreString, 0);
        var score = SceneData.SecondsPassed;
        if (SceneData.Win)
        {
            if (highscore == 0 || score < highscore)
            {
                highscore = score;
                PlayerPrefs.SetFloat(highscoreString, highscore);
                PlayerPrefs.Save();
            }

            timeText.text = $"{score:0.00} Sekunden";
        }
        else
        {
            timeText.text = $"Niederlage!";
        }

        highscoreText.text = $"Aktueller Rekord: {highscore:0.00} Sekunden";
    }

    private void Awake()
    {
        restartButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });

        mainMenuButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });

        quitButton.onClick.AddListener(() => Application.Quit());
    }
}