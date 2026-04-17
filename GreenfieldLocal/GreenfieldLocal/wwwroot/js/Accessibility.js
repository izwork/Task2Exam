document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;

    // Add a toggle for dark mode
    function toggleDarkMode() {
    body.classList.toggle("dark-mode");
        localStorage.setItem("theme", body.classList.contains("dark-mode") ? "dark" : "light");
    }
    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-mode");
    }
    window.toggleDarkMode = toggleDarkMode;

    // Add a toggle for high contrast mode
    function toggleHighContrast() {
        body.classList.toggle("high-contrast");
        localStorage.setItem("contrast", body.classList.contains("high-contrast") ? "high" : "normal");
    }
    if (localStorage.getItem("contrast") === "high") {
        body.classList.add("high-contrast");
    }
    window.toggleHighContrast = toggleHighContrast;

    // Text to speech functionality
    function speakText() {
        let text = document.body.innerText;
        let speech = new SpeechSynthesisUtterance(text);
        speech.lang = 'en-GB';
        window.speechSynthesis.speak(speech);
    }
    window.speakText = speakText;

    // Add a toggle for text size
    function changeFontSize(action) {
        let currentSize = parseInt(window.getComputedStyle(body).fontSize);
        if (action === 'increase') {
            currentSize += 2;
        } else if (action === 'decrease') {
            currentSize -= 2;
        }
        body.style.fontSize = currentSize + 'px';
        localStorage.setItem("fontSize", currentSize + "px");
    }
    window.changeFontSize = changeFontSize;

});