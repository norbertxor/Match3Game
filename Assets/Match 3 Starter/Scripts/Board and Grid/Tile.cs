using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer _render;
	private bool _isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		_render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
		_isSelected = true;
		_render.color = selectedColor;
		transform.localScale = new Vector3(.35f, .35f, .35f);
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		_isSelected = false;
		transform.localScale = new Vector3(.3f,.3f,.3f);
		_render.color = Color.white;
		previousSelected = null;
	}

	private void OnMouseDown() {
		if (_render.sprite == null || BoardManager.instance.IsShifting) return;
		if(_isSelected) Deselect();
		else {
				if(previousSelected == null) Select();
				else {
					SwapSprite(previousSelected._render);
					previousSelected.Deselect();
				}
		}
	}

	public void SwapSprite(SpriteRenderer forSwap) {
		if (_render.sprite == forSwap.sprite) return;
		(forSwap.sprite, _render.sprite) = (_render.sprite, forSwap.sprite);
		SFXManager.instance.PlaySFX(Clip.Swap);

	}

}