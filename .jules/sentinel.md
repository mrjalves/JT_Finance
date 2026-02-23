## Security Learnings

- **PII Leakage in Logs**: Avoid using `Console.WriteLine` to log sensitive user information like email addresses, even for debugging purposes in development. These logs can persist in production environments (e.g., browser console for Blazor WASM or server logs for Blazor Server) and expose user data. (Added Sat Feb 21 01:13:09 UTC 2026)
