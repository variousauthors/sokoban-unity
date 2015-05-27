using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour {

	private LevelMap map;
	private XmlDocument xml;
	private List<int> _targets; // a list of the indices in tiles where targets is
	private GameObject tileParent;

	[HideInInspector] public int cols;
	[HideInInspector] public int rows;
	
	public void Init(XmlDocument xml) {
		this.xml = xml;
	}

	public void NextLevel(out GameObject[] tiles, out int[] targets, out GameObject player) {
		this.rows = int.Parse(xml.FirstChild.Attributes["rows"].Value);
		this.cols = int.Parse(xml.FirstChild.Attributes["cols"].Value);

		if (tileParent != null) {
			Destroy(tileParent);
		}

		tileParent = new GameObject ("Board");
		Transform transform = tileParent.transform;

		map = GetComponent<LevelMap>();
		map.Init(this.rows, this.cols, xml.InnerText);

		// player is an out param, but clearly this won't do
		player = new GameObject ();

		// use the map data to create tile objects for the visual map
		tiles = new GameObject[map.cols * map.rows];
		_targets = new List<int> ();
		
		// populate the array of tiles
		for (int x = 0; x < map.cols; x++) {
			for (int y = 0; y < map.rows; y++) {
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
