## 2025-05-24 - Blazor Computed Properties
**Learning:** Blazor components frequently access property getters during rendering. Using LINQ queries (e.g., `Sum()`, `Count()`) directly in these getters causes significant performance overhead (O(N) per render).
**Action:** Cache these values in backing fields and update them only when the underlying data changes (e.g., in `OnParametersSet`, data loading methods, or filter applications).
