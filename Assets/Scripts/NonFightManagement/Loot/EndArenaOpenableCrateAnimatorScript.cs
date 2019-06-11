using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndArenaOpenableCrateAnimatorScript : MonoBehaviour
{
    [SerializeField] EndArenaOpenableCrate crate;
    public void PlayAppearingSound()
    {
        crate.PlayAppearingSound();
    }

    public void PlayOpeningSound()
    {
        crate.PlayOpeningSound();
    }
}
