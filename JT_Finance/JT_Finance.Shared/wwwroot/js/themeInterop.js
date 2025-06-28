// themeInterop.js

export function getSystemPreference() {
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

export function setDocumentTheme(themeName) {
    // Remove any existing theme classes/attributes to avoid conflicts
    document.documentElement.removeAttribute('data-theme-preference'); // User's choice: light, dark, or system
    document.documentElement.removeAttribute('data-theme-effective');   // Actual theme applied: light or dark

    let effectiveTheme = themeName;
    if (themeName === 'system') {
        effectiveTheme = getSystemPreference();
    }

    document.documentElement.setAttribute('data-theme-preference', themeName);
    document.documentElement.setAttribute('data-theme-effective', effectiveTheme);
    console.log(`Document theme set. Preference: ${themeName}, Effective: ${effectiveTheme}`);
}

export function getLocalStorage(key) {
    return localStorage.getItem(key);
}

export function setLocalStorage(key, value) {
    localStorage.setItem(key, value);
}

// Listener for system theme changes if the current preference is 'system'
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
    const currentPreference = document.documentElement.getAttribute('data-theme-preference');
    if (currentPreference === 'system') {
        const newEffectiveTheme = event.matches ? 'dark' : 'light';
        document.documentElement.setAttribute('data-theme-effective', newEffectiveTheme);
        console.log(`System theme changed. New effective theme: ${newEffectiveTheme}`);
        // Optionally, notify Blazor side if needed for C# logic reacting to system changes.
        // This might involve DotNet.invokeMethodAsync if Blazor needs to re-render based on this.
    }
});
