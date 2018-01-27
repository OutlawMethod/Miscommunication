using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public float FallSpeed = 100;
    public float BlinkDuration = 0.3f;
    public float FadeDuration = 0.3f;

    public bool Target;
    public bool Removed;
    public bool Fading;

    public float Fall;
    public float Fade;
    public float Blink;
    public int BlinkDirection = 1;

    public Character Character;

    public bool HasFallen
    {
        get { return Fall > 1000; }
    }

    private void Update()
    {
        if (Character == null)
        {
            Destroy(gameObject);
            return;
        }

        if (Removed && !HasFallen)
        {
            var move = Time.deltaTime * FallSpeed;
            Fall += move;
            transform.position -= Vector3.up * move;
        }

        if (Fading)
            Fade = Mathf.Clamp01(Fade + Time.deltaTime / FadeDuration);
        else
            Fade = Mathf.Clamp01(Fade - Time.deltaTime / FadeDuration);

        if (Target)
        {
            if (BlinkDirection > 0)
            {
                Blink += Time.deltaTime / BlinkDuration;

                if (Blink >= 1)
                {
                    Blink = 1;
                    BlinkDirection = -1;
                }
            }
            else
            {
                Blink -= Time.deltaTime / BlinkDuration;

                if (Blink <= 0)
                {
                    Blink = 0;
                    BlinkDirection = 1;
                }
            }
        }
        else
            Blink = 0;

        GetComponent<Image>().color = new Color(1, 1, 1, 1 - Mathf.Clamp01(Blink + Fade));
    }
}
