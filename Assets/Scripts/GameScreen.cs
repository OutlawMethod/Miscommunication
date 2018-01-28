using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public DirectorState State;
    public Director Director;
    public float Fade = 1;
    public float FadeInDuration = 1;
    public float FadeOutDuration = 0.3f;
    public float Opacity = 0.5f;
    public Image Background;
    public Text Text;
    public AudioSource Audio;
    public float Wait = 0.5f;

    private void Update()
    {
        var isFading = Director.State != State;

        if (isFading)
        {
            Wait = 0.5f;
            Fade = Mathf.Clamp01(Fade + Time.deltaTime / FadeOutDuration);
        }
        else
            Fade = Mathf.Clamp01(Fade - Time.deltaTime / FadeInDuration);

        if (Background != null)
            Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, Mathf.Clamp01(Opacity - Fade * Opacity));

        if (Text != null)
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Mathf.Clamp01(1 - Fade));

        if (!isFading && Fade <= float.Epsilon && Input.GetMouseButtonDown(0) && Wait <= float.Epsilon)
            Director.Restart();

        if (Wait > 0)
            Wait -= Time.deltaTime;

        if (Audio != null)
            Audio.volume = 1 - Fade * 0.75f;
    }
}
