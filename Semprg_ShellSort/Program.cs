// Arrange
var rng = new Random(42069);
var data = Enumerable.Range(1, 1_000_000)
    .Select(_ => rng.Next(1, 100))
    .ToArray();

// Sort
var sorted1 = DumbSpeedMeasure(delegate {
    var sorted = data.ToArray();
    ShellSort_HalfGap(ref sorted);
    return sorted;
});
var sorted2 = DumbSpeedMeasure(delegate
{
    var sorted = data.ToArray();
    ShellSort_GapSet(ref sorted, [ 1750, 701, 301, 132, 57, 23, 10, 4, 1 ]);
    return sorted;
});

var sorted3 = DumbSpeedMeasure(delegate
{
    var sorted = data.ToArray();
    ShellSort_HibbardsGaps(ref sorted);
    return sorted;
});

// Verify
var sortedVerified = data.OrderBy(x => x).ToArray();
Console.WriteLine($"1: {sorted1.SequenceEqual(sortedVerified)}");
Console.WriteLine($"2: {sorted2.SequenceEqual(sortedVerified)}");
Console.WriteLine($"3: {sorted3.SequenceEqual(sortedVerified)}");

static void ShellSort_HalfGap<T>(ref T[] array)
    where T : IComparable<T>
{
    var n = array.Length;
    var gap = n / 2;

    while (gap > 0)
    {
        for (var i = gap; i < n; i++)
        {
            var temp = array[i];
            var j = i;

            // This loop goes to the left (from i) while the element after gap
            // is smaller than temp (the element we are currently using for comparing).
            for (
                ;
                j >= gap && array[j - gap].CompareTo(temp) > 0;
                j -= gap)
            {
                // Shift (not swap). This will not lose data, because we always shift the
                // number after gap to the current j position. And at the end we set temp
                // to where we ended, essentialy moving the temp value from right to left.
                array[j] = array[j - gap];
            }

            array[j] = temp;
        }

        gap /=2;
    }
}

static void ShellSort_GapSet<T>(ref T[] array, IEnumerable<int> gapSetFromLargestToSmallest)
    where T : IComparable<T>
{
    var n = array.Length;

    foreach (var gap in gapSetFromLargestToSmallest)
    {
        // Skip if gap is larger than the array size
        // (Or we could binary search for the largest gap that is less than n)
        if (gap > n)
            continue;

        for (var i = gap; i < n; i++)
        {
            var temp = array[i];
            var j = i;

            for (
                ;
                j >= gap && array[j - gap].CompareTo(temp) > 0;
                j -= gap)
            {
                array[j] = array[j - gap];
            }

            array[j] = temp;
        }
    }
}

static void ShellSort_HibbardsGaps<T>(ref T[] array)
    where T : IComparable<T>
{
    ShellSort_GapSet(ref array, GenerateHibbardGaps(array.Length).Reverse());

    IEnumerable<int> GenerateHibbardGaps(int size)
    {
        var k = 1;
        var gap = 1;

        while (gap < size)
        {
            yield return gap;
            k++;
            gap = (1 << k) - 1; // Equivalent to 2^k - 1
        }
    }
}

static T DumbSpeedMeasure<T>(Func<T> @delegate)
{
    var snapshot = System.Diagnostics.Stopwatch.GetTimestamp();
    var result = @delegate();
    var elapsed = System.Diagnostics.Stopwatch.GetElapsedTime(snapshot);
    Console.WriteLine($"Elapsed time: {elapsed}");

    return result;
}