using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    public Character Character;
    public Text Title;
    public Image Gradient;
    public float Fade;
    public float FadeDuration = 0.3f;

    private void Update()
    {
        var t = GetComponent<RectTransform>();
        t.anchoredPosition = new Vector2(-500 * (1 - Fade), t.anchoredPosition.y);

        UpdateColors();
    }

    public void Fill()
    {
        Title.text = Character.Desc.Name;
        UpdateColors();
    }

    public void UpdateColors()
    {
        Title.color = new Color(Title.color.r, Title.color.g, Title.color.b, Fade);
        Gradient.color = new Color(Gradient.color.r, Gradient.color.g, Gradient.color.b, Fade);
    }

    public void FadeIn()
    {
        Fade = Mathf.Clamp01(Fade + Time.deltaTime / FadeDuration);
    }

    public bool FadeOut()
    {
        Fade = Mathf.Clamp01(Fade - Time.deltaTime / FadeDuration);
        return Fade <= float.Epsilon;
    }
}
