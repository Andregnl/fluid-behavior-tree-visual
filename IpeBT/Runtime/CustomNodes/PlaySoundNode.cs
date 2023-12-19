using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using UnityEngine.Audio;

public class PlaySoundAction : ActionBase
{
    public AudioClip audioClip;
    public Vector3 soundPosition;

    private bool isPlaying;
    private float elapsedTime;

    // Triggers only the first time this node is run (great for caching data)
    protected override void OnInit()
    {
        
    }

    // Triggers every time this node starts running. Does not trigger if TaskStatus.Continue was last returned by this node
    protected override void OnStart()
    {
        AudioSource.PlayClipAtPoint(audioClip, soundPosition);
        isPlaying = true;
        elapsedTime = 0f;
    }

    // Triggers every time `Tick()` is called on the tree and this node is run
    protected override TaskStatus OnUpdate()
    {
        if (!isPlaying) return TaskStatus.Failure;

        elapsedTime += Time.deltaTime;
        if (elapsedTime > audioClip.length)
        {
            isPlaying = false;
            return TaskStatus.Success;
        }

        return TaskStatus.Continue;
    }

    // Triggers whenever this node exits after running
    protected override void OnExit()
    {
        isPlaying = false;
        elapsedTime = 0f;
    }
}
