using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats Instance { get; private set; }

    [Header("Character Progress")]
    public int NPCA = 0;
    public int NPCB = 0;
    public int NPCC = 0;
    public int NPCD = 0;
    public int NPCE = 0;
    public int NPCF = 0;


    [Header("Story Progression")]

    public int NPCG = 0;

    [Header("Achievements")]
    public int NPCH = 0;


}
