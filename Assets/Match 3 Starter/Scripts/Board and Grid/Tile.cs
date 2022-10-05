using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer _render;
	private bool _isSelected = false;
	private bool _matchFound = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		_render = GetComponent<SpriteRenderer>();
	}

	private void Select() {
		_isSelected          = true;
		_render.color        = selectedColor;
		transform.localScale = new Vector3(.35f, .35f, .35f);
		previousSelected     = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		_isSelected          = false;
		transform.localScale = new Vector3(.3f, .3f, .3f);
		_render.color        = Color.white;
		previousSelected     = null;
	}

	private void OnMouseDown() {
		if (_render.sprite == null || BoardManager.instance.IsShifting) return;
		if (_isSelected) Deselect();
		else {
			if (previousSelected == null) Select();
			else {
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject)) {
					SwapSprite(previousSelected._render);
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
				}
				else {
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}
		}
	}

	public void SwapSprite(SpriteRenderer forSwap) {
		if (_render.sprite == forSwap.sprite) return;
		(forSwap.sprite, _render.sprite) = (_render.sprite, forSwap.sprite);
		SFXManager.instance.PlaySFX(Clip.Swap);
		GUIManager.instance.MoveCounter--;
	}

	private GameObject GetAdjacent(Vector2 castDir) {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		if (hit.collider != null)
			return hit.collider.gameObject;
		return null;
	}

	private List<GameObject> GetAllAdjacentTiles() {
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		return adjacentTiles;
	}

	private List<GameObject> FindMatch(Vector2 castDir) {
		List<GameObject> matchingTiles = new List<GameObject>();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == _render.sprite) {
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}

		return matchingTiles;
	}

	private void ClearMatch(Vector2[] paths) {
		List<GameObject> matchingTiles = new List<GameObject>();
		for (int i=0; i<paths.Length; i++)
			matchingTiles.AddRange(FindMatch(paths[i]));
		if (matchingTiles.Count >= 2) {
			for (int i = 0; i < matchingTiles.Count; i++)
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			_matchFound = true;
		}
	}

	public void ClearAllMatches() {
		if (_render.sprite == null)
			return;
		
		ClearMatch(new Vector2[2]{Vector2.left, Vector2.right});
		ClearMatch(new Vector2[2]{Vector2.up, Vector2.down});
		if (_matchFound) {
			_render.sprite = null;
			_matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			SFXManager.instance.PlaySFX(Clip.Clear);
		}
	}

}  //end