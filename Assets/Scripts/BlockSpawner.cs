using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BlockSpawner : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    public float spawnHeight = 10f;
    public string placedTag = "PlacedBlock"; // ���, ������� ������� ���� ��� �����������

    private Camera mainCamera;
    private GameObject previewBlock;
    private bool isDragging = false;

    public float minX = -5f; // ����������� ������� �� X
    public float maxX = 5f;  // ������������ ������� �� X

    public float spawnCooldown = 1.0f;
    private bool isOnCooldown = false;

    public Image cooldownIndicator;  // UI Image (��������, ����������� ����)

    [Header("Wall Offset Settings")]
    public float minWallOffset = 0.5f; // ������ �� ����
    public float maxWallOffset = 0.5f; // ������ �� ����
    private float effectiveMinX;
    private float effectiveMaxX;
    public float expandMinX = 0f; // �� ������� ��������� ����������� ������� (� ������������� �������)
    public float expandMaxX = 0f; // �� ������� ��������� ������������ �������


    [Header("Landing Prediction")]
    public LineRenderer landingLine; // ����� ������������ �����������
    public int lineSegments = 20; // ���������� ��������� �����
    public float lineWidthMultiplier = 0.2f; // ��������� ������ ������������ ������� �����
    public Color lineColor = new Color(1, 1, 1, 0.5f); // ���� �����

    private float currentBlockWidth; // ������ �������� �����

    void Start()
    {
        mainCamera = Camera.main;
        
        if (landingLine == null)
        {
            GameObject lineObj = new GameObject("LandingLine");
            landingLine = lineObj.AddComponent<LineRenderer>();
        }
        
        landingLine.material = new Material(Shader.Find("Sprites/Default"));
        landingLine.startColor = lineColor;
        landingLine.endColor = lineColor;
        landingLine.positionCount = lineSegments;
        landingLine.enabled = false;

        // �������������� ������� �� GameBoundsManager
        if (GameBoundsManager.Instance != null)
        {
            minX = GameBoundsManager.Instance.GetMinX();
            maxX = GameBoundsManager.Instance.GetMaxX();
        }
    }

    void Update()
    {
        // ��������� ����������� ������� � ������ �������
        effectiveMinX = minX + minWallOffset;
        effectiveMaxX = maxX - maxWallOffset;

        // ���� ���� ������������ (����� �������) � ���������� �����
        if (InputBlocker.IsInputBlocked())
            return;

        // ���������, ��������� �� ������/��� ��� UI ���������
        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        // ���� ��� �������, ����� ��������� ���������� fingerId
        if (Input.touchCount > 0)
        {
            isPointerOverUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        // ��������� ��� ����, ��� � �������
        bool inputStarted = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool inputEnded = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
        bool isInputActive = Input.GetMouseButton(0) || (Input.touchCount > 0 &&
                            (Input.GetTouch(0).phase == TouchPhase.Moved ||
                             Input.GetTouch(0).phase == TouchPhase.Stationary));

        if (inputStarted && !isOnCooldown && !isDragging  && !isPointerOverUI)
        {
            StartDragging();

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.CompleteStep("SpawnBlock");
            }
        }

        if (isDragging && isInputActive)
        {
            UpdatePreviewPosition();
            UpdateLandingPrediction(); // ��������� ����� ������������
        }

        if (inputEnded && isDragging && !isOnCooldown)
        {
            SpawnBlock();
            StartCoroutine(CooldownRoutine());
            isDragging = false;
        }

        // ��������� ������� ����� ��������������
        if (GameBoundsManager.Instance != null)
        {
            minX = GameBoundsManager.Instance.GetMinX();
            maxX = GameBoundsManager.Instance.GetMaxX();
        }
    }

    // �������� ��� ��������
    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        float timer = 0f;

        while (timer < spawnCooldown)
        {
            timer += Time.deltaTime;
            if (cooldownIndicator != null)
            {
                cooldownIndicator.fillAmount = timer / spawnCooldown;
            }
            yield return null;
        }

        isOnCooldown = false;
        if (cooldownIndicator != null)
        {
            cooldownIndicator.fillAmount = 0f;  // ���������� ���������
        }
    }

    void StartDragging()
    {
        isDragging = true;
        int randomIndex = Random.Range(0, blockPrefabs.Length);
        previewBlock = Instantiate(blockPrefabs[randomIndex], Vector3.zero, Quaternion.identity);
        previewBlock.tag = "Untagged";
        SetBlockTransparency(previewBlock, 0.5f);

        // �������� ������� ����� � ������ ���� �������� ��������
        CalculateBlockSize();

        // ������������ ������� ����� ����� �������� �����
        CalculateEffectiveBounds();

        // ������������� ������ ����� �� ������ ������� �����
        float lineWidth = currentBlockWidth * lineWidthMultiplier;
        landingLine.startWidth = lineWidth;
        landingLine.endWidth = lineWidth;

        landingLine.enabled = true;
    }

    void CalculateBlockSize()
    {
        if (previewBlock == null)
        {
            currentBlockWidth = 1f; // �������� �� ���������
            return;
        }

        // �������� �������� �����
        Renderer blockRenderer = previewBlock.GetComponent<Renderer>();
        if (blockRenderer != null)
        {
            currentBlockWidth = blockRenderer.bounds.size.x;
        }
        else
        {
            // �������������� ������ ��� �������� � �����������
            Collider2D collider = previewBlock.GetComponent<Collider2D>();
            if (collider != null)
            {
                currentBlockWidth = collider.bounds.size.x;
            }
            else
            {
                currentBlockWidth = 1f; // �������� �� ���������
            }
        }
    }

    private void CalculateEffectiveBounds()
    {
        // ������� ������� ��� ����� �����
        float baseMinX = minX + minWallOffset - expandMinX; // ��������� �����
        float baseMaxX = maxX - maxWallOffset + expandMaxX; // ��������� ������

        if (previewBlock == null)
        {
            effectiveMinX = baseMinX;
            effectiveMaxX = baseMaxX;
            return;
        }

        // ������������ ���������� "�����" ��� ������� ������
        float availableWidth = baseMaxX - baseMinX;
        float maxAllowedReduction = availableWidth * 0.3f;

        // ������������ �������� ���������� (�������� ������ �����)
        float desiredReduction = currentBlockWidth / 2f;

        // ��������� ����������, �� �� ������ ����������� �����������
        float actualReduction = Mathf.Min(desiredReduction, maxAllowedReduction);

        effectiveMinX = baseMinX + actualReduction;
        effectiveMaxX = baseMaxX - actualReduction;

        // �����������, ��� ����� ������� �� ����� ������ ������
        if (effectiveMinX > effectiveMaxX)
        {
            effectiveMinX = effectiveMaxX = (baseMinX + baseMaxX) / 2f;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(effectiveMinX, spawnHeight, 0), new Vector3(effectiveMinX, spawnHeight + 2, 0));
        Gizmos.DrawLine(new Vector3(effectiveMaxX, spawnHeight, 0), new Vector3(effectiveMaxX, spawnHeight + 2, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, spawnHeight, 0), new Vector3(minX, spawnHeight + 2, 0));
        Gizmos.DrawLine(new Vector3(maxX, spawnHeight, 0), new Vector3(maxX, spawnHeight + 2, 0));
    }

    void UpdateLandingPrediction()
    {
        if (previewBlock == null || !landingLine.enabled) return;

        // �������� ��������� �����
        Collider2D blockCollider = previewBlock.GetComponent<Collider2D>();
        if (blockCollider == null) return;

        // ������������ ���������� �������
        Vector2 startPos = previewBlock.transform.position;
        Vector2 velocity = Vector2.zero; // ��������� �������� (0, ��� ��� ���� ��� �� ������)
        Vector2 gravity = Physics2D.gravity;

        // ������������ ����� ����������
        for (int i = 0; i < lineSegments; i++)
        {
            float t = i / (float)(lineSegments - 1) * 2f; // ����������� ����� ��� ����� ������� ����������
            Vector2 pos = startPos + velocity * t + 0.5f * gravity * t * t;

            // ��������� ������������ � ������
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 0.1f, LayerMask.GetMask("Ground", "PlacedBlock"));
            if (hit.collider != null)
            {
                // ���� ������ � �����, ��������� ���������� ����� ��������� ��������
                for (int j = i; j < lineSegments; j++)
                {
                    landingLine.SetPosition(j, hit.point);
                }
                break;
            }

            landingLine.SetPosition(i, pos);
        }
    }

    void UpdatePreviewPosition()
    {
        Vector3 inputPos;

        if (Input.touchCount > 0)
        {
            inputPos = Input.GetTouch(0).position;
        }
        else
        {
            inputPos = Input.mousePosition;
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(inputPos);
        mouseWorldPos.z = 0;
        mouseWorldPos.y = spawnHeight;

        CalculateEffectiveBounds(); // ������ ������� ��� �������� ����������

        if (effectiveMinX >= effectiveMaxX)
        {
            mouseWorldPos.x = (minX + maxX) / 2f;
        }
        else
        {
            mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, effectiveMinX, effectiveMaxX);
        }

        previewBlock.transform.position = mouseWorldPos;
    }

    void SpawnBlock()
    {
        if (previewBlock == null) return;

        // ������������ ������� � ������ ������� ����� ����� �������
        CalculateEffectiveBounds();

        // ��������� ������� ����� �������
        Vector3 blockPos = previewBlock.transform.position;
        blockPos.x = Mathf.Clamp(blockPos.x, effectiveMinX, effectiveMaxX);
        previewBlock.transform.position = blockPos;

        SetBlockTransparency(previewBlock, 1f);

        Rigidbody2D rb = previewBlock.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = previewBlock.AddComponent<Rigidbody2D>();
        }

        LandDetector landDetector = previewBlock.AddComponent<LandDetector>();
        landDetector.Initialize(this, placedTag);

        previewBlock = null;

        landingLine.enabled = false; // ��������� ����� ����� ������ �����
    }

    // ����� ��� ��������� ���� (����� ���������� �� LandDetector)
    public void OnBlockLanded(GameObject block, string newTag)
    {
        block.tag = newTag;
    }

    void SetBlockTransparency(GameObject block, float alpha)
    {
        foreach (Renderer renderer in block.GetComponentsInChildren<Renderer>())
        {
            Material material = renderer.material;
            Color color = material.color;
            color.a = alpha;
            material.color = color;

            if (alpha < 1f)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }
        }
    }
}

// ����� ����� ��� ������������ �����������
public class LandDetector : MonoBehaviour
{
    private BlockSpawner spawner;
    private string targetTag;
    private bool hasLanded = false;

    private Rigidbody2D rb;
    public float maxFallSpeed = 20f; // ������������ �������� �������

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!hasLanded && rb != null)
        {
            // ������������ �������� �������
            if (rb.velocity.y < -maxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
            }
        }
    }

    public void Initialize(BlockSpawner spawnerRef, string tag)
    {
        spawner = spawnerRef;
        targetTag = tag;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground")) // ��������� ������������ � ������
        {
            hasLanded = true;
            spawner.OnBlockLanded(gameObject, targetTag);
            Destroy(this); // ������� ���������, ��� ��� �� ������ �� �����
        }
        if (!hasLanded && collision.gameObject.CompareTag("Block")) // ��������� ������������ � ������
        {
            hasLanded = true;
            spawner.OnBlockLanded(gameObject, targetTag);
            Destroy(this); // ������� ���������, ��� ��� �� ������ �� �����
        }
    }
}