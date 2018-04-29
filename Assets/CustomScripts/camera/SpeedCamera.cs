using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class SpeedCamera : MonoBehaviour {

	public GameObject TheCar;
	private float TopSpeed;

	private float slowDistance = 5;
	private float slowHeight = 1.5f;
	private float slowRotation = 12;
	private float slowFoV = 60;
	private float fastDistance = 4;
	private float fastHeight = 0.8f;
	private float fastRotation = 0;
	private float fastFoV = 90;
	private Vector3 slowPos; 
	private Vector3 fastPos;

	private float currentSlide = 0;
	public float maxSlide = 3;
	public float slideResponsiveness = 0.1f;
	public float cameraShakeStrengthCoeff = 0.001f;
	public float cameraShakeStartSpeed = 100;

	private float relativeSpeedCoeff = 1.3f; // controls speed-based interpolation

	void Start () {
		TopSpeed = TheCar.GetComponent<CarController> ().MaxSpeed;

		slowPos = new Vector3(0, slowHeight, -slowDistance);
		fastPos = new Vector3(0, fastHeight, -fastDistance);
	}

	void FixedUpdate () {

		float speed = TheCar.GetComponent<CarController> ().CurrentSpeed;
		float relativeSpeed = speed / TopSpeed * relativeSpeedCoeff;
		float rigAngle = transform.localEulerAngles.y;
		float targetSlide = - maxSlide * relativeSpeed * Mathf.Sin (rigAngle * Mathf.Deg2Rad);
		currentSlide = Mathf.Lerp (currentSlide, targetSlide, slideResponsiveness);

		float fov = Mathf.Lerp      (slowFoV,      fastFoV,      relativeSpeed);
		float rot = Mathf.LerpAngle (slowRotation, fastRotation, relativeSpeed);
		Vector3 pos = Vector3.Lerp  (slowPos,      fastPos,      relativeSpeed);
		pos.x = currentSlide;

		float shakeStrength = Mathf.Clamp01 ((speed - cameraShakeStartSpeed) * cameraShakeStrengthCoeff);
		Vector3 cameraShake = Random.insideUnitSphere * shakeStrength * relativeSpeed;

		Camera.main.fieldOfView = fov;
		Camera.main.transform.localPosition = pos + cameraShake;
		Camera.main.transform.localEulerAngles = new Vector3(rot, 0, 0);
	}
}
