using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HoloOCG/Definitions/Card Definition", fileName = "Card_")]
public sealed class CardDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string cardId;
    [SerializeField] private string displayName;

    [Header("Rules Text")]
    [TextArea(2, 10)]
    [SerializeField] private string description;

    [Header("Abilities (data only; resolved by rules engine later)")]
    [SerializeField] private List<AbilityDefinition> abilities = new();

    public string CardId => cardId;
    public string Name => displayName;
    public string Description => description;
    public IReadOnlyList<AbilityDefinition> Abilities => abilities;
}