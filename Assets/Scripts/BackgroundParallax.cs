using UnityEngine;
using System.Collections;

public class BackgroundParallax : MonoBehaviour {

	GameObject lastBlock = null;
	public float ParallaxFactor=0.4f;
	void SpawnBlock()
	{
		float minX = 0;
		if (lastBlock != null) {minX = lastBlock.GetComponent<Collider>().bounds.max.x-lastBlock.transform.position.x;}
		GameObject obj =  (GameObject)Instantiate(
													Resources.Load<GameObject>("BackgroundObjects/CityBg"),
		                                          	new Vector3(minX,0,0),
		                                          	Quaternion.identity
		                                          );
		obj.transform.parent = transform;
		obj.transform.localPosition = new Vector3 (minX, 0, 0);
		lastBlock = obj;
	}

	public void Update()
	{


	}

	public void PerformOffset(Vector3 offset)
	{
		for (int i=0; i<transform.childCount; ++i) 
			transform.GetChild(i).position+=offset*ParallaxFactor;

		RemoveBlocksOutside ();

		while (lastBlock.GetComponent<Collider>().bounds.max.x < GetComponent<Collider>().bounds.max.x)
			SpawnBlock();
	}

	void RemoveBlocksOutside()
	{
		for (int i=0;i<transform.childCount;++i)
			if (transform.GetChild(i).GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds)==false)
				Destroy(transform.GetChild(i).gameObject);
	}

	public void Regenerate()
	{
		if (lastBlock == null)
			SpawnBlock();

		while (lastBlock.GetComponent<Collider>().bounds.max.x < GetComponent<Collider>().bounds.max.x)
			SpawnBlock();
	}
}
