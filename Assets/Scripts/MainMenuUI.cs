using DefaultNamespace;
using MazeGenerator.Labyrinth;
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
    [SerializeField] private Button EasyButtonR;
    [SerializeField] private Button MediumButtonR;
    [SerializeField] private Button HardButtonR;
    [SerializeField] private Button ExtremButtonR;

    private void Awake()
    {
        EasyButton.onClick.AddListener(() => StartGame(4));
        MediumButton.onClick.AddListener(() => StartGame(7));
        HardButton.onClick.AddListener(() => StartGame(11));
        ExtremButton.onClick.AddListener(() => StartGame(20));
        EasyButtonR.onClick.AddListener(() => StartGame(3, LabyrinthBase.LabType.Remember));
        MediumButtonR.onClick.AddListener(() => StartGame(4, LabyrinthBase.LabType.Remember));
        HardButtonR.onClick.AddListener(() => StartGame(5, LabyrinthBase.LabType.Remember));
        ExtremButtonR.onClick.AddListener(() => StartGame(6, LabyrinthBase.LabType.Remember));
        QuitButton.onClick.AddListener(() => Application.Quit());
    }

    private void StartGame(int size, LabyrinthBase.LabType labType = LabyrinthBase.LabType.Square)
    {
        SceneData.LabType = labType;
        SceneData.Size = size;
        SceneManager.LoadScene(1);
    }
}