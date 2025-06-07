using UnityEngine;
using TMPro;
using System.Collections;

public class MergeBlocks : MonoBehaviour
{
    public int blockType;
    public GameObject nextBlockPrefab;
    public ParticleSystem mergeEffectPrefab;
    public AudioClip mergeSound;
    [Range(0, 1)] public float mergeSoundVolume = 0.7f; // Локальный множитель громкости для этого звука
    private bool isMerging = false;
    public bool IsMerging => isMerging; // Только для чтения

    [Header("Notification Settings")]
    public Sprite blockSprite; // Добавьте спрайт для этого блока
    public int blockLevel = 1; // Уровень блока (1, 2, 3...)

    // Добавляем статический список для отслеживания разблокированных уровней
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

                // Используем AudioManager вместо прямого вызова PlayClipAtPoint
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

                // Получаем компонент нового блока для уведомления
                MergeBlocks newBlockComponent = newBlock.GetComponent<MergeBlocks>();

                // Проверка на максимальный уровень
                if (newBlockComponent.nextBlockPrefab == null)
                {
                    // Сохраняем факт достижения максимального уровня
                    PlayerPrefs.SetInt("MaxBlockReached", 1);
                    PlayerPrefs.Save();

                    // Разблокируем уровни
                    UnlockLevelsForMaxBlock();
                }

                // Проверяем, является ли новый блок максимальным (у него нет nextBlockPrefab)
                if (newBlockComponent.nextBlockPrefab == null && GameBoundsManager.Instance != null)
                {
                    GameBoundsManager.maxBlocksAmount++;
                    GameBoundsManager.Instance.ReduceBounds();
                }

                // Проверяем, был ли этот уровень уже разблокирован
                bool isNewLevel = !unlockedLevels.Contains(newBlockComponent.blockLevel);

                // Если это новый уровень, добавляем его в список разблокированных
                if (isNewLevel)
                {
                    unlockedLevels.Add(newBlockComponent.blockLevel);
                }

                // Показываем уведомление только для новых уровней
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