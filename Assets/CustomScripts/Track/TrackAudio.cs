using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackAudio : MonoBehaviour {
    AudioClip[] audioList;
    AudioSource trackAudioSource;
    int trackNumber = 0;


	// Use this for initialization
	void Start () {
        trackAudioSource = GetComponent<AudioSource>();
        Scene scene = SceneManager.GetActiveScene();
        audioList = LoadAudioClipList(scene);
    }

    AudioClip[] LoadAudioClipList(Scene scene)
    {
        string listPath = "/Soundtrack/Lists/";
        string line;
        List<string> soundTrack = new List<string>();
        System.IO.StreamReader file =
            new System.IO.StreamReader(Application.dataPath + listPath + scene.name);
        while ((line = file.ReadLine()) != null)
        {
            soundTrack.Add("Soundtrack/" + line);
        }
        file.Close();
        AudioClip[] audioClipArray = new AudioClip[soundTrack.Count];
        int arrayIndex = 0;
        foreach (string s in soundTrack)
        {
            audioClipArray[arrayIndex] = Resources.Load<AudioClip>(s);
            arrayIndex++;
        }
        return audioClipArray;
    }

    // Update is called once per frame
    void Update () {
        if (!trackAudioSource.isPlaying)
        {
            trackAudioSource.PlayOneShot(audioList[trackNumber], 1.0F);
            trackNumber = (trackNumber + 1) % (audioList.Length);
        }
	}
}
