using UnityEngine;

public class WreckingBallSwing : MonoBehaviour
{
    public float swingAmplitude = 3f;   // how far side to side
    public float swingSpeed = 2f;       // how fast it swings

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * swingSpeed) * swingAmplitude;
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}
