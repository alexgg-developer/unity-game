using System;
using UnityEngine;

public class Cloud
{
	private GameObject _instance;
	private CloudSpawner _parent;
	private const float SPEED = 0.2f;
	private uint _id;
	private float _speedMultiplier;
    private float m_imgWidth;
    private double m_endPoint;
    public double EndPoint
    {
        get
        {
            return m_endPoint;
        }
        set
        {
            m_endPoint = value;
        }
    }

	public Cloud (GameObject instance, CloudSpawner parent, uint id, double endPoint)
	{
		_instance = instance;
        m_imgWidth = _instance.GetComponent<SpriteRenderer>().sprite.rect.width;
        m_imgWidth = m_imgWidth / _instance.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        _parent = parent;
		_id = id;
		_speedMultiplier = 1.0f;
        m_endPoint = endPoint;
	}

	public void update(float dt)
	{
        _instance.transform.Translate (SPEED * _speedMultiplier * dt, 0, 0);
		if (_instance.transform.position.x >= m_endPoint - m_imgWidth) {
			_parent.repositionCloud(_id);
		}
	}

	public void setPosition(Vector3 pos)
	{
		_instance.transform.position = pos;
	}

	public void changeMultiplier(float multiplier)
	{
		_speedMultiplier = multiplier;
	}
}

