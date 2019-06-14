using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapArenaInterface : MonoBehaviour
{
    [SerializeField] GameObject arenaPanel;
    [SerializeField] Text arenaNameText;
    [SerializeField] Text arenaDescriptionText;
    [SerializeField] GameButton playButton;
    [SerializeField] GameButton closeButton;
    [SerializeField] Image[] starsImages;
    [SerializeField] GameButton openInventoryButton;

    [Header("Arena Type")]
    [SerializeField] Image arenaTypeImage;
    [SerializeField] Sprite clearWaveArenaSprite;
    [SerializeField] Sprite escapeArenaSprite;
    [SerializeField] Sprite chaseArenaSprite;
    [SerializeField] Sprite assassinationArenaSprite;

    public void SetUp(GameButton.InteractionDeleguate playInteraction, GameButton.InteractionDeleguate closeInteraction)
    {
        playButton.Interaction = playInteraction;
        closeButton.Interaction = closeInteraction;
    }

    public void OpenArenaPanel(string arenaName, string arenaDescription, int starsNumber, ArenaGameMode arenaGameMode)
    {
        arenaPanel.SetActive(true);
        openInventoryButton.gameObject.SetActive(false);

        if (arenaNameText != null)
            arenaNameText.text = arenaName;

        if (arenaDescriptionText != null)
            arenaDescriptionText.text = arenaDescription;

        for(int i = 0; i < starsImages.Length && i < starsNumber; i++)
        {
            starsImages[i].gameObject.SetActive(true);
        }

        for (int i = starsNumber; i < starsImages.Length; i++)
        {
            starsImages[i].gameObject.SetActive(false);
        }

        if (arenaTypeImage != null)
        {            
            switch (arenaGameMode)
            {
                case (ArenaGameMode.WavesClearing):
                    arenaTypeImage.sprite = clearWaveArenaSprite;
                    break;

                case (ArenaGameMode.Escape):
                    arenaTypeImage.sprite = escapeArenaSprite;
                    break;

                case (ArenaGameMode.EnemyShipPursuit):
                    arenaTypeImage.sprite = chaseArenaSprite;
                    break;

                case (ArenaGameMode.Assassination):
                    arenaTypeImage.sprite = assassinationArenaSprite;
                    break;
            }
        }
    }

    public void CloseArenaPanel()
    {
        arenaPanel.SetActive(false);
        openInventoryButton.gameObject.SetActive(true);
    }
}
