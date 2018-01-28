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
        get { return Processing.Count == 0 && isActiveAndEnabled; }
    }

    public bool NeedPanel
    {
        get { return isActiveAndEnabled && (Current != null || HoverCharacter != null); }
    }

    public Character HoverCharacter
    {
        get
        {
            if (Current != null)
                return null;

            if (Grid.Hover != null && Grid.Hover.Character != null && Grid.Hover.Character.Team == Team)
                return Grid.Hover.Character;

            return null;
        }
    }

    public GameGrid Grid;
    public Canvas Canvas;
    public CharacterPanel Panel;
    public CharacterDesc[] Descriptions;
    public CharacterSet[] Prefabs;
    public HeartPanel Hearts;
    public HeartCount HeartCount;

    public List<Character> Characters = new List<Character>();
    public List<CharacterPanel> Panels = new List<CharacterPanel>();

    public int Team = 0;
    public int StartingTeam;

    public Character Current;

    public List<Actor> Processing = new List<Actor>();

    public void Process(Actor actor)
    {
        if (!Processing.Contains(actor))
            Processing.Add(actor);
    }

    private void Awake()
    {
        Grid.Setup();
    }

    public void Restart()
    {
        foreach (var character in Characters)
            Destroy(character.gameObject);

        Characters.Clear();
        Panels.Clear();

        for (int i = 0; i < Descriptions.Length; i++)
        {
            placeCharacter(i, i, 0, 0);
            placeCharacter(i, Descriptions.Length - i - 1, Grid.Height - 1, 1);
        }

        Grid.ClearStatus();

        Team = StartingTeam;

        if (StartingTeam == 0)
            StartingTeam = 1;
        else
            StartingTeam = 0;
    }

    public void Stop()
    {
        foreach (var panel in Panels)
            Destroy(panel.gameObject);

        Panels.Clear();
    }

    private void Update()
    {
        Grid.HoverTeam = -1;

        if (Processing.Count > 0)
        {
        REMOVE_NULL:
            for (int i = 0; i < Processing.Count; i++)
                if (Processing[i] == null)
                {
                    Processing.RemoveAt(i);
                    goto REMOVE_NULL;
                }

            foreach (var unit in Processing)
                if (unit.IsActing)
                {
                    updatePanels();
                    return;
                }

            Processing.Clear();

            if (Current.Range <= 0)
                Skip();

            updateNextOrder();
        }

        updateDead();
        updateInput();
        updatePanels();
    }

    private void pushPanel(Character character)
    {
        while (Panels.Count > 1)
        {
            GameObject.Destroy(Panels[1].gameObject);
            Panels.RemoveAt(1);
        }

        if (Panels.Count > 0 && Panels[0].Character == character)
            return;

        var instance = GameObject.Instantiate(Panel.transform);
        instance.SetParent(Canvas.transform);
        var rect = instance.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(1000, 0);
        instance.transform.localScale = new Vector3(1, 1, 1);
        instance.gameObject.SetActive(true);

        var panel = instance.GetComponent<CharacterPanel>();
        panel.Character = character;
        panel.Fill();

        Panels.Insert(0, panel);
    }

    private void updateDead()
    {
    LOOP:
        foreach (var character in Characters)
            if (character.Lives <= 0 && !character.IsActing)
            {
                if (character.Cell != null) character.Cell.Character = null;
                GameObject.Destroy(character.gameObject);
                Characters.Remove(character);
                goto LOOP;
            }
    }

    private void updatePanels()
    {
        if (Panels.Count > 1)
            if (Panels[1].FadeOut())
            {
                GameObject.Destroy(Panels[1].gameObject);
                Panels.RemoveAt(1);
            }

        if (Panels.Count > 0)
        {
            if (NeedPanel)
                Panels[0].FadeIn();
            else if (Panels[0].FadeOut())
            {
                GameObject.Destroy(Panels[0].gameObject);
                Panels.RemoveAt(0);
            }
        }
    }

    private void updateInput()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
            return;

        if (Current == null)
        {
            Grid.HoverTeam = Team;

            if (Grid.Hover != null && Grid.Hover.Character != null && Grid.Hover.Character.Team == Team)
            {
                if ((Grid.HoverTeam == 0 && Grid.Last(Grid.Hover.X, Grid.HoverTeam) >= Grid.Hover.Y) ||
                    (Grid.HoverTeam == 1 && Grid.Last(Grid.Hover.X, Grid.HoverTeam) <= Grid.Hover.Y))
                {
                    pushPanel(Grid.Hover.Character);

                    if (Input.GetMouseButtonDown(0))
                    {
                        Current = Grid.Hover.Character;
                        Current.Range = Current.Desc.MaxRange;
                        updateNextOrder();
                    }
                }
            }
        }
        else
        {
            if (Grid.Hover != null)
            {
                var canGive = false;

                if (Grid.Hover.Character != null && Grid.Hover.Character.Team != Current.Team && Grid.Hover.IsInAttackRange(Current))
                {
                    canGive = true;
                    Grid.Point(Current.Cell, Grid.Hover);
                    Grid.Hover.Character.Hearts.Target(Current.IsRivalsWith(Grid.Hover.Character) ? 4 : 2);
                }
                else if (Grid.HasPath(Grid.Hover))
                {
                    canGive = true;

                    if (Grid.Hover.Status == CellStatus.enemy)
                    {
                        Grid.Point(Grid.Hover.AttackOrigin(Current), Grid.Hover);
                        Grid.Hover.Character.Hearts.Target(Current.IsRivalsWith(Grid.Hover.Character) ? 4 : 2);
                    }
                }

                if (canGive && Input.GetMouseButtonDown(0))
                    giveCommand(Current, Grid.Hover);
            }
        }
    }

    private void giveCommand(Character character, Cell target)
    {
        if (UnityEngine.Random.Range(0, 100) <= character.Control)
        {
            if (target.Character != null && target.IsInAttackRange(character))
                character.Attack(new Cell[] { target });
            else if (Grid.HasPath(target))
            {
                if (target.Character != null)
                    character.Attack(Grid.AttackPath(target));
                else
                    character.Move(Grid.Path(target));
            }
        }
        else
            character.Stay();

        Grid.ClearStatus();
    }

    private void placeCharacter(int type, int x, int y, int team)
    {
        var instance = GameObject.Instantiate(Prefabs[type].Teams[team].gameObject);
        instance.transform.parent = transform;
        instance.transform.position = Grid.Cells[x, y].transform.position;
        instance.transform.eulerAngles = new Vector3(instance.transform.eulerAngles.x, 180 * team, instance.transform.eulerAngles.z);

        instance.SetActive(true);

        var character = instance.GetComponent<Character>();
        character.Team = team;
        character.Cell = Grid.Cells[x, y];
        character.Cell.Character = character;
        character.Desc = Descriptions[type];
        character.Lives = character.Desc.MaxLives;
        character.Manager = this;

        Characters.Add(character);

        instance = GameObject.Instantiate(Hearts.gameObject);
        instance.transform.SetParent(Canvas.transform);
        instance.SetActive(true);

        var hearts = instance.GetComponent<HeartPanel>();
        hearts.Manager = this;
        hearts.Character = character;
        hearts.Setup();

        character.Hearts = hearts;

        instance = GameObject.Instantiate(HeartCount.gameObject);
        instance.transform.SetParent(Canvas.transform);
        instance.transform.localScale = new Vector3(1, 1, 1);
        instance.SetActive(true);

        var count = instance.GetComponent<HeartCount>();
        count.Manager = this;
        count.Character = character;
    }

    private void updateNextOrder()
    {
        if (Current != null)
        {
            Grid.FindPaths(Current.Cell, Current.Range);
            pushPanel(Current);
        }
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
