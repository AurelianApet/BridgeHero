using UnityEngine;
using System.Collections;

public class HumanoidCharacterController : IBridgesCharacterController
{
    public float velocity;
    public GameObject[] PlayerExtraScoreEffect = null;

    public override void OnStartMove() {
        GetComponent<Rigidbody>().velocity = new Vector3(1,0,0) * velocity;
    }

    public override void OnEndMove() { GetComponent<Rigidbody>().velocity = Vector3.zero; }
    public void Update()
    {
    }
	override public void OnSuccessLanding(bool HasExtraScore,GameObject aPillar) {
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
    
    }
}
