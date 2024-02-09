using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float maxAngle;
    private bool isOpened;
    [SerializeField] private AnimationCurve curve;
    public IEnumerator OpenDoor()
    {
        if(!isOpened)
        {
            for (float i = 0; i < 1; i += Time.deltaTime / duration)
            {
                transform.parent.rotation = Quaternion.Lerp(
                    Quaternion.Euler(0, 0, 0),
                    Quaternion.Euler(0, maxAngle, 0),
                    curve.Evaluate(i));

                yield return null;
            }
        }
    }
}
