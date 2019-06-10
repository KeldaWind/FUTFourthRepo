using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPreview : Preview
{
    [SerializeField] Animator anchorAnimator;
    [SerializeField] AudioSource anchorSoundSource;
    [SerializeField] Sound anchorStartSound;
    [SerializeField] Sound anchorEndSound;
    Transform shipToFollow;
    bool previewing;
    bool active;
    float duration;

    private void Update()
    {
        if (previewing)
            UpdatePreparePreview(Vector3.zero, Vector3.zero);
        else if (active)
            UpdateLaunchedPreview();
        else if (ReadyToBeReturned)
            EndAnchor();
    }

    public void ShowPreparePreview(Transform transformToFollow)
    {
        gameObject.SetActive(true);
        previewing = true;
        shipToFollow = transformToFollow;
    }

    public override void UpdatePreparePreview(Vector3 originPos, Vector3 aimedPos)
    {
        transform.position = shipToFollow.position;
    }

    public override void StartLaunchedPreview(Vector3 startPos, List<Vector3> aimPos, float parameter)
    {
        previewing = false;
        anchorAnimator.SetTrigger("started");
        active = true;
        duration = parameter;

        anchorSoundSource.PlaySound(anchorStartSound);
    }

    public override void UpdateLaunchedPreview()
    {
        transform.position = shipToFollow.position;

        if(duration > 0)
            duration -= Time.deltaTime;
        else if (duration < 0)
            EndLaunchedPreview();
    }

    public override void EndLaunchedPreview()
    {
        /*active = false;
        anchorAnimator.SetTrigger("ended");
        duration = 0;*/
        anchorSoundSource.PlaySound(anchorEndSound);
        active = false;
        //EndAnchor();
    }

    public bool ReadyToBeReturned
    {
        get
        {
            return !anchorSoundSource.isPlaying;
        }
    }

    public void EndAnchor()
    {
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(this);
        shipToFollow = null;
    }
}
