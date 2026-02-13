## 2025-02-17 - [Password Toggle Accessibility]
**Learning:** Icon-only password toggle buttons are critical interactive elements that often lack accessible names, confusing screen reader users. Dynamic labels ("Show password" vs "Hide password") are essential.
**Action:** Always verify toggle buttons have `aria-label` that reflects their current state (e.g., using ternary operators like `@(isVisible ? "Hide" : "Show")`).
