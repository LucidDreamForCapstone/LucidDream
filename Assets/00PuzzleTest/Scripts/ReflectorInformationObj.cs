using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReflectionData", menuName = "Scriptable Object/Reflection Data", order = int.MaxValue)]
public class ReflectorInformationObj : ScriptableObject
{
    [SerializeField]
    List<Vector2> reflectionDirections;
    public List<Vector2> ReflectionDirections => reflectionDirections;
}
