using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackObject : MonoBehaviour
{
    [SerializeField] FeedbackObjectPoolTag feedbackPoolTag;
    public FeedbackObjectPoolTag GetFeedbackPoolTag { get { return feedbackPoolTag; } }

    [SerializeField] ParticleSystem fx;
    [SerializeField] AudioSource soundSource;
    [SerializeField] Sound sound;

    public void StartFeedback(float size, float volume)
    {
        gameObject.SetActive(true);
        fx.Play();
        soundSource.PlaySound(sound);
        transform.localScale = Vector3.one * size;
    }

    private void Update()
    {
        if (ReadyToBeReturnedToPool)
        {
            GameManager.gameManager.PoolManager.ReturnFeedbackObject(this);
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;
        }
    }

    public bool ReadyToBeReturnedToPool
    {
        get
        {
            return !fx.isPlaying && !soundSource.isPlaying;
        }
    }
}
