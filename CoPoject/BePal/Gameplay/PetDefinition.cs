#nullable enable
namespace BePal.Gameplay;

public sealed class PetDefinition
{
    public PetKind Kind { get; init; }
    public string Name { get; init; } = string.Empty;
    public int HazardLevel { get; init; }
    public HarmType HarmType { get; init; }
    public ActionPattern Pattern { get; init; } = new();
}
