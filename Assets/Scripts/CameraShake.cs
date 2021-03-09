using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Transform cameraTransform;
    
    /*
     * shakeDuraction - время за которое наша камера будет трястись
     * shakeAmount - как сильно камера будет трястись
     * decreaseFactor - на сколько быстро мы остановим shakeDuraction (т.е. тряску камеры)
     */
    private float shakeDuraction = 1f, shakeAmount = 0.04f, decreaseFactor = 1.5f;

    // Исходная позиция камеры
    private Vector3 originPosition;

    private void Start()
    {
        cameraTransform = GetComponent<Transform>();
        originPosition = cameraTransform.localPosition;
    }

    private void Update()
    {
        if (shakeDuraction > 0)
        {
            cameraTransform.localPosition = originPosition + Random.insideUnitSphere * shakeAmount;
            shakeDuraction -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuraction = 0;
            cameraTransform.localPosition = originPosition;
        }
    }
}
