using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectionUI : MonoBehaviour
{
    [Header("Cars (index = CarId)")]
    public string[] carNames;
    public Sprite[] carThumbnails; // opsiyonel

    [Header("UI Refs")]
    public Image previewImage;
    public TMP_Text nameText;
    public Button prevButton;
    public Button nextButton;

    private int index;

    private void Awake()
    {
        if (prevButton) prevButton.onClick.AddListener(Prev);
        if (nextButton) nextButton.onClick.AddListener(Next);
    }

    private void Start()
    {
        if (carNames == null || carNames.Length == 0)
        {
            Debug.LogError("CarSelectionUI: carNames boþ!");
            return;
        }

        index = Mathf.Clamp(SelectionStore.SelectedCarId, 0, carNames.Length - 1);
        Refresh();
    }

    private void Prev()
    {
        if (carNames == null || carNames.Length == 0) return;
        index = (index - 1 + carNames.Length) % carNames.Length;
        Refresh();
    }

    private void Next()
    {
        if (carNames == null || carNames.Length == 0) return;
        index = (index + 1) % carNames.Length;
        Refresh();
    }

    private void Refresh()
    {
        //  SEÇÝMÝ KAYDET
        SelectionStore.SelectedCarId = index;

        if (nameText) nameText.text = carNames[index];

        if (previewImage)
        {
            if (carThumbnails != null && carThumbnails.Length > index && carThumbnails[index] != null)
            {
                previewImage.enabled = true;
                previewImage.sprite = carThumbnails[index];
                previewImage.preserveAspect = true;
            }
            else
            {
                previewImage.enabled = false;
            }
        }
    }
}