namespace ChordPathFinder.Models;

public static class ChordGraph
{
    public static readonly IReadOnlyList<ChordDefinition> Chords = new List<ChordDefinition>
    {
        new("A", new[] { "Bm", "Dbm", "F#m" }),
        new("Ab", new[] { "Bbm", "Cm", "Fm" }),
        new("Abm", new[] { "B", "Dbm", "Ebm", "E", "F#" }),
        new("Am", new[] { "C", "Dm", "Em", "F", "G" }),
        new("B", new[] { "Abm", "Dbm", "Ebm" }),
        new("Bb", new[] { "Dm", "Gm", "Cm" }),
        new("Bbm", new[] { "Ab", "Db", "Ebm", "Fm", "F#" }),
        new("Bm", new[] { "A", "C", "D", "Em", "F#m", "G" }),
        new("C", new[] { "Am", "Dm", "Em", "G" }),
        new("Cm", new[] { "Ab", "Bb", "Eb", "Fm", "Gm" }),
        new("D", new[] { "Bm", "F#m", "Fm" }),
        new("Db", new[] { "Bbm", "Ebm", "Fm" }),
        new("Dbm", new[] { "A", "Abm", "B", "E", "F#m" }),
        new("Dm", new[] { "Am", "Bb", "C", "F", "Gm" }),
        new("E", new[] { "Abm", "Dbm", "F#m" }),
        new("Eb", new[] { "Cm", "Fm", "Gm" }),
        new("Ebm", new[] { "Abm", "B", "Bbm", "Db", "F#" }),
        new("Em", new[] { "Am", "Bm", "C", "G" }),
        new("F", new[] { "Am", "Bb", "Dm", "Gm" }),
        new("F#", new[] { "Abm", "Bbm", "Dbm", "Ebm", "E" }),
        new("F#m", new[] { "A", "Bm", "Dbm", "D", "E" }),
        new("Fm", new[] { "Ab", "Bbm", "Cm", "Db", "D", "Eb" }),
        new("G", new[] { "Am", "Bm", "C", "Em" }),
        new("Gm", new[] { "Bb", "Cm", "Dm", "Eb", "F" })
    };

    public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Graph =
        Chords.ToDictionary(chord => chord.Name, chord => chord.Transitions);

    public static readonly IReadOnlyList<string> OrderedChordNames = Chords.Select(x => x.Name).ToList();
}
