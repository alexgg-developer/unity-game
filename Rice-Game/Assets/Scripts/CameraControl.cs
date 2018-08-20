using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{	
	public const float CAMERA_SPEED_MOVING = 1.0f; // velocidad de desplazar con el dedo
	public const float CAMERA_SPEED_TO_TILE = 0.8f; // velocidad de desplazamiento automatico
	public const float DIST_UMBRAL = 4.5f;
	public const float LIMIT = 100.0f;
	public const float MAX_ZOOM = 25.0f;
	public const float MIN_ZOOM = 5.0f;
	public const float MAX_X = 150.0f;
	public const float MIN_X = 20.0f;
	public const float MAX_Y = 40.0f;
	public const float MIN_Y = -30.0f;
	public const float CAMERA_Y_CENTER_DESPLAZAMIENTO = 0.80f; // porcentaje 0~1
	private Vector3 lastMouseDownPos;
	private Vector3 lastMousePos, finalTouchPos;
	private Vector3 targetPos, camTargetPos;
	private bool hasTarget;

	private bool isMouseButtonPressed, isTouchPressed;

	void Start ()
	{
		//CAMERA_SPEED_MOVING = 1.0f; // velocidad de desplazar con el dedo
		//CAMERA_SPEED_TO_TILE = 0.8f; // velocidad de desplazamiento automatico
		//DIST_UMBRAL = 0.3f;
		//MAX_ZOOM = 25.0f;
		//MIN_ZOOM = 5.0f;
		//MAX_X = 160.0f;
		//MIN_X = 10.0f;
		//MAX_Y = 50.0f;
		//MIN_Y = -40.0f;
		//targetPos = Vector3.;
		hasTarget = false;
		lastMousePos = Input.mousePosition;
		finalTouchPos = lastMousePos;
		isMouseButtonPressed = false;
		isTouchPressed = false;
	}

	void Update ()
	{
		if (hasTarget) {
			Vector3 dirToTarget = camTargetPos - gameObject.transform.position;
			//Debug.Log(dirToTarget);
			if (dirToTarget.magnitude <= CAMERA_SPEED_TO_TILE) {
				// Objetivo Alcanzado
				gameObject.transform.position = camTargetPos;
				hasTarget = false;
				GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>().cameraArriveSelection();
			}
			else {
				gameObject.transform.position += dirToTarget.normalized*CAMERA_SPEED_TO_TILE;
			}
		} else {
			if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject (Input.touches [0].fingerId) && !isTouchPressed) {
				isTouchPressed = true;
				lastMouseDownPos = Input.touches[0].position;
			}
			else if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject (Input.touches [0].fingerId) && isTouchPressed) {
				finalTouchPos = Input.touches[0].position;
			}
			else if (Input.touchCount == 0 && Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject ()) {
				isMouseButtonPressed = true;
				lastMouseDownPos = Input.mousePosition;
				//Debug.Log("MouseButtonDown on "+Input.mousePosition.x+" "+Input.mousePosition.y);
			} else if (Input.touchCount == 0 && isTouchPressed) {
				float distTotal = (finalTouchPos - lastMouseDownPos).magnitude;
				if (distTotal < DIST_UMBRAL) {
					// CLICK
					Vector3 newTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					//Debug.Log ("CLICK On: " + newTargetPos.x + ", " + newTargetPos.y);
					setNewTarget (newTargetPos.x, newTargetPos.y);

					GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>().clickOn(targetPos.x, targetPos.y);
					//WorldTerrain worldTerrain = WorldTerrain.GetInstance();
					//Debug.Log("Matrix i: "+worldTerrain.getMatrixRow(targetPos.x, targetPos.y));
					//Debug.Log("Matrix j: "+worldTerrain.getMatrixCol(targetPos.x, targetPos.y));
				}
				isTouchPressed = false;
			} else if (Input.GetMouseButtonUp (0) && isMouseButtonPressed) {
				float distTotal = (Input.mousePosition - lastMouseDownPos).magnitude;
				if (distTotal < DIST_UMBRAL) {
					// CLICK
					Vector3 newTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					//Debug.Log ("CLICK On: " + newTargetPos.x + ", " + newTargetPos.y);
					setNewTarget (newTargetPos.x, newTargetPos.y);

					GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>().clickOn(targetPos.x, targetPos.y);
					//WorldTerrain worldTerrain = WorldTerrain.GetInstance();
					//Debug.Log("Matrix i: "+worldTerrain.getMatrixRow(targetPos.x, targetPos.y));
					//Debug.Log("Matrix j: "+worldTerrain.getMatrixCol(targetPos.x, targetPos.y));
				}
				isMouseButtonPressed = false;
			} 
				
			// Touch cam
			if (isTouchPressed) {
				Vector3 touchDelta = Input.touches [0].deltaPosition;
				touchDelta.z = 0;
				Vector3 finalPos = this.gameObject.transform.position - (touchDelta * 0.015f*Camera.main.orthographicSize);
				float yMid = (MAX_Y + MIN_Y) / 2;
				finalPos.y = Mathf.Clamp (finalPos.y, MIN_Y, MAX_Y);
				finalPos.x = Mathf.Clamp (finalPos.x, MIN_X+Mathf.Abs(finalPos.y-yMid)*1.65f, MAX_X-Mathf.Abs(finalPos.y-yMid)*1.65f);
				this.gameObject.transform.position = finalPos;
			}
			else if (isMouseButtonPressed) {
				Vector3 mouseDelta = (Input.mousePosition - lastMousePos);
				mouseDelta.z = 0;
				Vector3 finalPos = this.gameObject.transform.position - (mouseDelta * 0.05f);
				float yMid = (MAX_Y + MIN_Y) / 2;
				finalPos.y = Mathf.Clamp (finalPos.y, MIN_Y, MAX_Y);
				finalPos.x = Mathf.Clamp (finalPos.x, MIN_X+Mathf.Abs(finalPos.y-yMid)*1.65f, MAX_X-Mathf.Abs(finalPos.y-yMid)*1.65f);
				this.gameObject.transform.position = finalPos;
			}

            // ZOOM
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + 1, MAX_ZOOM);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - 1, MIN_ZOOM);
            }
		}

	

		lastMousePos = Input.mousePosition;
	}

	void clampCam(){
		camTargetPos.x = Mathf.Clamp (camTargetPos.x, MIN_X, MAX_X);
		camTargetPos.y = Mathf.Clamp (camTargetPos.y, MIN_Y, MAX_Y);
	}

	public void setNewTarget(float x, float y) {
		float cameraSize = Camera.main.orthographicSize;
		targetPos.x = x;
		targetPos.y = y;
		targetPos.z = gameObject.transform.position.z;
		camTargetPos.x = targetPos.x;
		camTargetPos.y = targetPos.y + cameraSize * CAMERA_Y_CENTER_DESPLAZAMIENTO;
		camTargetPos.z = targetPos.z;
		clampCam ();
		hasTarget = true;
	}

	
}

