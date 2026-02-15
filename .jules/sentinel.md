## 2024-05-24 - URL Injection in Repository Layer
**Vulnerability:** `FindAsync` methods in `ApiDespRep` and `ApiReceitaRep` were concatenating raw condition strings into URLs, allowing for URL injection and potentially broken requests.
**Learning:** Repositories were designed to accept raw "condition" strings (likely SQL fragments) but were not handling them safely for transport over HTTP.
**Prevention:** Always sanitize user-provided strings used in URLs with `Uri.EscapeDataString`. Add warnings if the underlying data model suggests direct SQL execution.
