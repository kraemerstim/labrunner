using MazeGenerator;
using UnityEngine;

public class MazeTile : MonoBehaviour
{
    public enum HighlightType
    {
        None,
        Red,
        Green
    }

    [SerializeField] private GameObject door0;
    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;
    [SerializeField] private GameObject door3;
    [SerializeField] private GameObject door4;
    [SerializeField] private GameObject door5;
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject visualRed;
    [SerializeField] private GameObject visualGreen;

    private MazeHexagon _hexagon;

    public void SetMazeHexagon(MazeHexagon hexagon)
    {
        _hexagon = hexagon;
        var i = 0;
        foreach (var transition in hexagon.MazeTransitions)
        {
            if (transition is {Activated: true})
            {
                Hide(i);
            }
            else
            {
                Show(i);
            }

            i++;
        }

        goal.SetActive(hexagon.IsGoal);
    }

    public MazeHexagon GetHexagon()
    {
        return _hexagon;
    }

    private GameObject GetDoor(int doorNo)
    {
        return doorNo switch
        {
            0 => door0,
            1 => door1,
            2 => door2,
            3 => door3,
            4 => door4,
            5 => door5,
            _ => null
        };
    }

    private void Show(int doorNo)
    {
        GetDoor(doorNo).SetActive(true);
    }

    private void Hide(int doorNo)
    {
        GetDoor(doorNo).SetActive(false);
    }

    public void Highlight(HighlightType type)
    {
        var highlightGreen = false;
        var highlightRed = false;
        switch (type)
        {
            case HighlightType.Red:
                highlightRed = true;
                break;
            case HighlightType.Green:
                highlightGreen = true;
                break;
            case HighlightType.None:
                break;
        }

        visualRed.SetActive(highlightRed);
        visualGreen.SetActive(highlightGreen);
    }
}