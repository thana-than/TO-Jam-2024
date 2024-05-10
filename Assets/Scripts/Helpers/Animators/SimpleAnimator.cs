using System;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class SimpleAnimator : MonoBehaviour
{
    //[Serializable] public class WeightedAnimationClip : WeightedItem<AnimationClip> { public WeightedAnimationClip() { } }

    // [Serializable] public class WeightedRandomAnimationClip : WeightedRandom<AnimationClip> { }
    // public WeightedRandomAnimationClip clips;
    public AnimationClip[] clips;
    int clipLength = 0;
    public int playOnStart = -1;

    public float speed = 1;

    float buffer_speed = 1;
    PlayableGraph playableGraph;

    AnimationClipPlayable[] playableClips;

    AnimationPlayableOutput output;

    public int PlayingIndex { get; private set; } = -1;

    public float Time => PlayingIndex >= 0 ? ((float)playableClips[PlayingIndex].GetTime() % Duration) : 0;
    public float Duration => PlayingIndex >= 0 ? clips[PlayingIndex].length : 0;

    void OnEnable()
    {
        clipLength = clips.Length;
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        output = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());
        // Wrap the clip in a playable
        BuildClips();
        Play(playOnStart);
        //var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
    }

    void BuildClips()
    {
        playableClips = new AnimationClipPlayable[clipLength];

        for (int i = 0; i < clipLength; i++)
            playableClips[i] = AnimationClipPlayable.Create(playableGraph, clips[i]);
    }

    public void Play(int index)
    {
        if (index < 0 || index >= clipLength)
            return;

        // Connect the Playable to an output
        playableClips[index].SetTime(0);
        playableClips[index].SetSpeed(speed);
        output.SetSourcePlayable(playableClips[index]);
        playableGraph.Play();
        PlayingIndex = index;
    }

    // public void PlayRandom()
    // {
    //     int index = clips.RollForIndex();
    //     if (index >= 0)
    //         Play(index);
    // }

    public void Stop()
    {
        PlayingIndex = -1;
        playableGraph.Stop();
    }

    void OnDisable()
    {
        Stop();
        // Destroys all Playables and PlayableOutputs created by the graph.
        playableGraph.Destroy();
    }

    private void Update()
    {
        if (PlayingIndex < 0)
            return;

        if (buffer_speed != speed)
        {
            playableClips[PlayingIndex].SetSpeed(speed);
            buffer_speed = speed;
        }
    }

}