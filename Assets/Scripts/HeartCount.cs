using UnityEngine;
using UnityEngine.UI;

public class HeartCount : MonoBehaviour
{
    public Text Count; 

    public Character Character;
    public Manager Manager;

    private void Update()
    {
        Count.text = Character.Lives.ToString();

        var rect = GetComponent<RectTransform>();
        rect.position = Camera.main.WorldToScreenPoint(Character.transform.position);
        rect.anchoredPosition -= new Vector2(0, 35);
    }
}
