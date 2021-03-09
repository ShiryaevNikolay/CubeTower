using System;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float speed = 10f;
    private Transform _rotator;

    private void Start()
    {
        // в переменную _rotator нашли и поместили компонент Transform, так как скрипт прикреплен к объекту, у которого есть этот компонент
        _rotator = GetComponent<Transform>();
    }

    private void Update()
    {
        /*
         * Функция Rotate позволяет поворачивать текущий объект вокруг
         * собственной оси по определенной координате с определенной скоростью
         *
         * Умножаем скорость speed на Time.deltaTime чтобы камера не крутилась слишком быстро
         */
        _rotator.Rotate(0, speed * Time.deltaTime, 0);
    }
}
