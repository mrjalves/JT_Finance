# Sentinel Security Learnings

## PII Logging Vulnerability

### Vulnerability Description
Sensitive Data Logging: The application was logging Personally Identifiable Information (PII), specifically user email addresses, to the console during login, registration, and password recovery attempts.

### Impact
Logging PII can lead to:
- Privacy violations (e.g., GDPR, CCPA).
- Exposure of sensitive user information if logs are accessed by unauthorized parties.
- Increased risk of targeted attacks (e.g., phishing) if email addresses are leaked.

### Remediation
- Removed PII from `Console.WriteLine` calls in `RecuperarSenha.razor`, `Cadastro.razor`, and `Login.razor`.
- Replaced sensitive log messages with generic, non-sensitive ones.
- In a production environment, use a proper logging framework and ensure that PII is never logged at any log level unless absolutely necessary and properly protected (e.g., hashed or masked).
