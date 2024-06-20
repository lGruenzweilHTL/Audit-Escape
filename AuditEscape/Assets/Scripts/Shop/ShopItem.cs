using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
    [SerializeField, Space] private ShopManager manager;
    
    [SerializeField] private float collapsedSize, expandedSize;
    [SerializeField] private TMP_Text nameText, costText, descriptionText;
    [SerializeField] private Button dropdownButton, buyButton;
    [SerializeField] private string title;
    [SerializeField, TextArea(2, 3)] private string description;
    [SerializeField] private int cost, upgradeIndex;

    private bool isExpanded;
    private RectTransform rect;

    [ContextMenu("Apply Values")]
    private void ApplyValues() {
        nameText.text = title;
        descriptionText.text = description;
        costText.text = $"{cost} €";
        dropdownButton.onClick.RemoveAllListeners();
        descriptionText.gameObject.SetActive(false);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, collapsedSize);
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();
    }

    private void Start() {
        if (manager == null) {
            Debug.LogError($"ShopManager of item {name} has not been assigned. Initialization will not be done", this);
            return;
        }
        
        buyButton.onClick.AddListener(() => manager.UpgradeItem(upgradeIndex, cost));
        ApplyValues();
        dropdownButton.onClick.AddListener(ToggleExpand);
    }

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