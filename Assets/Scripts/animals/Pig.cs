using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.XR;

public class Pig : BaseAnimal
{
    // add later
    //[Header("Audio")]

    //[Header("Animation")]

    private void Awake()
    {
        animalName = "Pig";
        behaviorNote = "Other animals will bounce off of it";
        bounce = true;
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
        // play idle animation
        
    }

}
