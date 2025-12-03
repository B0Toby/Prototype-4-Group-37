using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.XR;
using System.Collections;

public class Pig : BaseAnimal
{
    //[Header("Audio")]
    //private AudioSource audioSource;
    //public AudioClip bounceClip;

    [Header("Audio")]
    public AudioSource pigAudioSource;
    public AudioClip[] pigClips;
    [Header("Pig Noise Timing")]
    public float minOinkDelay = 3f;
    public float maxOinkDelay = 10f;

    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer sr;

    private static readonly int HashDoBounce = Animator.StringToHash("DoBounce");

    private void Awake()
    {
        animalName = "Pig";
        behaviorNote = "Some animals will bounce off of it";
        bounce = true;

        if (animator == null)
            animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

        if (pigAudioSource != null && pigClips.Length > 0)
            StartCoroutine(PigNoiseRoutine());
    }
    

    private IEnumerator PigNoiseRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minOinkDelay, maxOinkDelay);
            yield return new WaitForSeconds(wait);

            AudioClip clip = pigClips[Random.Range(0, pigClips.Length)];

            // random pitch/volume for variety
            pigAudioSource.pitch = 1f + Random.Range(-0.05f, 0.05f);
            float volume = Random.Range(0.2f, 0.5f);

            pigAudioSource.PlayOneShot(clip, volume);
        }
    }

    // copied from crab, adjust in a minute maybe
    public override Vector3Int RageStep(NpcController npc)
    {

        // move 1 step toward player
        Vector3Int npcCell = npc.GetCellPosition();
        Vector3Int playerCell = GameManager.I.currentPlayerCell;

        Vector3Int step = GetStepToward(npcCell, playerCell);

        if (step == Vector3Int.zero)
            return npcCell;

        Vector3Int nextCell = npcCell + step;

        // walls and obstacles both block in rage phase
        if (IsWall(nextCell) || IsObstacle(nextCell))
        {
            return npcCell;
        }

        return nextCell;
    }

    public override void OnPlayerStep()
    {
        Debug.Log("Pig cell = " + this.GetCellPosition());
        if (GameManager.I == null)
            return;
    }

    public override void OnBounced(Vector3Int fromDir)
    {
        if (animator != null)
        {
            animator.ResetTrigger(HashDoBounce);
            animator.SetTrigger(HashDoBounce);
        }

        if (pigAudioSource != null && pigClips.Length > 0)
        {
            AudioClip clip = pigClips[Random.Range(0, pigClips.Length)];
            pigAudioSource.pitch = 1f + Random.Range(-0.1f, 0.1f);
            float volume = Random.Range(0.4f, 0.7f);
            pigAudioSource.PlayOneShot(clip, volume);
        }
    }
}
