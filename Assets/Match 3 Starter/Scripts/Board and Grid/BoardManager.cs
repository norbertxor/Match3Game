using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
	public List<Sprite> characters = new List<Sprite>();
	public GameObject tile;
	public int xSize, ySize;

	private GameObject[,] _tiles;

	public bool IsShifting { get; set; }

	void Start () {
		instance = GetComponent<BoardManager>();

		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

	private void CreateBoard (float xOffset, float yOffset) {
		_tiles = new GameObject[xSize, ySize];
		Sprite[] previousLeft = new Sprite[ySize];
		Sprite previousBelow = null;

        float startX = transform.position.x;
		float startY = transform.position.y;

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				GameObject newTile = Instantiate(tile, 
					new Vector3(startX + (xOffset*x),
						startY + (yOffset*y),
						0),
					tile.transform.rotation);
				_tiles[x, y] = newTile;
				newTile.transform.parent = transform;
				List<Sprite> possibleCharacters = new List<Sprite>();
				//виключаю можливість створення одинакових тайлів при створенні ігрового поля
				possibleCharacters.AddRange(characters);
				possibleCharacters.Remove(previousLeft[y]);
				possibleCharacters.Remove(previousBelow);
				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
				previousLeft[y] = newSprite;
				previousBelow = newSprite;
			}
        }
    }

}
