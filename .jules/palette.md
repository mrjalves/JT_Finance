## 2024-03-24 - Blazor DataGrid Keyboard Sorting
**Learning:** Default Blazor sort headers using `role="button"` without `tabindex` and `onkeydown` are inaccessible to keyboard users.
**Action:** Always add `tabindex="0"` and listen for `Enter`/`Space` keys on custom sort headers.
