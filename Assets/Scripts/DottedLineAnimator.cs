using UnityEngine;

public class DottedLineAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // �������� ���� ����� ����� �������� ����� ������
    public void RefreshAnimation()
    {
        if (_animator != null)
        {
            _animator.Rebind(); // ����� � ���������� ��������
            _animator.Update(0); // �������������� ���������� �����
        }
    }
}