## 2025-05-18 - Unsafe Repository Pattern
**Vulnerability:** The repository layer (e.g., `DesktopDespRep.cs`) exposes a `Find(string condicao)` method that accepts raw SQL WHERE clauses from the service layer.
**Learning:** This pattern encourages SQL injection if user input is ever concatenated into the `condicao` string in the service layer (which is external to this repo). The architecture fundamentally relies on dynamic SQL generation without parameterization in these methods.
**Prevention:** Future refactoring should introduce parameterized query methods (e.g., `FindByDate(DateTime date)`) instead of exposing raw SQL strings, or use a proper ORM query builder.
