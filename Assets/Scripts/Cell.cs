using UnityEngine;

public class Cell : MonoBehaviour
{
    public Character Character;

    public Grid Grid;
    public int X;
    public int Y;

    private void OnMouseOver()
    {
        Grid.Hover = this;
    }
}
