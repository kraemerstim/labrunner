using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button EasyButton;
    [SerializeField] private Button MediumButton;
    [SerializeField] private Button HardButton;
    [SerializeField] private Button ExtremButton;
    [SerializeField] private Button QuitButton;

    private void Awake()
    {
        EasyButton.onClick.AddListener(() => StartGame(4));
        MediumButton.onClick.AddListener(() => StartGame(7));
        HardButton.onClick.AddListener(() => StartGame(11));
        ExtremButton.onClick.AddListener(() => StartGame(20));
        QuitButton.onClick.AddListener(() => Application.Quit());
    }

    private void StartGame(int size)
    {
        SceneData.size = size;
        SceneManager.LoadScene(1);
    }
}