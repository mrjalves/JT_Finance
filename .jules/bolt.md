## 2024-05-23 - [C# LINQ Optimization]
**Learning:** Checking `LevenshteinDistance` inside a LINQ `Where` clause without short-circuiting or pre-checks can be extremely slow (O(N*M) per item).
**Action:** Always check cheaper conditions first (`Contains`, length difference) before expensive fuzzy matching. Hoist invariant calculations like `.ToLower()` out of loops.
