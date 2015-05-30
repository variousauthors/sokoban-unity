using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public List<TextAsset> raw_levels;
	public LevelManager levelManager;

	private int level = 0;

	private GameObject[] tiles;
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

		levelManager = GetComponent<LevelManager> ();
		levelManager.Init (raw_levels);
		levelManager.NextLevel(out tiles, out targets, out player);
	
		Camera.main.backgroundColor = Color.magenta;
		victory = false;
	}

	// Update is called once per frame
	void Update () {
		int horizontal = 0;
		int vertical = 0;

		if (victory) {
			levelManager.NextLevel(out tiles, out targets, out player);
			victory = false;
		} else {
			horizontal = (int)Input.GetAxisRaw ("Horizontal");
			vertical = (int)Input.GetAxisRaw ("Vertical");
			
			if (horizontal != 0) {
				vertical = 0;
			}
			
			if (horizontal != 0 || vertical != 0) {
				AttemptMove (horizontal, vertical);
			}
		}
	}

	void AttemptMove (int xDir, int yDir) {
		// we are subtracting the yDir because it is given to us backwards (and because we are foolish and brash)
		int px = (int)player.transform.position.x;
		int py = (int)player.transform.position.y;

		int nx = px + xDir;
		int ny = py + yDir;

		int new_tile = ny * levelManager.cols + nx;
		int player_tile = py * levelManager.cols + px;

		if (0 > new_tile || new_tile >= tiles.Length) {
			return;
		}

		GameObject neighbour = tiles[new_tile];

		if (neighbour != null && neighbour.tag == "Wall") {
			// check beyond the wall

			int far_tile = (ny + yDir) * levelManager.cols + (nx + xDir);

			if ((far_tile > -1) && (far_tile < tiles.Length) && (tiles[far_tile].tag != "Wall")) {
				// we can push!

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
