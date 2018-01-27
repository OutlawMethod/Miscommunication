using UnityEngine;
using System.Collections;

public enum DirectorState
{
    start,
    game,
    redWins,
    blueWins,
    draw
}

public class Director : MonoBehaviour
{
    public Manager Manager;
    public DirectorState State;

    public void Restart()
    {
        State = DirectorState.game;
        Manager.enabled = true;
        Manager.Restart();
    }

    private void Update()
    {
        var previousState = State;

        if (State == DirectorState.game)
        {
            if (Manager.Processing.Count == 0)
            {
                var team0 = 0;
                var team1 = 0;
                var hasAnyControl = false;

                foreach (var character in Manager.Characters)
                {
                    if (character.Team == 0)
                        team0++;
                    else if (character.Team == 1)
                        team1++;

                    if (!hasAnyControl && character.Control > 0)
                        hasAnyControl = true;
                }

                if (team0 > 0 && team1 == 0)
                    State = DirectorState.redWins;
                else if (team1 > 0 && team0 == 0)
                    State = DirectorState.blueWins;
                else if (!hasAnyControl)
                    State = DirectorState.draw;
                else if (team0 == 0 && team1 == 0)
                    State = DirectorState.draw;
            }
        }
        else
        {
            Manager.Stop();
            Manager.enabled = false;
            Manager.Grid.ClearStatus();
        }
    }
}
