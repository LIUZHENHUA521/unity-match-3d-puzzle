using System.Collections.Generic;

public enum ProbePickSource { Manual, Probe, Automatic }

// Logical state is committed in pickup order, independently of concurrent flight animations.
public sealed class ComboProbeState {
    public const int ColumnCount = 4;
    readonly List<long>[] columns = { new List<long>(), new List<long>(), new List<long>(), new List<long>() };
    readonly HashSet<long> bound = new HashSet<long>();
    long lastOperation;
    public int Combo { get; private set; }
    public int Stars { get; private set; }
    public int NextColumn { get; private set; }
    public int Revision { get; private set; }
    public int Count => bound.Count;
    public IReadOnlyList<long> Column(int index) => columns[index];
    public long Top(int index) => columns[index].Count == 0 ? 0 : columns[index][columns[index].Count - 1];
    public bool Contains(long id) => bound.Contains(id);

    public bool Pick(long operation, ProbePickSource source, bool matched) {
        if (operation <= lastOperation) return false;
        lastOperation = operation;
        if (source == ProbePickSource.Automatic) return false;
        if (matched) {
            Combo++; Stars = 3; Revision++;
            return Combo >= 2 && source == ProbePickSource.Manual;
        }
        if (Combo > 0) {
            Stars--;
            if (Stars <= 0) { Stars = 0; Combo = 0; }
            Revision++;
        }
        return false;
    }
    public bool Add(long id) {
        if (id <= 0 || !bound.Add(id)) return false;
        columns[NextColumn].Add(id);
        NextColumn = (NextColumn + 1) % ColumnCount;
        Revision++; return true;
    }
    public bool Remove(long id) {
        if (!bound.Remove(id)) return false;
        foreach (var column in columns) column.Remove(id);
        Revision++; return true;
    }
    public void RestoreReplay(int combo,int stars,int next,IEnumerable<long>[] values) {
        Reset();Combo=combo;Stars=stars;NextColumn=next;
        for(int i=0;i<ColumnCount;i++)foreach(long token in values[i]){columns[i].Add(token);bound.Add(token);}
    }
    public void Reset() {
        foreach (var column in columns) column.Clear();
        bound.Clear(); Combo = Stars = NextColumn = 0; lastOperation = 0; Revision++;
    }
}
