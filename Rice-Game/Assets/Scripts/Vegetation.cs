using UnityEngine;
using System;
using System.Collections.Generic;

public class Vegetation {
	public enum VegType {TREE, PLANT}; // -----------

	static Dictionary<VegID, GameObject> resources = null;

	GameObject representation;
    private VegID m_currentVegetation;
    public VegID CurrentVegetation
    {
        get
        {
            return m_currentVegetation;
        }
        private set
        {
            m_currentVegetation = value;
        }
    }
	//VegType vegType;

	public Vegetation(uint i, uint j, VegID vegID) {
		if (resources == null) {
			initResources ();
		}

		//vegType = (vegID == VegID.tree_0 || vegID == VegID.tree_1) ? VegType.TREE : VegType.PLANT;
		//this.vegID = vegID;
		initRepresentation (i, j, vegID);
	}

	private void initResources() {
		resources = new Dictionary<VegID, GameObject> ();

		foreach (VegID id in Enum.GetValues(typeof(VegID))) {
			string spritePath = "Textures/decorations/nature_"+id.ToString();
			try {
				resources[id] = Resources.Load<GameObject> (spritePath);
			} catch (Exception e) {
				Debug.LogError (e.ToString ());
			}
			Debug.Assert (resources[id] != null, "Nature not loaded: " + spritePath);
		}
	}

	private void initRepresentation(uint i, uint j, VegID id) {
		Debug.Assert (resources.ContainsKey (id), "Nature not loaded: " + id);
		GameObject prefab = resources[id];

		WorldTerrain wt = WorldTerrain.GetInstance ();
		float map_top = wt.getTileWorldPositionY (wt.getNumTilesY () - 1, 0);
		float map_h = wt.getTileWorldPositionY (0, wt.getNumTilesX () - 1) - wt.getTileWorldPositionY (wt.getNumTilesY () - 1, 0);

		Vector3 size = prefab.GetComponent<SpriteRenderer> ().bounds.size;
		Vector3 pos = wt.getTileWorldPosition (i, j);
		pos.y += size.y / 2 - WorldTerrain.TILE_HEIGHT_UNITS/2;
		pos.z = WorldTerrain.PLANTS_Z_LAYER - (map_top - (pos.y))/map_h;

		representation = (GameObject) GameObject.Instantiate(prefab, pos, Quaternion.identity);
        m_currentVegetation = id;

    }

	public void delete() {
		//Debug.Log ("Delete Vegetation!");
		GameObject.Destroy (representation);
	}

	public void markToDelete() {
		representation.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public void unmarkToDelete() {
		representation.GetComponent<SpriteRenderer>().color = Color.white;
	}
}
