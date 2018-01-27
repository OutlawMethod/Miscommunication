﻿using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    public Character Character;
    public Text Title;
    public Image Gradient;
    public Text Control;
    public float Fade;
    public float FadeDuration = 0.3f;

    private void Update()
    {
        var t = GetComponent<RectTransform>();
        t.anchoredPosition = new Vector2(-500 * (1 - Fade), t.anchoredPosition.y);

        if (Character.Manager.Processing.Count == 0)
        {
            var control = Character.Manager.GetControl(Character);
            Control.text = control.ToString() + "%";

            var min = 20f;

            Control.color = Color.Lerp(Color.red, Color.white, Mathf.Clamp01((control / min) / (100f / min)));
        }

        UpdateColors();
    }

    public void Fill()
    {
        Title.text = Character.Desc.Name;
        UpdateColors();
    }

    public void UpdateColors()
    {
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
