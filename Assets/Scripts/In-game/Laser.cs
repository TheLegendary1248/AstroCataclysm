using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Gradient Gradient;
    public LineRenderer Line;
    public ParticleSystem ParticleSys;
    public float FadeTime;
    float TimeStamp;
    void OnEnable()
    {
        TimeStamp = FadeTime + Time.time;
    }
    private void FixedUpdate()
    {
        Color color = Gradient.Evaluate(Mathf.Lerp(1, 0, (TimeStamp - Time.time) / FadeTime));
        Line.startColor = color;
        Line.endColor = color;
    }
    public void Turn()
    {
        enabled = true;
        StartCoroutine(Kill());
    }
    public IEnumerator Kill()
    {
        yield return new WaitForFixedUpdate();
        if (ParticleSys)
        {
            ParticleSys.Stop();
        }
        yield return new WaitForSeconds(FadeTime);
        Destroy(gameObject);
    }
}
