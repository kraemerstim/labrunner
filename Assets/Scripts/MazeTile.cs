using System.Collections;
using System.Collections.Generic;
using MazeGenerator;
using UnityEngine;

public class MazeTile : MonoBehaviour
{
    [SerializeField] private GameObject door0;
    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;
    [SerializeField] private GameObject door3;
    [SerializeField] private GameObject door4;
    [SerializeField] private GameObject door5;
    [SerializeField] private GameObject goal;
    
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
        goal.SetActive(hexagon.IsFinish);
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
}