public class Fixed_Turret_AI : Turret
{
    void Start()
    {
        StartCoroutine(Fire()); 
    }
}