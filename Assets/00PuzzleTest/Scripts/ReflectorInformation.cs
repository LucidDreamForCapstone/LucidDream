using System.Collections.Generic;
using UnityEngine;

public class ReflectorInformation : MonoBehaviour, Interactable
{
    [SerializeField]
    private ReflectorInformationObj reflectorInformationObj;
    [SerializeField]
    private bool isUsed = false;
    void Start()
    {

    }

    void Update()
    {

    }

    public bool IsUsed {
        get {
            return isUsed;
        }
        set {
            isUsed = value;
        }
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

    #region Interaction
    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            RotateReflector();
        }
    }
    public bool IsInteractBlock()
    {
        return true;
    }

    public string GetInteractText()
    {
        return "회전 (G)";
    }

    protected virtual void RotateReflector()
    {
        if (Player2_InteractManager.Instance.CheckInteractable(this))
        {

            if (Input.GetKey(KeyCode.G))
            {
                Player2_InteractManager.Instance.InteractCoolTime().Forget();
                transform.Rotate(0, 0, 2);
            }
        }
    }
    #endregion
    #region properties
    public ReflectorInformationObj ReflectorInformationObj {
        get {
            return reflectorInformationObj;
        }
    }
    #endregion
}
