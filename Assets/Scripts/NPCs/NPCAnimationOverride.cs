using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Auf jeden NPC legen (zusammen mit CityPeople).
// Setzt AutoPlayAnimations auf false und spielt nur die erlaubten Clips.
public class NPCAnimationOverride : MonoBehaviour
{
    [Tooltip("Sekunden zwischen Animationswechsel")]
    public float minInterval = 10f;
    public float maxInterval = 20f;

    private Animator _animator;
    private List<AnimationClip> _allowedClips = new List<AnimationClip>();

    private static readonly string[] AllowedNames =
    {
        "dance_hype_100f",
        "dance_m_flossing_40f",
        "dance_riverdance_60f",
        "exercise_warmingUp_170f"
    };

    private void Start()
    {
        var cityPeople = GetComponent<CityPeople.CityPeople>();
        if (cityPeople != null)
            cityPeople.enabled = false;

        _animator = GetComponent<Animator>();
        if (_animator == null) return;

        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            foreach (var allowed in AllowedNames)
            {
                if (clip.name == allowed)
                {
                    _allowedClips.Add(clip);
                    break;
                }
            }
        }

        if (_allowedClips.Count > 0)
        {
            PlayRandom();
            StartCoroutine(Loop());
        }
    }

    private void PlayRandom()
    {
        var clip = _allowedClips[Random.Range(0, _allowedClips.Count)];
        _animator.CrossFadeInFixedTime(clip.name, 0.5f);
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            PlayRandom();
        }
    }
}
