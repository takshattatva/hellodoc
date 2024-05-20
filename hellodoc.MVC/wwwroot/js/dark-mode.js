document.getElementById('dark-mode-btn').addEventListener('click', () => {
    const mode = localStorage.getItem("mode");

    if (mode == 'light' || mode == null) {
        localStorage.setItem('mode', 'dark')
        document.getElementById("moon").classList.add("d-none")
        document.getElementById("sun").classList.remove("d-none")
        document.documentElement.setAttribute('data-bs-theme', 'dark');
    }
    else {
        localStorage.setItem('mode', 'light')
        document.getElementById("sun").classList.add("d-none")
        document.getElementById("moon").classList.remove("d-none")
        document.documentElement.setAttribute('data-bs-theme', 'light');
    }
})

const checkMode = () => {
    try {
        const mode = localStorage?.getItem("mode");

        if (mode == 'light' || mode == null) {
            document.getElementById("sun").classList.add("d-none")
            document.getElementById("moon").classList.remove("d-none")
            document.documentElement.setAttribute('data-bs-theme', 'light');
        }
        else {
            document.getElementById("moon").classList.add("d-none")
            document.getElementById("sun").classList.remove("d-none")
            document.documentElement.setAttribute('data-bs-theme', 'dark');
        }
    }
    catch (err) {
        alert("there was some issue in changing mode")
    }
}