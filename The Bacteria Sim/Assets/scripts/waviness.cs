using UnityEngine;
using System.Collections;

//http://docs.unity3d.com/ScriptReference/Material.SetFloat.html

public class waviness : MonoBehaviour {

		public float newValue;
		public float speed;

	void Update() {

		
		//newValue = Mathf.PingPong(newValue, 1.15f);
		newValue = (Mathf.Sin(Time.time * speed));

		if (newValue < 0) {
		
			newValue *= -1;
		
		}

		newValue += 0.35f;

		
		this.GetComponent<Renderer>().material.SetFloat("_wavyness", newValue);
	}
}