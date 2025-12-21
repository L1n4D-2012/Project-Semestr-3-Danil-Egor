using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Игрок
    public float smoothSpeed = 10f;

    [Header("Настройки позиции")]
    // X всегда ставь 0, чтобы камера была по центру дороги
    // Y - высота, Z - отдаление (с минусом)
    public Vector3 offset = new Vector3(0, 7, -10);

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Берем позицию игрока + твою настройку смещения
        Vector3 desiredPosition = target.position + offset;

        // 2. ЖЕСТКО запрещаем камере смещаться влево/вправо. Она всегда по центру (X=0).
        desiredPosition.x = 0f;

        // 3. Плавно перемещаем камеру (только позицию, БЕЗ вращения)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // ВАЖНО: Я убрал transform.LookAt. Теперь камера не поворачивается.
    }
}