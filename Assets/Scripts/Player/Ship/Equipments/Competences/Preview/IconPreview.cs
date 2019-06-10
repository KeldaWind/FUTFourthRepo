using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconPreview : Preview
{
    [SerializeField] Animator iconAnimator;
    [SerializeField] AudioSource iconSoundSource;
    [SerializeField] Sound iconStartSound;
    [SerializeField] Sound iconEndSound;
    Transform shipToFollow;
    bool previewing;
    bool active;
    float duration;
    float minimumTimeBeforeReturn = 1;
    float remainingTimeBeforeReturn;

    private void Update()
    {
        if (previewing)
            UpdatePreparePreview(Vector3.zero, Vector3.zero);
        else if (active)
            UpdateLaunchedPreview();
        else if (ReadyToBeReturned)
            EndIcon();
        else
        {
            transform.position = shipToFollow.position;

            if (remainingTimeBeforeReturn > 0)
                remainingTimeBeforeReturn -= Time.deltaTime;
            else if (remainingTimeBeforeReturn < 0)
                remainingTimeBeforeReturn = 0;
        }
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
        if (GetPreviewType == PreviewPoolTag.Anchor)
        {
            previewing = false;
            iconAnimator.SetTrigger("started");
            active = true;
            duration = parameter;

            iconSoundSource.PlaySound(iconStartSound);
        }
        else if (GetPreviewType == PreviewPoolTag.Boost)
        {
            previewing = false;
            iconAnimator.SetTrigger("started");

            iconSoundSource.PlaySound(iconStartSound);
            remainingTimeBeforeReturn = minimumTimeBeforeReturn;
        }
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
        iconSoundSource.PlaySound(iconEndSound);
        active = false;
    }

    public bool ReadyToBeReturned
    {
        get
        {
            return !iconSoundSource.isPlaying && remainingTimeBeforeReturn == 0;
        }
    }

    public void EndIcon()
    {
        Debug.Log("end");
        gameObject.SetActive(false);
        GameManager.gameManager.PoolManager.ReturnPreview(this);
        shipToFollow = null;
    }
}
