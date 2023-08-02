using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AdProvider : MonoBehaviour {

	public List<AdProviderInterface>	Providers;

	public bool IsVideoAvailable()
	{
		return false;
	}
}
