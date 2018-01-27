using UnityEngine;
using UnityEngine.UI;

public class HeartCount : MonoBehaviour
{
    public Image Heart;
    public Text Count;

    public float Fade = 1;

    public float FadeDuration = 0.3f;

    public Character Character;
    public Manager Manager;

    private void Update()
    {
        if (Character == null)
        {
            Destroy(gameObject);
            return;
        }

        Count.text = Character.Lives.ToString();

        var rect = GetComponent<RectTransform>();
        rect.position = Camera.main.WorldToScreenPoint(Character.transform.position);
        rect.anchoredPosition -= new Vector2(0, 40);

        var isFading = false;

        if (Manager.Current != null && Character.Team != Manager.Current.Team)
            if (Manager.Grid.HasPath(Character.Cell) || Character.Cell.IsInAttackRange(Manager.Current))
                isFading = true;

        if (isFading)
            Fade = Mathf.Clamp01(Fade + Time.deltaTime / FadeDuration);
        else
            Fade = Mathf.Clamp01(Fade - Time.deltaTime / FadeDuration);

        Count.color = new Color(1, 1, 1, 1 - Fade);
        Heart.color = new Color(1, 1, 1, 1 - Fade);
    }
}
