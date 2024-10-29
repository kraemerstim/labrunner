using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        timeText.text = $"{SceneData.secondsPassed.ToString("0.00")} Sekunden";
    }

    private void Awake()
    {
        restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        
        quitButton.onClick.AddListener(() => Application.Quit());
    }
}
