using UnityEngine;

public class DottedLineAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Вызываем этот метод после создания новых капсул
    public void RefreshAnimation()
    {
        if (_animator != null)
        {
            _animator.Rebind(); // Сброс и перезапуск анимации
            _animator.Update(0); // Принудительное обновление кадра
        }
    }
}