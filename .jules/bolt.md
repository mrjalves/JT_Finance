## GUID Generation in Blazor Components
- Avoid generating unique IDs (e.g., `Guid.NewGuid()`) in `OnParametersSet`.
- Use `OnInitialized` instead to ensure the ID is generated only once per component lifecycle.
- This prevents unnecessary allocations on every render cycle and ensures ID stability for DOM associations and accessibility.
