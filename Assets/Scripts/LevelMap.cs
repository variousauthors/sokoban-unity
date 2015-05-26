using UnityEngine;
using System.Collections;

public class LevelMap : MonoBehaviour {

	public GameObject FloorTile;
	public GameObject WallTile;
	public GameObject Player;
	public GameObject Target;

	public int rows;
	public int cols;
	public char[,] grid;
	
	public void Init(int rows, int cols, string data) {
		this.rows = rows;
		this.cols = cols;
		this.grid = new char[this.cols, this.rows];

		data = data.Replace ("\n", "");

		for (int i = 0; i < this.rows; i++) {
			for (int j = 0; j < this.cols; j++) {
				char type = data[i*this.cols + j];
				this.grid[j, i] = type;
			}
		}
	}

	// returns the class of object to instantiate for the given indices
	public GameObject GetTileClass(int row, int col) {
		char type = this.grid[row, col];
		GameObject toInstantiate = null;

		if (type == '#') {
			toInstantiate = WallTile;
		} else if (type == '.') {
			toInstantiate = FloorTile;
		} else if (type == 'P') {
			toInstantiate = Player;
		} else if (type == '+') {
			toInstantiate = Target;
		}
		
		return toInstantiate;
	}
}
