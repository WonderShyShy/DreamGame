using System;
using Models;
using Other;
using Spine.Unity;
using UnityEngine;

public class GameOverDogGroup : MonoBehaviour
{
    public GameObject starEff;
    public GameObject _starEff2;
    public GameObject _starEff3;

    public GameObject textNormal1;
    public GameObject textNewBest1;
    
    private Action _endCallback;
    
    public void SetEndCallback(Action act)
    {
        _endCallback = act;
    }

    public void ShowStarEff(int starNum)
    {
        starEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "star", false);
        if (starNum >= 2)
        {
            StartCoroutine(Delay.Run(() =>
            {
                _starEff2.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "star", false);
            }, 0.3f));
        }

        if (starNum >= 3)
        {
            StartCoroutine(Delay.Run(() =>
            {
                _starEff3.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "star", false);
            }, 0.6f));
        }

        StartCoroutine(Delay.Run(() =>
        {
            _endCallback?.Invoke();
        }, (starNum - 1) * 0.3f + 0.01f));
    }

    public void SetText(string textType = "normal")
    {
        textNormal1.SetActive(false);
        textNewBest1.SetActive(false);
        
        switch (textType)
        {
            case "normal":
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                    textNormal1.SetActive(true);
                }
                break;
            case "newBest":
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                    textNewBest1.SetActive(true);
                }
                break;
        }
    }
}
