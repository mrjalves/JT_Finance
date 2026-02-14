# Sentinel's Journal

## 2025-02-18 - UI SQL Construction and Unsafe Queries
**Vulnerability:** The `Calendario` component was constructing raw SQL strings (`WHERE MONTH(...)`) and passing them to the service layer. Additionally, the `ApiDespRep` and `ApiReceitaRep` were not URL-encoding the query parameters in `FindAsync`, allowing potential injection or URL breakage.
**Learning:** Constructing SQL in the UI bypasses repository abstraction and encourages SQL injection if user input is ever used. It also couples the UI to the database schema.
**Prevention:** Always use specific, strongly-typed repository methods (e.g., `GetReceitasPorMesAsync`) instead of generic `Find(string sql)` methods in the UI layer. Ensure all user-supplied query parameters are URL-encoded (`Uri.EscapeDataString`) before appending to HTTP requests.
