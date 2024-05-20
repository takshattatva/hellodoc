function showPassword() {

    const togglePassword = document.querySelector("#togglePassword");
    const password = document.querySelector("#floatingPassword");

    const type = password.getAttribute("type") === "password" ? "text" : "password";
    password.setAttribute("type", type);

    document.getElementById("togglePassword").classList.toggle('bi-eye');
}
//****************************************************************************************************************************
function showPassword2() {

    const togglePassword = document.querySelector("#togglePassword2");
    const password = document.querySelector("#floatingPassword2");

    const type = password.getAttribute("type") === "password" ? "text" : "password";
    password.setAttribute("type", type);

    document.getElementById("togglePassword2").classList.toggle('bi-eye');
}
