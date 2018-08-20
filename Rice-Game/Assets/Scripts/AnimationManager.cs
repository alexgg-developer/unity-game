using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChunkAnimation {
	public GameObject instance;
	public int actionDuration; //in hours
	public float speed;
	public Vector3 chunkAnimationDirection;
	public float remainingDistance;
	public ChunkAnimation(string animation, int duration)
	{
		instance = null;
		actionDuration = duration;
		speed = 0.2f;
		chunkAnimationDirection = Vector3.zero;
		remainingDistance = 0.0f;
		try {		
			//MODIFY THIS: CALCULATE SPEED
			GameObject animationGameObject = Resources.Load<GameObject> (animation);
			instance = (GameObject)GameObject.Instantiate(animationGameObject, new Vector3(0, 0, 0), Quaternion.identity);
		}
		catch(Exception e) {
			Debug.LogError ("WorldTerrain::startActionAnimationInAChunk::Exception::" + e.Message);
		}
	}

	public void calculateSpeed(float distance)
	{
		TimeManager timeManager = GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ();
		speed = (distance / (actionDuration * timeManager.getTimePerHour ()));
		//remainingDistance = distance;
	}

	public void setRemainingDistance(float distance)
	{
		remainingDistance = distance;
	}

	public void setPosition(Vector3 pos)
	{
		instance.transform.Translate (pos);
	}

	public void setDirection(Vector3 direction)
	{
		chunkAnimationDirection = direction; 
	}
	public void calculateRemainingDistance(float distanceRun)
	{
		remainingDistance = remainingDistance - distanceRun;
	}
}
public class AnimationManager {
	
	// SINGLETON
	private static AnimationManager instance = null;
	private Dictionary<int, ChunkAnimation> _currentActionAnimation; //current animation in a chunk
	TimeManager _timeManager;

	public static AnimationManager GetInstance()
	{
		if(instance == null)
		{
			instance = new AnimationManager();
		}
		
		return instance;
	}

	private AnimationManager() 
	{
		_currentActionAnimation = new Dictionary<int, ChunkAnimation> ();
		_timeManager = GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ();
	}

	public void addChunkAnimation(int chunk, ChunkAnimation animation)
	{
		_currentActionAnimation.Add (chunk, animation);
	}

    public ChunkAnimation getCurrentAnimationInAChunk(int chunk)
    {
        if (_currentActionAnimation.ContainsKey(chunk)) {
            return _currentActionAnimation[chunk];
        }

        return new ChunkAnimation("void", 0);
    }

	public void removeChunkAnimation(int chunk)
	{
		if (_currentActionAnimation.ContainsKey (chunk)) {
			ChunkAnimation animation = _currentActionAnimation [chunk];
			GameObject.Destroy (animation.instance);
			_currentActionAnimation.Remove (chunk);
		}
	}

	public void update(float dt)
	{
		List<int> keys = new List<int> (_currentActionAnimation.Keys);
		foreach (int key in keys) {
			if (_currentActionAnimation[key].remainingDistance > 0.0f) {
				float distanceRun = _currentActionAnimation[key].speed * dt * _timeManager.getCurrentMultiplier();
				_currentActionAnimation[key].setPosition (_currentActionAnimation[key].chunkAnimationDirection * distanceRun);
				_currentActionAnimation[key].calculateRemainingDistance(distanceRun);
			} 
			else {
				removeChunkAnimation (key);
			}
		}
	}

}
