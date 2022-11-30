using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Utils : MonoBehaviour
{
    public static Utils instance;
    private AudioSource buttonSound;
    public AudioClip[] sounds;

    private void Awake()
    {
        if (instance != null)
        {
            return; //Do nothing, session handler destroy the object.
        }
        instance = this;
        buttonSound = GetComponent<AudioSource>();
    }
    private void Start() {
        AssingButtonSound();
    }
    public void AssingButtonSound()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            button.gameObject.AddComponent<ButtonHanlder>().AssignValues(buttonSound, sounds[0], sounds[1]);
        }
    }
}
