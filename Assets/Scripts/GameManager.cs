using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public List<TextAsset> raw_levels;

	private XmlDocument raw_level;
	private int level = 0;

	private LevelMap map;
	private GameObject[] tiles;
	private List<int> _targets; // a list of the indices in tiles where targets is
	private int[] targets;
	private GameObject player;
	private bool victory;

	void Awake () {
		Debug.Log ("GameManager Awake");
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		
		DontDestroyOnLoad (gameObject);
		InitGame ();
	}
	
	private void OnLevelWasLoaded (int index) {
		InitGame ();
	}

	// call this from elsewhere
	public void GameOver() {}
	
	void InitGame () {
		// read the map data from a file
		raw_level = new XmlDocument();
		raw_level.LoadXml(raw_levels[level].text);
		
		int rows =  int.Parse(raw_level.FirstChild.Attributes["rows"].Value);
		int cols =  int.Parse(raw_level.FirstChild.Attributes["cols"].Value);
		map = GetComponent<LevelMap>();
		map.Init(rows, cols, raw_level.InnerText);

		// use the map data to create tile objects for the visual map
		tiles = new GameObject[map.cols * map.rows];
		_targets = new List<int> ();

		// populate the array of tiles
		for (int x = 0; x < map.cols; x++) {
			for (int y = 0; y < map.rows; y++) {
				GameObject toInstantiate = map.GetTileClass(x, y);
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				if (instance.tag == "Player") {
					Debug.Log("player");
					player = instance;
										
				} else if (instance.tag == "Target") {
					_targets.Add(y*map.cols + x);
				}

				tiles[y*map.cols + x] = instance;

				// every index has a floor tile
				toInstantiate = map.FloorTile;
				instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
					
				// this is supposed to be useful to clear up clutter but I don't get it
				//instance.transform.SetParent (mapHolder);
			}
		}

		targets = _targets.ToArray ();
		Camera.main.backgroundColor = Color.magenta;
		victory = false;
	}

	// Update is called once per frame
	void Update () {
		int horizontal = 0;
		int vertical = 0;
		
		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");
		
		if (horizontal != 0) {
			vertical = 0;
		}
		
		if (horizontal != 0 || vertical != 0) {
			AttemptMove (horizontal, vertical);
		}
	}

	void AttemptMove (int xDir, int yDir) {
		// we are subtracting the yDir because it is given to us backwards (and because we are foolish and brash)
		int px = (int)player.transform.position.x;
		int py = (int)player.transform.position.y;

		int nx = px + xDir;
		int ny = py + yDir;

		int new_tile = ny * map.cols + nx;
		int player_tile = py * map.cols + px;

		if (0 > new_tile || new_tile >= tiles.Length) {
			return;
		}

		GameObject neighbour = tiles[new_tile];
		GameObject tmp;

		if (neighbour != null && neighbour.tag == "Wall") {
			// check beyond the wall
			int far_tile = (ny + yDir) * map.cols + (nx + xDir);

			if ((far_tile > -1) && (far_tile < tiles.Length) && (tiles[far_tile].tag != "Wall")) {
				// we can push!
				GameObject far_neighbour = tiles[far_tile];

				// push the wall
				SwapTiles(new_tile, far_tile);
				neighbour.transform.position = new Vector3(nx + xDir, ny + yDir, 0);

				// move the player
				SwapTiles(player_tile, new_tile);
				player.transform.position = new Vector3(nx, ny, 0);

				// a wall moved so check for victory
				CheckVictory();
			}

			return;
		} else {
			// move the player, and swap the player and the floor tile

			player.transform.position = new Vector3(nx, ny, 0);
			SwapTiles(player_tile, new_tile);
		}
	}

	void SwapTiles (int tile_a, int tile_b) {
		GameObject tmp = tiles[tile_a];
		tiles [tile_a] = tiles[tile_b];
		tiles [tile_b] = tmp;
	}

	void SwapSprites (GameObject sprite_a, GameObject sprite_b) {
		// move the sprites
		Vector3 tmp = sprite_a.transform.position;
		sprite_a.transform.position = sprite_b.transform.position;
		sprite_b.transform.position = tmp;

	}

	void CheckVictory () {
		victory = true;

		for (int i = 0; i < targets.Length; i++) {
			int target = targets[i];

			// if there is a target in bounds with no wall on it, then we are not winning
			if (target > -1 && target < tiles.Length && tiles[target].tag != "Wall") {
				victory = false;
			}
		}

		Debug.Log(victory);
	}
	
	
	
}
