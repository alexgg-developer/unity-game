using System;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner
{
	private GameObject[] _cloud;
	private const uint NUMBER_CLOUDS = 6;
    private const uint NUMBER_OF_INSTANCES = 40;
    private float _defaultZ;
	private List<Cloud> _cloudInstance;
	private Vector2 _movementDirection;
	//private Vector2 _AB, _AC, _AD, _A; //makes for the rombus created by the chunks
	private Vector2 _AC;
    private List<double> m_terrainBoundaries = new List<double>();
    private uint m_terrainRows;
    private uint m_terrainCols;

    public CloudSpawner (float defaultZ)
	{
		_cloud = new GameObject[NUMBER_CLOUDS];
		try {		
			for(uint i = 1; i <= NUMBER_CLOUDS; ++i) { 
				_cloud[i-1]  = Resources.Load<GameObject>("Textures/decorations/cloud_0" + i);		
			}
		}
		catch(Exception e) {
			Debug.LogError (e.ToString());
		}
		_defaultZ = defaultZ;
		createInitialInstances ();
		GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ().addListenerToMultiplierChange (this.multiplierChanged);
	}

	/*
	 * Creates clouds into the rhombus limited by A, B, D, being A the left-most point an defining the other points CCW
	 * Note that C is not needed to calculate the points of the rhombus http://stackoverflow.com/questions/240778/random-points-inside-a-polygon
	 */
	private void createInitialInstances()
	{
		_cloudInstance = new List<Cloud> ();
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
        m_terrainRows = worldTerrain.getNumTilesY ();
        m_terrainCols = worldTerrain.getNumTilesX ();

		//Vector3 A = worldTerrain.getTileWorldPosition(0   ,0);
		//Vector3 B = worldTerrain.getTileWorldPosition(m_terrainRows,0);
		//Vector3 C = worldTerrain.getTileWorldPosition(m_terrainRows,m_terrainCols);
		//Vector3 D = worldTerrain.getTileWorldPosition(0   ,m_terrainCols);
        for(uint i = 0; i < m_terrainCols; ++i) {
            Vector3 pos = worldTerrain.getTileWorldPosition(m_terrainRows, i);
            m_terrainBoundaries.Add(pos.x);
        }
        

		//_AB = new Vector2(B.x -A.x, B.y - A.y);
		//_AC = new Vector2(C.x -A.x, C.y - A.y);
		//_AD = new Vector2(D.x -A.x, D.y - A.y);
		//_movementDirection = new Vector2(_AC.x, _AC.y);
		//_movementDirection.Normalize ();
		//_A = new Vector2 (A.x, A.y);
		/*Debug.Log ("A::" + initialPoint.ToString ());
		Debug.Log ("B::" + B.ToString ());
		Debug.Log ("D::" + D.ToString ());
		Debug.Log ("AB::" + AB.ToString ());
		Debug.Log ("AD::" + AD.ToString ());*/
		for (uint i = 0; i < NUMBER_OF_INSTANCES; ++i) {			
			GameObject cloud = _cloud[(int)UnityEngine.Random.Range(0.0f, (float)NUMBER_CLOUDS)];
            //float u = UnityEngine.Random.value;
            //float v = UnityEngine.Random.value;
            //Vector2 randomPoint = _A + u * _AB + v * _AD;
            uint randomCol = (uint)UnityEngine.Random.Range(0.0f, m_terrainCols);
            uint randomRow = (uint)UnityEngine.Random.Range(0.0f, m_terrainRows);
            uint minNumber = Math.Min(randomCol, randomRow);
            uint firstCol = randomCol - minNumber;
            uint firstRow = randomRow - minNumber;
            Vector3 randomPoint = worldTerrain.getTileWorldPosition(randomRow, randomCol);
            uint minEndPoint = Math.Min(m_terrainRows - firstRow, m_terrainCols - firstCol);
            Vector3 endPoint = worldTerrain.getTileWorldPosition(firstRow + minEndPoint, firstCol + minEndPoint);
            
            //arriba es y = 54
            //abajo x = 42          
            //GameObject instance = ((GameObject)GameObject.Instantiate(cloud, new Vector3(randomPoint.x, randomPoint.y, _defaultZ), Quaternion.identity));
            GameObject instance = ((GameObject)GameObject.Instantiate(cloud, new Vector3(randomPoint.x, randomPoint.y, _defaultZ), Quaternion.identity));
            _cloudInstance.Add(new Cloud(instance, this, i, endPoint.x));
		}
	}

	/* no references
	public Vector2 getMovementDirection()
	{
		return _movementDirection;
	}
	*/

	public void update (float dt)
	{
		for(uint i = 0; i < _cloudInstance.Count; ++i) {
			_cloudInstance[(int)i].update(dt);
		}
	}

	public void repositionCloud(uint id)
    {
        WorldTerrain worldTerrain = WorldTerrain.GetInstance();
        uint randomCol = (uint)UnityEngine.Random.Range(0.0f, m_terrainCols);
        uint randomRow = (uint)UnityEngine.Random.Range(0.0f, m_terrainRows);
        uint minNumber = Math.Min(randomCol, randomRow);
        uint firstCol = randomCol - minNumber;
        uint firstRow = randomRow - minNumber;
        Vector3 randomPoint = worldTerrain.getTileWorldPosition(randomRow, randomCol);
        uint minEndPoint = Math.Min(m_terrainRows - firstRow, m_terrainCols - firstCol);
        Vector3 endPoint = worldTerrain.getTileWorldPosition(firstRow + minEndPoint, firstCol + minEndPoint);
        _cloudInstance[(int)id].setPosition(new Vector3(randomPoint.x, randomPoint.y, _defaultZ));
        _cloudInstance[(int)id].EndPoint = endPoint.x;
    }

	public void multiplierChanged()
	{
		float multiplier = GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ().getCurrentMultiplier ();
		for(uint i = 0; i < _cloudInstance.Count; ++i) {
			_cloudInstance[(int)i].changeMultiplier(multiplier);
		}
	}
}


