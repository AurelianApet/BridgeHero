using UnityEngine;
using System.Collections;

public class BridgesCharacterController : IBridgesCharacterController
{

	public bool RollingCube = false;
	public bool ColorizeBridge = false;
	public bool RandomizeColor = false;

    public GameObject[] PlayerExtraScoreEffect = null;

	private Material _skin=null;
	private Material _roadSkin=null;

	public Material Skin(){return _skin;}
	public Material RoadSkin(){return _roadSkin;}

    public override void ModifyANewBlock(GameObject aNewObject)
    {
        Renderer[] allrenderers = aNewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in allrenderers) rend.material = RoadSkin();
        if (aNewObject.GetComponent<Renderer> () != null) 
		{
			aNewObject.GetComponent<Renderer> ().material = RoadSkin ();
		}
    }

	public void Start()
	{
		if (RollingCube) {
			Animator animator=GetComponent<Animator>();
			if (animator==null) 
			{
				animator = gameObject.AddComponent<Animator>();
				RuntimeAnimatorController animController =  Resources.Load<RuntimeAnimatorController>("PlayerController");
				animator.runtimeAnimatorController = 
					(RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(animController);
			}
		}
	}

    public override void OnStartMove()
	{
		ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem> ();
		foreach (var ps in pss) {
            var particle = ps.emission;
            particle.enabled = true;
		}
	}

    public override void OnStartFall()
	{
		ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem> ();
		foreach (var ps in pss) {
            var particle = ps.emission;
            particle.enabled = true;
        }

        if (RollingCube == false)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -900, 0));
        else
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -500, 0));
	}

    public override void OnEndMove()
	{
		ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem> ();
		foreach (var ps in pss) {
            var particle = ps.emission;
            particle.enabled = true;
        }
	}

    public override void OnSuccessLanding(bool HasExtraScore,GameObject aPillar)
	{
        if (HasExtraScore)
        {
            if (PlayerExtraScoreEffect != null)
            {
                foreach (var ExtraScoreEffect in PlayerExtraScoreEffect)
                {
                    if (ExtraScoreEffect != null)

                        ExtraScoreEffect.SetActive(true);
                    Animator extraScore_animator = ExtraScoreEffect.GetComponent<Animator>();
                    if (extraScore_animator != null) extraScore_animator.Play("Player_ExtraScoreAnimation", 0, 0);
                }
            }
        }

		Transform tr = transform.FindChild ("ScorePS");
		if (tr != null) {
			ParticleSystem ps = tr.gameObject.GetComponent<ParticleSystem>();
			if (ps!=null)
			{
				ps.time=0;
                var particle = ps.emission;
                particle.enabled = true;
                ps.Play();
			}
		}
	}

    public override void OnStartShift()
	{
		TrailRenderer[] trail_renderers = GetComponentsInChildren<TrailRenderer>();
		foreach (TrailRenderer trail_renderer in trail_renderers) {
						trail_renderer.enabled = false;
				}


	}

    public override void OnEndShift()
	{
		TrailRenderer[] trail_renderers = GetComponentsInChildren<TrailRenderer>();

		foreach (TrailRenderer trail_renderer in trail_renderers) {
			trail_renderer.enabled = true;
		}


	}

    public override void Reset()
	{
		int skinIndex = RandomizeColor ? Random.Range (1, 6) : 1;
		Material skin = Resources.Load<Material>("Skins/Skin"+(skinIndex).ToString());


		Material roadSkin = Instantiate<Material> (skin);
		roadSkin.color = new Color(
			roadSkin.color.r*0.7f,
			roadSkin.color.g*0.7f,
			roadSkin.color.b*0.7f,
			roadSkin.color.a*1.0f
			);

		_roadSkin = roadSkin;

		_skin = Resources.Load<Material>("Skins/Skin"+(skinIndex).ToString()+"_Em");

		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in renderers)
			rend.material = _skin;

		ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem> ();
		foreach (var ps in pss) {
            var particle = ps.emission;
            particle.enabled = true;
        }

		TrailRenderer[] trail_renderers = GetComponentsInChildren<TrailRenderer>();

		foreach(TrailRenderer trail_renderer in trail_renderers)
		{
			trail_renderer.materials[0] = _skin;
			Color aNewColor = Color.Lerp(skin.color,Color.white,0.3f);
			trail_renderer.materials[0].color = new Color(aNewColor.r,aNewColor.g,aNewColor.b,0.75f);
		}
	}

	public void Step()
	{
		transform.position += new Vector3 (GetComponent<Collider>().bounds.size.x, 0, 0);
	}
}
