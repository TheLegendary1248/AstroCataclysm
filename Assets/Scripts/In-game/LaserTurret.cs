using System.Collections;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
	/// <summary>
	/// Determines how the laser should behave
	/// </summary>
	public enum LaserType
	{
		/// <summary>
		/// Single shot that hits obstacles and the player
		/// </summary>
		Alpha,
		/// <summary>
		/// A thinner, weaker consitent laser that hits obstacles and the player
		/// </summary>
		Beta,
		/// <summary>
		/// A single shot, weaker laser that goes through everything 
		/// </summary>
		Gamma
	}
	public bool isFixed;
	public float fixedRange;
	public int damage = 1;
	public float Delay = 3;
	public float BurstDelay = 0;
	public int BurstAmount = 1;
	public float Range = 150;
	public float Betatime = 2;
	public float SweepRange;
	public float LaserWidth = 2f;
	delegate void FireType(Vector2 dir);
	FireType Fire;
	public LaserType Type = LaserType.Alpha;
	public float ChargeTime = 0.5f;
	bool aim = true;
	public ParticleSystem ParticleSys;
	public LineRenderer Line;
	public GameObject Laser;
	public GameObject GammaFX;
	GameObject LaserInstance;
	static int ABLAYER = 8960;
	Vector2 point;
	float startTimeOfFire;
	float startRot;
	private void Start()
	{
		StartCoroutine(Aim());
		ParticleSys.Stop();
		Line.widthMultiplier = LaserWidth;
		if (Type == LaserType.Gamma)
		{
			Fire = Gamma;
		}
		else
		{
			Fire = AB;
		}
	}
	private void FixedUpdate()
	{
		if (aim)
		{
            if (!(!isFixed & Type == LaserType.Beta))
            {
				Vector2 dif = point - (Vector2)transform.position;
				transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg) - 90f);
            }
            else
            {
				transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(startRot - SweepRange, startRot + SweepRange, (Time.time - startTimeOfFire)/Betatime));
            }
		}
	}
	void AB(Vector2 dir) 
	{ 
		RaycastHit2D hit = Physics2D.CircleCast(transform.position, LaserWidth/3f, dir, Range, ABLAYER);
		if(hit.collider != null)
		{
			Health hp = hit.transform.GetComponent<Health>();
			if (hp)
			{
				hp.Dmg(damage);
				if(!hit.collider.gameObject.CompareTag("Obstacle"))PopUp_Text.Set(hit.point, Color.red, PopUp_Text.TypeOfValue.Damage, damage, 2.5f);
			}
		}
		else
		{
			hit.point = (Vector2)transform.position + (dir * Range);
		}
		GameObject obj = LaserInstance == null ? Instantiate(Laser) : LaserInstance;
		obj.transform.position = hit.point;
		LaserInstance = obj;
		LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
		lineRenderer.SetPositions(new Vector3[] { transform.position, hit.point });
		lineRenderer.widthMultiplier = LaserWidth;
	}
	void Gamma(Vector2 dir) 
	{
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, LaserWidth/3f, dir, Range, ABLAYER);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider != null)
			{
				Health hp = hit.transform.GetComponent<Health>();
				if (hp)
				{
					hp.Dmg(damage);
					PopUp_Text.Set(hit.point, Color.white, PopUp_Text.TypeOfValue.Damage, damage, 2.5f);
				}
				Instantiate(GammaFX, hit.point, Quaternion.identity);
			}
		}
		GameObject obj = Instantiate(Laser);
		LaserInstance = obj;
		LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
		lineRenderer.SetPositions(new Vector3[] { transform.position, (Vector2)transform.position + (dir * Range) });
		lineRenderer.widthMultiplier = LaserWidth;
	}
	void AimLaser()//Used to call the appropiate function and calculate angle
	{
		float angle = transform.eulerAngles.z + 90;
		Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
		Fire(dir);
	}
	public IEnumerator Aim() //The firing routine, determined by its variables
	{ 
		yield return new WaitForSeconds(Delay);
		for (int i = 0; i < BurstAmount; i++) //Burst
		{
			yield return new WaitForSeconds(BurstDelay);
			aim = true;
			Line.enabled = true;
			StartCoroutine(AnimSight());
			ParticleSys.Play();
			point = (Random.insideUnitCircle.normalized * fixedRange * (isFixed ? 1f : 0)) + (Vector2)Player.instTransform.position;
			if(!(!isFixed & Type == LaserType.Beta))Markers.Set(point, 0, Markers.Marks.Laser);
			startTimeOfFire = Time.time;
			yield return new WaitForSeconds(ChargeTime);
			float timestamp = Time.time + Betatime;
			startTimeOfFire = Time.time;
			Vector3 dif = Player.instTransform.position - transform.position;
			startRot = (Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg) - 90f;
			do //Used for continous fire
			{
				yield return new WaitForFixedUpdate();//DO NOT REMOVE THIS LINE!!!(crash #3, Reason: forever looping code)
				AimLaser();
			} while (Time.time < timestamp & Type == LaserType.Beta);
			LaserInstance.GetComponent<Laser>().Turn();
			LaserInstance = null;
			ParticleSys.Stop();
			aim = false;
		}
		Line.enabled = false;
		Line.SetPosition(1, Vector3.zero);
		
		StartCoroutine(Aim());
	}
	public IEnumerator AnimSight()
	{
		float f = Time.time;
		do
		{
			yield return new WaitForFixedUpdate();
			Line.SetPosition(1, Vector3.up * (Time.time - f) * Range * 3f);
		} while (Time.time - f < 0.33f);
		Line.SetPosition(1, Vector3.up * Range);
	}
    private void OnDestroy()
    {
        if(LaserInstance)
        {
			LaserInstance.GetComponent<Laser>().Turn();
		}
    }
}
