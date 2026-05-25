using UnityEngine;

public class MonsterFixRotation : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }
}
