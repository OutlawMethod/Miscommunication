using UnityEngine;

public enum CellStatus
{
    default_,
    available,
    enemy
}

public class Cell : MonoBehaviour
{
    public Character Character;

    public CellStatus Status;

    public GameGrid Grid;
    public int X;
    public int Y;

    public Material Material;

    public Cell Origin;
    public int Value;
    public bool Visited;

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) renderer = GetComponentInChildren<Renderer>();

        Material = Material.Instantiate(renderer.material);
        renderer.material = Material;
    }

    private void Update()
    {
        switch (Status)
        {
            case CellStatus.default_:
                if (Grid.Hover != null && Grid.HoverTeam >= 0)
                {
                    if (Grid.Hover == this && Character != null && Character.Team == Grid.HoverTeam)
                        Material.color = Color.yellow;
                    else if (Grid.Hover.X == X)
                    {
                        var inPlace = false;

                        if (Grid.HoverTeam == 0)
                        {
                            if (Y < Grid.Hover.Y)
                                inPlace = true;
                        }
                        else if (Grid.HoverTeam == 1)
                        {
                            if (Y > Grid.Hover.Y)
                                inPlace = true;
                        }

                        if (inPlace)
                            Material.color = Color.blue;
                        else
                            Material.color = Color.white;
                    }
                    else
                        Material.color = Color.white;
                }
                else
                    Material.color = Color.white;
                break;

            case CellStatus.available:
                {
                    var inPath = false;

                    var node = Grid.Hover;

                    while (node != null)
                    {
                        if (node == this)
                        {
                            inPath = true;
                            break;
                        }
                        else
                            node = node.Origin;
                    }

                    if (inPath)
                        Material.color = Color.yellow;
                    else
                        Material.color = Color.green;
                }
                break;

            case CellStatus.enemy:
                if (Grid.Hover == this)
                    Material.color = Color.Lerp(Color.red, Color.white, 0.5f);
                else
                    Material.color = Color.red;
                break;
        }
    }

    private void OnMouseOver()
    {
        Grid.Hover = this;
    }

    private void OnMouseExit()
    {
        if (Grid.Hover == this)
            Grid.Hover = null;
    }
}
