// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Dark/Light mode toggle with localStorage persistence
(function () {
    const key = 'theme';
    const btnId = 'themeToggle';
    function apply(theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);
    }
    function save(theme) {
        try { localStorage.setItem(key, theme); } catch { }
    }
    document.addEventListener('DOMContentLoaded', function () {
        const btn = document.getElementById(btnId);
        if (!btn) return;
        btn.addEventListener('click', function () {
            const current = document.documentElement.getAttribute('data-bs-theme') || 'light';
            const next = current === 'light' ? 'dark' : 'light';
            apply(next); save(next);
        });
    });
})();
