using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class SymbolVisualAnimation : NetworkBehaviour
{
    [SerializeField] private Transform visualChild; // Drag the "Cross" or "Circle" child here
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float overshootAmount = 3f;

    public override void OnNetworkSpawn()
    {
        // This code runs on EVERY client when the object appears
        if (visualChild != null)
        {
            AnimateIn();
        }
    }

    private void AnimateIn()
    {
        float targetScaleValue = 2.5f;
        Vector3 targetScale = new Vector3(targetScaleValue, targetScaleValue, targetScaleValue);

        // 1. Start from nothing
        visualChild.localScale = Vector3.zero;

        visualChild.DOScale(targetScale, animationDuration)
            .SetEase(Ease.OutBack, overshootAmount);

        visualChild.DOPunchRotation(new Vector3(0, 0, 15f), animationDuration);
    }
}
