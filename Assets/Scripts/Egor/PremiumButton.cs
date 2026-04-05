using UnityEngine;
using UnityEngine.EventSystems; // Нужно для отслеживания мышки
using DG.Tweening; // Подрубаем наш скачанный DOTween

// Добавляем интерфейсы для наведения и клика
public class PremiumButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Время анимации (можешь потом покрутить в Инспекторе)
    public float animDuration = 0.2f;

    // Срабатывает, когда мышка наводится на иконку
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Плавно увеличиваем размер до 1.15
        transform.DOScale(1.15f, animDuration).SetEase(Ease.OutBack);
    }

    // Срабатывает, когда мышка уходит с иконки
    public void OnPointerExit(PointerEventData eventData)
    {
        // Возвращаем размер к стандартному (1.0)
        transform.DOScale(1f, animDuration).SetEase(Ease.OutQuad);
    }

    // Срабатывает при самом клике/тапе
    public void OnPointerDown(PointerEventData eventData)
    {
        // Эффект легкой пульсации-вдавливания
        transform.DOPunchScale(new Vector3(-0.2f, -0.2f, -0.2f), 0.15f, 1);

        // Если это шестеренка, можно еще заставить ее крутиться
        // transform.DORotate(new Vector3(0, 0, 90), 0.2f, RotateMode.LocalAxisAdd);
    }
}