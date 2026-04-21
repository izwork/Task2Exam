document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;

    // Theme / contrast / tts / font size helpers
    function toggleDarkMode() {
        body.classList.toggle("dark-mode");
        localStorage.setItem("theme", body.classList.contains("dark-mode") ? "dark" : "light");
    }
    if (localStorage.getItem("theme") === "dark") body.classList.add("dark-mode");
    window.toggleDarkMode = toggleDarkMode;

    function toggleHighContrast() {
        body.classList.toggle("high-contrast");
        localStorage.setItem("contrast", body.classList.contains("high-contrast") ? "high" : "normal");
    }
    if (localStorage.getItem("contrast") === "high") body.classList.add("high-contrast");
    window.toggleHighContrast = toggleHighContrast;

    function speakText() {
        let text = document.body.innerText;
        let speech = new SpeechSynthesisUtterance(text);
        speech.lang = 'en-GB';
        window.speechSynthesis.speak(speech);
    }
    window.speakText = speakText;

    function changeFontSize(action) {
        let currentSize = parseInt(window.getComputedStyle(body).fontSize);
        if (action === 'increase') currentSize += 2;
        else if (action === 'decrease') currentSize -= 2;
        body.style.fontSize = currentSize + 'px';
        localStorage.setItem("fontSize", currentSize + "px");
    }
    // apply persisted font size if available
    if (localStorage.getItem("fontSize")) {
        body.style.fontSize = localStorage.getItem("fontSize");
    }
    window.changeFontSize = changeFontSize;

    // Sidebar toggle logic (runs after DOM ready)
    const toggle = document.getElementById('accessibility-toggle');
    const sidebar = document.getElementById('accessibility-sidebar');
    const overlay = document.getElementById('accessibility-overlay');

    if (!toggle || !sidebar || !overlay) return;

    function ensureOverlayVisibleForUI() {
        overlay.classList.remove('sr-only');
    }

    function openSidebar() {
        ensureOverlayVisibleForUI();
        sidebar.classList.add('open');
        overlay.classList.add('visible');
        toggle.setAttribute('aria-expanded', 'true');
        sidebar.setAttribute('aria-hidden', 'false');
        overlay.setAttribute('aria-hidden', 'false');

        const firstBtn = sidebar.querySelector('button');
        if (firstBtn) firstBtn.focus();
    }

    function closeSidebar() {
        sidebar.classList.remove('open');
        overlay.classList.remove('visible');
        toggle.setAttribute('aria-expanded', 'false');
        sidebar.setAttribute('aria-hidden', 'true');
        overlay.setAttribute('aria-hidden', 'true');

        toggle.focus();
    }

    // reset all accessibility settings: classes, saved prefs, speech, font size
    function resetAccessibility() {
        // stop any speaking
        if (window.speechSynthesis && window.speechSynthesis.speaking) {
            window.speechSynthesis.cancel();
        }

        // remove classes and inline styles
        body.classList.remove('dark-mode', 'high-contrast');
        body.style.fontSize = ''; // revert to CSS default / media-query

        // remove persisted keys (update if you add more)
        localStorage.removeItem('theme');
        localStorage.removeItem('contrast');
        localStorage.removeItem('fontSize');

        // close sidebar and give feedback by focusing toggle
        closeSidebar();

        // optional small visual feedback: brief flash on toggle (non-blocking)
        toggle.classList.add('accessibility-reset-flash');
        setTimeout(() => toggle.classList.remove('accessibility-reset-flash'), 600);
    }
    window.resetAccessibility = resetAccessibility;

    toggle.addEventListener('click', function (e) {
        e.preventDefault();
        if (sidebar.classList.contains('open')) closeSidebar();
        else openSidebar();
    });

    overlay.addEventListener('click', function () {
        closeSidebar();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && sidebar.classList.contains('open')) closeSidebar();
    });

    // Close on link clicks outside the sidebar
    document.addEventListener('click', function (e) {
        if (!sidebar.classList.contains('open')) return;
        const target = e.target;
        const clickedInsideSidebar = target.closest && target.closest('#accessibility-sidebar');
        const clickedToggle = target.closest && target.closest('#accessibility-toggle');
        if (!clickedInsideSidebar && !clickedToggle && target.closest && target.closest('a')) closeSidebar();
    });
});