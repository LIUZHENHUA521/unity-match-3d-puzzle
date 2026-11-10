using System;
using System.Collections.Generic;
using System.Globalization;

// Presentation data only. Creating or displaying an item never credits inventory.
public sealed class Match3DRewardItem {
    public int ToolIndex { get; private set; }
    public string Caption { get; private set; }
    public bool IsRedPacket => ToolIndex < 0;
    Match3DRewardItem(int tool, string caption) { ToolIndex = tool; Caption = caption; }

    public static Match3DRewardItem Tool(int index, int count) {
        if (index < 0 || index >= BoardTools.ToolCount) throw new ArgumentOutOfRangeException(nameof(index));
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        return new Match3DRewardItem(index, "×" + count.ToString(CultureInfo.InvariantCulture));
    }
    public static Match3DRewardItem RedPacket(decimal? yuan) {
        if (yuan.HasValue && yuan.Value <= 0) throw new ArgumentOutOfRangeException(nameof(yuan));
        return new Match3DRewardItem(-1, yuan.HasValue ? yuan.Value.ToString("0.##", CultureInfo.InvariantCulture) + "元" : "红包");
    }
    public static Match3DRewardItem[] FromTools(int[] quantities) {
        var result = new List<Match3DRewardItem>();
        if (quantities != null)
            for (int i = 0; i < Math.Min(quantities.Length, BoardTools.ToolCount); i++)
                if (quantities[i] > 0) result.Add(Tool(i, quantities[i]));
        return result.ToArray();
    }
}
