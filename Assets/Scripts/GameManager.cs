using DefaultNamespace;
using LabManager;
using MazeGenerator.Labyrinth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private SquareLabManager squareLabManagerPrefab;
    [SerializeField] private RememberLabManager rememberLabManagerPrefab;
    // Start is called before the first frame update

    private ILabManager _labManager;

    void Start()
    {
        if (SceneData.LabType == LabyrinthBase.LabType.Remember)
        {
            _labManager = Instantiate(rememberLabManagerPrefab);
        }
        else
        {
            _labManager = Instantiate(squareLabManagerPrefab);
        }

        _labManager.OnGameEnd += (sender, args) =>
        {
            SceneData.SecondsPassed = _labManager.GetGameTime();
            SceneData.Win = args.Win;
            SceneManager.LoadScene(2);
        };

        _labManager.OnGameReset += (sender, args) =>
        {
            _labManager.GameStart();
            player.transform.position = _labManager.GetStartPosition();
        };

        player.OnGoalTouched += (sender, args) => _labManager.PlayerTouchedGoal();
        player.OnMoveToNewHex += (o, args) => _labManager.PlayerMovedOnNewField(args.Hexagon);
        _labManager.GameStart();
        player.transform.position = _labManager.GetStartPosition();
    }

    void Update()
    {
        var gameTimer = _labManager.GetGameTime();
        timerText.text = gameTimer.ToString("N1");
    }
}