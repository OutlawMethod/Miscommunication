using UnityEngine;

public class Projectile : Actor
{
    public override bool IsActing
    {
        get { return Transition < 1; }
    }

    public float Speed = 10;
    public Vector3 Origin;
    public Vector3 Target;
    public Character Enemy;
    public int Damage;
    public float Transition;
    public bool WasFinished;

    private void LateUpdate()
    {
        if (WasFinished)
            Destroy(gameObject);
        else
        {
            Transition += Time.deltaTime * Speed / Vector3.Distance(Origin, Target);
            transform.position = Fluid.Lerp(Origin, Target, Mathf.Clamp01(Transition), AnimationMode.rough);

            if (Transition >= 1 - float.Epsilon)
            {
                Enemy.Lives -= Damage;

                if (Enemy.Lives <= 0)
                    Enemy.Die();

                WasFinished = true;
            }
        }
    }
}
