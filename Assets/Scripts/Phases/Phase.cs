using UnityEngine;
using System.Collections;

public class Phase : MonoBehaviour
{
    public int timeLimit;

    public PlayerAgent CurrentPlayer { get; set; }
    public bool GameOver { get; set; }

    public virtual void OnEnter()
    {
    }

    public virtual void OnTick()
    {

    }

    public virtual void OnExit()
    {

    }
}
