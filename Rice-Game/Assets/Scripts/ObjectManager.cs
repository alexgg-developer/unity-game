using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
	private float _startTimeFarmer;
	//private float _startTimePost;
	private float _startTimeRice;

	private float _delayRice = 0.5f;
	//private float _delayPost = 1.0f;

	public Image _farmer;
	public float farmerMovementDuration = 2.0f;
	private float _distanceToMoveFarmer;
	private Vector3 _startPositionFarmer, _endPositionFarmer;

	public Image _post;
	public float postMovementDuration = 2.0f;

	public Image _rice;
	public float riceMovementDuration = 2.0f;
	private float _distanceToMoveRice;
	private Vector3 _startPositionRice, _endPositionRice;
	// Use this for initialization

	void Awake ()
	{
		//Dictionary.init ();
	}

	void Start ()
	{

		_startTimeFarmer = Time.time;
		//_startTimePost = Time.time + _delayPost;
		_startTimeRice = Time.time + _delayRice;

		_farmer.transform.position = new Vector3 (-_farmer.rectTransform.rect.width,
			_farmer.transform.position.y,
			_farmer.transform.position.z);

		_distanceToMoveFarmer = _farmer.rectTransform.rect.width;
		_startPositionFarmer = _farmer.transform.position;
		_endPositionFarmer = new Vector3 (_farmer.transform.position.x + _distanceToMoveFarmer, _farmer.transform.position.y, _farmer.transform.position.z);
		
		_rice.transform.position = new Vector3 (_rice.transform.position.x,
			0,
			_rice.transform.position.z);
		_distanceToMoveRice = Screen.height * 0.475f;
		_startPositionRice = _rice.transform.position;
		_endPositionRice = new Vector3 (_rice.transform.position.x, _rice.transform.position.y + _distanceToMoveRice, _rice.transform.position.z);
	}


	
	// Update is called once per frame
	void Update ()
	{
		float distCovered = (Time.time - _startTimeFarmer) * (_distanceToMoveFarmer / farmerMovementDuration);
		float fracJourney = distCovered / _distanceToMoveFarmer;
		_farmer.transform.position = Vector3.Lerp (_startPositionFarmer, _endPositionFarmer, fracJourney);

		if ((Time.time - _startTimeFarmer) >= _delayRice) {
			distCovered = (Time.time - _startTimeRice) * (_distanceToMoveRice / riceMovementDuration);
			fracJourney = distCovered / _distanceToMoveRice;
			_rice.transform.position = Vector3.Lerp (_startPositionRice, _endPositionRice, fracJourney);
		}
	}
}
