using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

    public Vector3 spinAngles;
	
	// Update is called once per frame
	void Update () {
        transform.localRotation *= Quaternion.Euler(spinAngles * Time.deltaTime * 2*Mathf.PI);
	}
}
