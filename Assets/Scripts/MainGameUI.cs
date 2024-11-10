using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    // Start is called before the first frame update
    void Awake()
    {
        restartButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        menuButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }
}