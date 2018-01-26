using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct CharacterSet
{
    public Character[] Teams;
}

public class Manager : MonoBehaviour
{
    public bool CanSkip
    {
        get { return !IsProcessing && Current != null; }
    }

    public GameGrid Grid;
    public CharacterSet[] Prefabs;

    public List<Character> Characters = new List<Character>();

    public int Team = 0;

    public Character Current;
    public bool IsProcessing;

    private void Awake()
    {
        Grid.Setup();

        for (int i = 0; i < Prefabs.Length; i++)
        {
            placeCharacter(Prefabs[i], i, 0, 0);
            placeCharacter(Prefabs[i], i, Grid.Height - 1, 1);
        }

        Grid.ClearStatus();
    }

    private void Update()
    {
        Grid.HoverTeam = -1;

        if (IsProcessing && Current.IsActing)
            return;
        else if (IsProcessing)
            updateNextOrder();

        updateInput();   
    }

    private void updateInput()
    {
        if (Current == null)
        {
            Grid.HoverTeam = Team;

            if (Grid.Hover != null && Grid.Hover.Character != null && Grid.Hover.Character.Team == Team)
                if (Input.GetMouseButtonDown(0))
                {
                    Current = Grid.Hover.Character;
                    Current.Range = Current.MaxRange;
                    updateNextOrder();
                }
        }
        else
        {
            if (Grid.Hover != null && Grid.HasPath(Grid.Hover))
            {
                if (Input.GetMouseButtonDown(0))
                    giveCommand(Current, Grid.Hover);
            }
        }
    }

    private void giveCommand(Character character, Cell target)
    {
        if (Grid.HasPath(target))
        {
            var path = Grid.Path(target);

            if (target.Character != null)
                character.Attack(Grid.Path(target));
            else
                character.Move(Grid.Path(target));
        }

        IsProcessing = true;
    }

    private void placeCharacter(CharacterSet prefabs, int x, int y, int team)
    {
        var instance = GameObject.Instantiate(prefabs.Teams[team].gameObject);
        instance.transform.parent = transform;
        instance.transform.position = Grid.Cells[x, y].transform.position;
        instance.SetActive(true);

        var character = instance.GetComponent<Character>();
        character.Team = team;
        character.Cell = Grid.Cells[x, y];
        character.Cell.Character = character;

        Characters.Add(character);
    }

    private void updateNextOrder()
    {
        if (IsProcessing)
        {
            if (Current.Range <= 0)
                Skip();
        }

        if (Current != null)
            Grid.FindPaths(Current.Cell, Current.Range);

        IsProcessing = false;
    }

    public void Skip()
    {
        Current = null;
        Grid.ClearStatus();

        if (Team == 0)
            Team = 1;
        else
            Team = 0;
    }
}
