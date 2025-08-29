using static System.Text.Json.JsonElement;

namespace TestUtil.Extension;

public static class ArrayEnumeratorExtensions
{
    public static List<string?>? ToListString(this ArrayEnumerator array)
    {
        return array.Select(p => p.GetString()).ToList();
    }
}
