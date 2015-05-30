using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelMap : MonoBehaviour {

	public GameObject FloorTile;
	public GameObject WallTile;
	public GameObject Player;
	public GameObject Target;

	public int rows;
	public int cols;
	public char[] grid;

	private string raw;

	public void Init(TextAsset raw) {
		string text = raw.text;
		int start = text.LastIndexOf ('-') + 2;
		start = (start == 1) ? 0 : start; // if frontmatter missing

		this.raw = text.Substring (start); // + 1 for the - and +1 for the \n

		Debug.Log (this.raw);

		if (text [text.Length - 1] != '\n') {
			throw new Exception("No new line at end of file!");
		}

		this.rows = 0;
		this.cols = 0;

		{
			int index = 0;
			
			while (index < this.raw.Length) {
				char ch = this.raw[index];
				
				if (ch == '\n') {
					this.rows++;
				}
				
				index++;
			}
		}

		this.grid = this.raw.Replace ("\n", "").ToCharArray();
		this.cols = this.grid.Length / this.rows;
	}

	// returns the class of object to instantiate for the given indices
	// here is where we flip things: we need to read the grid upside-down
	// to jive with Unity's y-axis
	public GameObject GetTileClass(int col, int row) {
		row = this.rows - row - 1;

		char type = this.grid[row*this.cols + col];
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
