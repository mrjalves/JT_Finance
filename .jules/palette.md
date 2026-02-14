## 2025-02-27 - [Sortable Grid Accessibility]
**Learning:** DataGrid components in this app implement sortable headers using `role="button"` on `<th>` elements but lack `tabindex="0"` and keyboard event handlers (`onkeydown`). This makes table sorting inaccessible to keyboard users despite having a clickable role.
**Action:** When working on DataGrids in this project, always verify that sortable headers include `tabindex="0"` and handle `Enter`/`Space` keys via `@onkeydown` to ensure keyboard accessibility. Ensure `aria-sort` is also dynamically updated.
