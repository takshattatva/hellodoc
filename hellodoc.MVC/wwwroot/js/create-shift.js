var today = new Date();
var formattedDate = today.toISOString().split('T')[0];

document.getElementById('StartDate').min = formattedDate;

document.getElementById("StartDate").addEventListener("change", function () {
    console.log(document.getElementById('StartDate').value);
    if (document.getElementById('StartDate').value == formattedDate) {

        const currentHours = today.getHours().toString().padStart(2, "0");
        const currentMinutes = today.getMinutes().toString().padStart(2, "0");
        const currentTime = `${currentHours}:${currentMinutes}`;

        document.getElementById('StartTime').min = currentTime;
    }
    else {
        document.getElementById('StartTime').min = '00:00';
    }
});

document.getElementById('StartTime').addEventListener('change', function () {
    var startTimeValue = this.value.split(':');
    var startTime = new Date();
    startTime.setHours(startTimeValue[0], startTimeValue[1], 0);
    startTime.setHours(startTime.getHours() + 1); // Set the start time to 1 hour later
    document.getElementById('EndTime').min = startTime.toTimeString().slice(0, 5);

    if (document.getElementById('EndTime').value < this.value) {
        document.getElementById('EndTime').value = this.value;
    }
});

var startTimeInput = document.getElementById('StartTime');
var endTimeInput = document.getElementById('EndTime');

startTimeInput.addEventListener('input', function () {
    var startTimeValue = this.value.split(':');
    var startTime = new Date();
    startTime.setHours(startTimeValue[0], startTimeValue[1], 0);
    startTime.setHours(startTime.getHours() + 1); // Set the start time to 1 hour later
    endTimeInput.min = startTime.toTimeString().slice(0, 5);

    if (endTimeInput.value < this.value) {
        endTimeInput.value = this.value;
    }
});

window.onload = toggleCheckboxes;
function toggleCheckboxes() {
    var repeatCheckbox = document.getElementById('Isrepeat');
    var refile = document.getElementById('Refile');
    var checkboxes = document.querySelectorAll('.Every');

    if (!repeatCheckbox.checked) {
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
            checkbox.disabled = true;
            refile.disabled = true;
        });
    } else {
        checkboxes.forEach(function (checkbox) {
            checkbox.disabled = false;
            refile.disabled = false;
        });
    }
}

function menubox() {
    event.preventDefault();
    let checkboxes = document.querySelectorAll('.Every:checked');
    let repeatDays = [];
    checkboxes.forEach((checkbox) => {
        repeatDays.push(checkbox.value);
    });
    document.querySelector('#checkWeekday').value = repeatDays.join(',');
    console.log(document.querySelector('#checkWeekday').value);
};