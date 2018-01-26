using UnityEngine;

public class Cell : MonoBehaviour
{
    public Character Character;

    public Grid Grid;

    public Cell Front;
    public Cell Back;
    public Cell Left;
    public Cell Right;

    private void OnMouseOver()
    {
        Grid.Hover = this;
    }
}
