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

	public IEnumerator FindNullTiles() {
		for(int x = 0; x < xSize; x++)
			for(int y = 0; y<ySize; y++)
				if (_tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) {
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
		
		for(int x=0; x<xSize; x++)
			for(int y=0; y<ySize; y++)
				_tiles[x,y].GetComponent<Tile>().ClearAllMatches();
	}
	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = 0.2f) {
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < ySize; y++) {
			SpriteRenderer render = _tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
				nullCount++;
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++) {
			GUIManager.instance.Score += 1;
			yield return new WaitForSeconds(shiftDelay);
			for (int k = 0; k < renders.Count - 1; k++) {
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
			}
		}
		
		IsShifting = false;
	}

	private Sprite GetNewSprite(int x, int y) {
		List<Sprite> possibleCharacter = new List<Sprite>();
		possibleCharacter.AddRange(characters);

		if (x > 0)
			possibleCharacter.Remove(_tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		
		if(x < xSize - 1)
			possibleCharacter.Remove(_tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		
		if(y > 0)
			possibleCharacter.Remove(_tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);

		return possibleCharacter[Random.Range(0, possibleCharacter.Count)];
	}

} //end
