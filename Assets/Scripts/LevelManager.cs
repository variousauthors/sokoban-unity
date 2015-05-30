using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour {

	private LevelMap map;
	private List<TextAsset> raw;
	private List<int> _targets; // a list of the indices in tiles where targets is
	private GameObject tileParent;
	private int level;

	[HideInInspector] public int cols;
	[HideInInspector] public int rows;
	
	public void Init(List<TextAsset> raw) {
		this.raw = raw;
		this.level = 0;
	}

	public void NextLevel(out GameObject[] tiles, out int[] targets, out GameObject player) {
		if (level >= raw.Count) {
			level = 0; // TODO more like, return false to let the game know there are no more levels
		}

		if (tileParent != null) {
			Destroy(tileParent);
		}

		tileParent = new GameObject ("Board");
		Transform transform = tileParent.transform;

		map = GetComponent<LevelMap>();
		map.Init(raw[level]);

		level++;

		this.rows = map.rows;
		this.cols = map.cols;

		// player is an out param, but clearly this won't do
		player = new GameObject ();

		// use the map data to create tile objects for the visual map
		tiles = new GameObject[map.cols * map.rows];
		_targets = new List<int> ();
		
		// populate the array of tiles
		for (int y = 0; y < map.rows; y++) {
			for (int x = 0; x < map.cols; x++) {
				GameObject toInstantiate = map.GetTileClass(x, y);
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;


				if (instance.tag == "Player") {
					Debug.Log("player: (" + instance.transform.position.x + ", " + instance.transform.position.y + ")");
					player = instance;
					
				} else if (instance.tag == "Target") {
					_targets.Add(y*map.cols + x);
				}
				
				tiles[y*map.cols + x] = instance;
				instance.transform.SetParent (transform);

				// every index has a floor tile
				toInstantiate = map.FloorTile;
				instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (transform);

				// this is supposed to be useful to clear up clutter but I don't get it
				//instance.transform.SetParent (mapHolder);
			}
		}

		targets = _targets.ToArray ();
	}
}
