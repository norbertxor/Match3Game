using UnityEngine;

public enum Clip { Select, Swap, Clear };

public class SFXManager : MonoBehaviour {
	public static SFXManager instance;

	private AudioSource[] _sfx;

	// Use this for initialization
	void Start () {
		instance = GetComponent<SFXManager>();
		_sfx = GetComponents<AudioSource>();
    }

	public void PlaySFX(Clip audioClip) {
		_sfx[(int)audioClip].Play();
	}
}
