using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHanlder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public AudioSource buttonSound;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public void AssignValues(AudioSource audioSource, AudioClip click, AudioClip hover)
    {
        buttonSound = audioSource;
        clickSound = click;
        hoverSound = hover;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonSound.PlayOneShot(clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonSound.PlayOneShot(hoverSound);
    }
}
