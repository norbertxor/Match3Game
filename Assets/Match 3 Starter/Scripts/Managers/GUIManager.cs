using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public static GUIManager instance;

	public GameObject gameOverPanel;
	public Text       yourScoreTxt;
	public Text       highScoreTxt;

	public Text scoreTxt;
	public Text moveCounterTxt;

	private int _score;
	private int _moveCounter;

	public int Score {
		get { return _score; }
		set {
			_score        = value;
			scoreTxt.text = _score.ToString();
		}
	}

	public int MoveCounter {
		get { return _moveCounter; }
		set {
			_moveCounter        = value;
			if (_moveCounter <= 0) {
				_moveCounter = 0;
				StartCoroutine(WaitForShifting());
			}
			moveCounterTxt.text = _moveCounter.ToString();
		}
	}
	void Awake() {
		_moveCounter = 10;
		moveCounterTxt.text = _moveCounter.ToString();
		instance = GetComponent<GUIManager>();
	}

	public void GameOver() {
		GameManager.instance.gameOver = true;

		gameOverPanel.SetActive(true);

		if (_score > PlayerPrefs.GetInt("HighScore")) {
			PlayerPrefs.SetInt("HighScore", _score);
			highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		} else {
			highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		}

		yourScoreTxt.text = _score.ToString();
	}

	private IEnumerator WaitForShifting() {
		yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
		yield return new WaitForSeconds(.25f);
		GameOver();
	}

} //end
