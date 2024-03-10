using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LEADER
{
    NONE,
    ATK_LEADER,
    HP_LEADER,
}

public class LeadersAbility : MonoBehaviour
{
    public LEADER leaderType;
}
