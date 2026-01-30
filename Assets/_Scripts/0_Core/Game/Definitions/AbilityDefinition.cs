using UnityEngine;

[CreateAssetMenu(menuName = "HoloOCG/Definitions/Ability Definition", fileName = "Ability_")]
public sealed class AbilityDefinition : ScriptableObject
{
    [SerializeField] private string abilityId;
    [SerializeField] private string displayName;
    [TextArea(2, 8)]
    [SerializeField] private string rulesText;

    public string AbilityId => abilityId;
    public string DisplayName => displayName;
    public string RulesText => rulesText;
}