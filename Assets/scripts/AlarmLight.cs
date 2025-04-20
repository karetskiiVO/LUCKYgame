using UnityEngine;

public class AlarmLight : MonoBehaviour {
    [SerializeField]
    private float period = 3f;

    [SerializeField]
    private Light emergencyLight;

    private float timer = 0;
    private float maxIntensity; 
    void Start () {
        maxIntensity = emergencyLight.intensity;
    }

    void Update () {
        timer += Time.deltaTime;

        emergencyLight.intensity = maxIntensity * Mathf.Pow(Mathf.Cos(timer * Mathf.PI / period), 2);
    }
}