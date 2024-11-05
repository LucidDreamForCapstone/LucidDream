using System.Collections.Generic;
using UnityEngine;

public class ReflectorInformation : MonoBehaviour
{
    [SerializeField]
    private ReflectorInformationObj reflectorInformationObj;
    void Start()
    {

    }

    void Update()
    {

    }
    public List<Vector2> GetRotatedReflectionDirections()
    {
        float rotationAngle = transform.rotation.eulerAngles.z;

        List<Vector2> result = new List<Vector2>();

        foreach (Vector2 direction in reflectorInformationObj.ReflectionDirections)
        {
            Vector2 rotatedDirection = RotateVector(direction, rotationAngle);
            result.Add(rotatedDirection);
        }
        return result;
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radian = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        float x = vector.x * cos - vector.y * sin;
        float y = vector.x * sin + vector.y * cos;

        return new Vector2(x, y);
    }
    #region properties
    public ReflectorInformationObj ReflectorInformationObj {
        get {
            return reflectorInformationObj;
        }
    }
    #endregion
}
