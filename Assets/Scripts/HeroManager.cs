using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public float FOV;
    public float attackRange;

    [HideInInspector]
    public static Dictionary<string, Color> fieldDebugColors;
}
