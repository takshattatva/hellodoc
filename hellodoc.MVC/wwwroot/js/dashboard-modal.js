const toggleButton = (curr_btn, redirect_page) => {
    const buttons = document.getElementsByClassName("common-btn")
    for (let i = 0; i < buttons.length; ++i) {
        if (buttons[i] != curr_btn) {
            buttons[i].classList.remove('create-request-active')
        }
        else {
            buttons[i].classList.add('create-request-active')
        }
    }
    document.getElementById("redirect-value").value = redirect_page;
}

const redirect = () => {
    window.location.assign(`./${document.getElementById("redirect-value").value}`);
}
