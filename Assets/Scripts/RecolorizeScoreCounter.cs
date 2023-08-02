using UnityEngine;
using System.Collections;

public class RecolorizeScoreCounter : ScoreCounter {

	public Renderer[]		RendererToRecolorize;
	public float ColorizeSpeed;

	override public void Score(IBridgesCharacterController PlayerController)
	{

		if (PlayerController is BridgesCharacterController) {
			BridgesCharacterController controller = (BridgesCharacterController)PlayerController;

			if (controller.ColorizeBridge && RendererToRecolorize!=null) {
				StartCoroutine(Recolorize(controller.RoadSkin()));
			}
		}

	}

	IEnumerator Recolorize(Material destinationMaterial)
	{

		foreach (Renderer r in RendererToRecolorize)
		{
			float lerpVal = 0;

			Material initialMaterial = r.material;
			while (lerpVal<1)
			{
				lerpVal+=Time.deltaTime * ColorizeSpeed;
				if (lerpVal>1) lerpVal=1;
				r.material.Lerp(initialMaterial,destinationMaterial,lerpVal);
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
