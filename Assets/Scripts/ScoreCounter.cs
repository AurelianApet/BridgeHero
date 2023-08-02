using UnityEngine;
using System.Collections;

public abstract class ScoreCounter : MonoBehaviour {

	abstract public void Score(IBridgesCharacterController PlayerController);
}
