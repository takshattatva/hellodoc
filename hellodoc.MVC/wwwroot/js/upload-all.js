const tickAll = () => {
    const checkboxes = document.getElementsByClassName("checkbox")
    if (document.getElementsByClassName("checkbox-main")[0].checked == true) {
        for (let i = 0; i < checkboxes.length; ++i) {
            checkboxes[i].checked = true;
        }
    }
    else {
        for (let i = 0; i < checkboxes.length; ++i) {
            checkboxes[i].checked = false;
        }
    }
}

const allCheck = () => {
    const checkboxes = document.getElementsByClassName("checkbox")
    let flag = 0
    for (let i = 0; i < checkboxes.length; ++i) {
        if (checkboxes[i].checked == false) {
            flag = 1
            break
        }
    }
    if (flag == 0) {
        document.getElementsByClassName("checkbox-main")[0].checked = true;
    }
    else {
        document.getElementsByClassName("checkbox-main")[0].checked = false;
    }
}