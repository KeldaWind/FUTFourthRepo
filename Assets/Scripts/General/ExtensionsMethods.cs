using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fonctions suppélementaires pour des classes de base de Unity
/// </summary>
public static class ExtensionsMethods
{
    #region Vectors
    /// <summary>
    /// Renvoie le vecteur perpendiculaire vers la droite, selon l'axe Y
    /// </summary>
    /// <param name="baseVector">Vecteur de base</param>
    /// <returns>Vecteur perpendiculaire vers la droite selon l'axe Y</returns>
    public static Vector3 GetLeftOrthogonalVectorXZ(this Vector3 baseVector)
    {
        return new Vector3(-baseVector.z, baseVector.y, baseVector.x);
    }

    /// <summary>
    /// Renvoie le vecteur perpendiculaire vers la gauche, selon l'axe Y
    /// </summary>
    /// <param name="baseVector">Vecteur de base</param>
    /// <returns>Vecteur perpendiculaire vers la gauche selon l'axe Y</returns>
    public static Vector3 GetRightOrthogonalVectorXZ(this Vector3 baseVector)
    {
        return new Vector3(baseVector.z, baseVector.y, -baseVector.x);
    }
    #endregion

    #region Lists
    /// <summary>
    /// Permet de récupérer un membre aléatoire d'une liste, quelle que soit le type de paramètre
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">Liste dont on veut avoir un membre aléatoire</param>
    /// <returns>Membre aléatoire de la liste</returns>
    public static T GetRandomMemberOfTheList<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    #endregion

    #region Curves
    public static AnimationCurve SetAsFastInSlowOutIncreasingCurve(this AnimationCurve curve)
    {
        Keyframe startKey = new Keyframe(0, 0, 2, 2);
        Keyframe endKey = new Keyframe(1, 1, 0, 0);

        return new AnimationCurve(startKey, endKey);
    }

    public static AnimationCurve SetAsSlowInFastOutIncreasingCurve(this AnimationCurve curve)
    {
        Keyframe startKey = new Keyframe(0, 0, 0, 0);
        Keyframe endKey = new Keyframe(1, 1, 2, 2);

        return new AnimationCurve(startKey, endKey);
    }

    public static AnimationCurve SetAsFastInSlowOutDescreasingCurve(this AnimationCurve curve)
    {
        Keyframe startKey = new Keyframe(0, 1, -2, -2);
        Keyframe endKey = new Keyframe(1, 0, 0, 0);

        return new AnimationCurve(startKey, endKey);
    }

    public static AnimationCurve SetAsSlowInFastOutDecreasingCurve(this AnimationCurve curve)
    {
        Keyframe startKey = new Keyframe(0, 1, 0, 0);
        Keyframe endKey = new Keyframe(1, 0, -2, -2);

        return new AnimationCurve(startKey, endKey);
    }
    #endregion

    public static void SetUpWithSound(this AudioSource source, Sound sound)
    {
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.loop = sound.loop;
        source.pitch = Random.Range(sound.minPitch, sound.maxPitch);
    }

    public static void PlaySound(this AudioSource source, Sound sound)
    {
        source.SetUpWithSound(sound);
        source.Play();
    }
}