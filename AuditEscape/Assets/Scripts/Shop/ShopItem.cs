using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
    [SerializeField] private ShopManager manager;

    [SerializeField] private float collapsedSize, expandedSize;
    [SerializeField] private TMP_Text nameText, costText, descriptionText, levelText;
    [SerializeField] private Button dropdownButton, buyButton;
    [SerializeField] private string title;
    [SerializeField, TextArea(2, 3)] private string description;
    [SerializeField] private int cost, upgradeIndex;

    [Tooltip("Set to -1 for infinite levels"), SerializeField]
    private int maxLevel;

    private int currLevel = 0;
    private bool isExpanded;
    private RectTransform rect;

    [ContextMenu("Apply Values")]
    private void ApplyValues() {
        nameText.text = title;
        descriptionText.text = description;
        costText.text = $"{cost * manager.DiscountMultiplier} €";
        dropdownButton.onClick.RemoveAllListeners();
        descriptionText.gameObject.SetActive(false);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, collapsedSize);
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();
    }

    private void Start() {
        if (manager == null) {
            Debug.LogError($"ShopManager of item {name} has not been assigned. Will not initialize", this);
            return;
        }

        buyButton.onClick.AddListener(() => {
            if (manager.UpgradeItem(upgradeIndex, cost, currLevel, maxLevel)) {
                currLevel++;
                SetLevelText();
            }
        });
        manager.OnDiscountChanged += ApplyValues;

        ApplyValues();
        dropdownButton.onClick.AddListener(ToggleExpand);
        SetLevelText();
    }

    private void SetLevelText() => levelText.text = maxLevel == -1 ? "∞" : $"{currLevel}/{maxLevel}";

    private void ToggleExpand() {
        isExpanded = !isExpanded;
        if (!isExpanded) descriptionText.gameObject.SetActive(false);

        Vector2 targetSize = new(rect.rect.size.x, isExpanded ? expandedSize : collapsedSize);
        GetComponent<RectTransform>().DOSizeDelta(targetSize, 0.2f).onComplete +=
            () => descriptionText.gameObject.SetActive(isExpanded);

        RectTransform dropdown = dropdownButton.GetComponent<RectTransform>();
        Vector3 targetRot = dropdown.rotation.eulerAngles;
        targetRot.z += isExpanded ? 180 : -180;
        dropdown.DORotate(targetRot, 0.2f);
    }
}