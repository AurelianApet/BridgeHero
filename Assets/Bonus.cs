using UnityEngine;
using System.Collections;

public class Bonus : MonoBehaviour {

	public void Activate()
    {
        PlayerPrefs.SetInt("Coins",PlayerPrefs.GetInt("Coins") + 1);
        PlayerPrefs.Save();

        GetComponent<AudioSource>().Play();
        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.Play("Activate", 0, 0);
        Destroy(gameObject,2);
    }
}
