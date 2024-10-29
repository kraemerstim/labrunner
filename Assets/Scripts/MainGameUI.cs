using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    
    private float _gameTimer = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        restartButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        menuButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }
    
    // Update is called once per frame
    void Update()
    {
        _gameTimer += Time.deltaTime;
        SceneData.secondsPassed = _gameTimer;
        timerText.text = _gameTimer.ToString("N1");
    }
}
