using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LoadingScreenManager
{
    OnLoadingScreenEvent OnScreenFinishedTransition;
    [SerializeField] Image[] loadingImages;
    [SerializeField] AnimationCurve opacityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float appearingDuration;
    [SerializeField] float disappearingDuration;
    float currentDuration;

    LoadingScreenState currentLoadingScreenState = LoadingScreenState.Shown;
    public void StartBeginLoad(OnLoadingScreenEvent actionWhenLoaded)
    {
        currentLoadingScreenState = LoadingScreenState.Disappearing;

        foreach(Image loadingImage in loadingImages)
            loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, 1);

        currentDuration = disappearingDuration;

        OnScreenFinishedTransition = actionWhenLoaded;
    }

    public void StartEndLoad(OnLoadingScreenEvent actionWhenLoaded)
    {
        currentLoadingScreenState = LoadingScreenState.Appearing;

        foreach (Image loadingImage in loadingImages)
            loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, 0);

        currentDuration = appearingDuration;

        OnScreenFinishedTransition = actionWhenLoaded;
    }

    public void UpdateLoadingScreen()
    {
        if(currentLoadingScreenState == LoadingScreenState.Disappearing)
        {
            if(currentDuration > 0)
            {
                currentDuration -= Time.deltaTime;

                foreach (Image loadingImage in loadingImages)
                {
                    Color newColor = loadingImage.color;
                    newColor.a = opacityCurve.Evaluate(currentDuration / disappearingDuration);
                    loadingImage.color = newColor;
                }
            }
            else if (currentDuration < 0)
            {
                currentDuration = 0;
                foreach (Image loadingImage in loadingImages)
                {
                    loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, 0);
                    currentLoadingScreenState = LoadingScreenState.Hidden;
                }

                if(OnScreenFinishedTransition != null)
                    OnScreenFinishedTransition();
            }
        }
        else if(currentLoadingScreenState == LoadingScreenState.Appearing)
        {
            if (currentDuration > 0)
            {
                currentDuration -= Time.deltaTime;
                foreach (Image loadingImage in loadingImages)
                {
                    Color newColor = loadingImage.color;
                    newColor.a = opacityCurve.Evaluate(1 - (currentDuration / appearingDuration));
                    loadingImage.color = newColor;
                }
            }
            else if (currentDuration < 0)
            {
                currentDuration = 0;
                foreach (Image loadingImage in loadingImages)
                {
                    loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, 1);
                    currentLoadingScreenState = LoadingScreenState.Shown;
                }

                if (OnScreenFinishedTransition != null)
                    OnScreenFinishedTransition();
            }
        }
    }
}

public enum LoadingScreenState
{
    Shown, Disappearing, Hidden, Appearing
}

public delegate void OnLoadingScreenEvent();
