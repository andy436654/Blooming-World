using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class HammerBooster : MonoBehaviour
{
    public Button hammerButton;
    private bool isHammerActive = false;
    public ParticleSystem destroyEffect;
    public AudioClip destroySound;
    public AudioMixerGroup sfxMixerGroup;

    public Color normalColor = Color.white;
    public Color activeColor = Color.yellow;
    public Color cooldownColor = Color.gray; // ���� �� ����� ��������

    private Image buttonImage;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private const float cooldownDuration = 15f; // 15 ������ ��������

    [SerializeField] private int maxUses = 3; // ������������ ���������� �������������
    private int remainingUses; // ���������� �������������

    // ��������� ��������� ��� ����������
    private Image cooldownFillImage;
    public GameObject cooldownFillObject; // �������� ������ � Image ��� ����������

    public TMP_Text remainingText;

    void Start()
    {
        remainingUses = maxUses; // �������������� ���������� �������������

        if (hammerButton != null)
        {
            hammerButton.onClick.AddListener(ActivateHammer);
            buttonImage = hammerButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = normalColor;
            }
        }

        // �������������� fill image
        if (cooldownFillObject != null)
        {
            cooldownFillImage = cooldownFillObject.GetComponent<Image>();
            cooldownFillImage.fillAmount = 0f; // �������� ������
            cooldownFillObject.SetActive(false); // �������� �������
        }

        UpdateButtonState();
    }

    void Update()
    {
        remainingText.text = $"�������\n({remainingUses})";

        // ��������� ��������
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            // ��������� ����������
            if (cooldownFillImage != null)
            {
                float fillAmount = 1f - (cooldownTimer / cooldownDuration);
                cooldownFillImage.fillAmount = fillAmount;

                // ����� �������� �������� �� ������ � ����������� �����
                cooldownFillImage.color = Color.Lerp(cooldownColor, normalColor, fillAmount);
            }

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;

                if (cooldownFillObject != null)
                    cooldownFillObject.SetActive(false);

                UpdateButtonState();
            }
        }

        if (!isHammerActive) return;

        // ��������� ������
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTouch(touch.position);
            }
        }
    }

    public void ActivateHammer()
    {
        if (isOnCooldown || remainingUses <= 0) return;

        isHammerActive = true;
        if (buttonImage != null)
        {
            buttonImage.color = activeColor;
        }
        Debug.Log("����� �����������!");
    }

    void HandleTouch(Vector2 screenPosition)
    {
        if (!isHammerActive) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Block"))
        {
            InputBlocker.BlockInputForSeconds(0.2f);

            // ���������� ������
            if (destroyEffect != null)
            {
                AudioManager.PlaySound(destroySound, hit.point);
                ParticleSystem effect = Instantiate(destroyEffect, hit.point, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }

            Destroy(hit.collider.gameObject);

            // ����� �������������
            isHammerActive = false;
            remainingUses--; // ��������� ���������� �������������
            StartCooldown();

            UpdateButtonState();
        }
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownDuration;

        if (cooldownFillObject != null & remainingUses != 0)
        {
            cooldownFillObject.SetActive(true);
            cooldownFillImage.fillAmount = 0f;
        }
    }

    void UpdateButtonState()
    {
        if (buttonImage == null) return;

        if (remainingUses <= 0)
        {
            // ���� ������������� �� ��������
            buttonImage.color = cooldownColor;
            hammerButton.interactable = false;
        }
        else if (isOnCooldown)
        {
            // ���� �� ��������
            buttonImage.color = cooldownColor;
            hammerButton.interactable = false;
        }
        else
        {
            // ���� ����� � �������������
            buttonImage.color = normalColor;
            hammerButton.interactable = true;
        }
    }

    public static bool IsHammerActive()
    {
        HammerBooster instance = FindObjectOfType<HammerBooster>();
        return instance != null && instance.isHammerActive;
    }
}