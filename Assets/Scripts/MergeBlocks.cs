using UnityEngine;
using TMPro;
using System.Collections;

public class MergeBlocks : MonoBehaviour
{
    public int blockType;
    public GameObject nextBlockPrefab;
    public ParticleSystem mergeEffectPrefab;
    public AudioClip mergeSound;
    [Range(0, 1)] public float mergeSoundVolume = 0.7f; // ��������� ��������� ��������� ��� ����� �����
    private bool isMerging = false;
    public bool IsMerging => isMerging; // ������ ��� ������

    [Header("Notification Settings")]
    public Sprite blockSprite; // �������� ������ ��� ����� �����
    public int blockLevel = 1; // ������� ����� (1, 2, 3...)

    // ��������� ����������� ������ ��� ������������ ���������������� �������
    private static System.Collections.Generic.HashSet<int> unlockedLevels = new System.Collections.Generic.HashSet<int>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMerging) return;

        if (collision.gameObject.CompareTag("Block") && collision.gameObject != gameObject)
        {
            MergeBlocks otherBlock = collision.gameObject.GetComponent<MergeBlocks>();

            if (otherBlock != null &&
                otherBlock.blockType == blockType &&
                nextBlockPrefab != null &&
                !otherBlock.isMerging)
            {
                isMerging = true;
                otherBlock.isMerging = true;

                Vector2 mergePosition = (transform.position + collision.transform.position) / 2f;

                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.CompleteStep("MergeBlocks");
                }

                // ���������� AudioManager ������ ������� ������ PlayClipAtPoint
                if (mergeSound != null)
                {
                    AudioManager.PlaySound(mergeSound, mergePosition, mergeSoundVolume);
                }
                else
                {
                    Debug.LogWarning("Merge sound is not assigned!");
                }

                if (mergeEffectPrefab != null)
                {
                    ParticleSystem effect = Instantiate(mergeEffectPrefab, mergePosition, Quaternion.identity);
                    effect.Play();
                    Destroy(effect.gameObject, effect.main.duration);
                }
                else
                {
                    Debug.LogWarning("Merge effect prefab is not assigned!");
                }

                Destroy(collision.gameObject);
                Destroy(gameObject);

                GameObject newBlock = Instantiate(nextBlockPrefab, mergePosition, Quaternion.identity);

                // �������� ��������� ������ ����� ��� �����������
                MergeBlocks newBlockComponent = newBlock.GetComponent<MergeBlocks>();

                // �������� �� ������������ �������
                if (newBlockComponent.nextBlockPrefab == null)
                {
                    // ��������� ���� ���������� ������������� ������
                    PlayerPrefs.SetInt("MaxBlockReached", 1);
                    PlayerPrefs.Save();

                    // ������������ ������
                    UnlockLevelsForMaxBlock();
                }

                // ���������, �������� �� ����� ���� ������������ (� ���� ��� nextBlockPrefab)
                if (newBlockComponent.nextBlockPrefab == null && GameBoundsManager.Instance != null)
                {
                    GameBoundsManager.maxBlocksAmount++;
                    GameBoundsManager.Instance.ReduceBounds();
                }

                // ���������, ��� �� ���� ������� ��� �������������
                bool isNewLevel = !unlockedLevels.Contains(newBlockComponent.blockLevel);

                // ���� ��� ����� �������, ��������� ��� � ������ ����������������
                if (isNewLevel)
                {
                    unlockedLevels.Add(newBlockComponent.blockLevel);
                }

                // ���������� ����������� ������ ��� ����� �������
                if (BlockUnlockNotification.Instance != null &&
                    newBlockComponent != null &&
                    isNewLevel)
                {
                    BlockUnlockNotification.Instance.ShowNotification(
                        blockSprite,
                        newBlockComponent.blockSprite,
                        newBlockComponent.blockLevel
                    );
                }

                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(blockType * 10);
                }
                else
                {
                    Debug.LogWarning("ScoreManager.Instance is null!");
                }
            }
        }
    }

    private void UnlockLevelsForMaxBlock()
    {
        foreach (var button in FindObjectsOfType<LevelButtonController>())
        {
            button.CheckUnlockConditions();
        }
    }
}