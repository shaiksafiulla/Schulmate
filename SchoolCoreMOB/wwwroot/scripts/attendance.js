// Initialize the current date and other variables
let currentMonth = new Date().getMonth();
let currentYear = new Date().getFullYear();

// Global variables to hold attendance and leave data
let totalAttendance = [];
let filterAttendance = [];
let leaveRequests = [];
let filterleaveRequests = [];

// Function to render the calendar with custom parameters
function renderCalendar(month, year) {
    // console.log(month, year, attendUrl, leaveUrl, studentId);
    const monthYear = document.getElementById("monthYear");
    const datesContainer = document.getElementById("dates");

    monthYear.textContent = `${getMonthName(month)} ${year}`;
    datesContainer.innerHTML = ""; // Clear the previous month's dates

    const firstDay = new Date(year, month, 1).getDay();
    const numDays = new Date(year, month + 1, 0).getDate();

    // Add empty cells before the first day of the month
    for (let i = 0; i < firstDay; i++) {
        const emptyCell = document.createElement("div");
        emptyCell.classList.add("date");
        datesContainer.appendChild(emptyCell);
    }
    filterAttendance = totalAttendance.filter(item => {
        return item.month === month + 1;
    });
    filterleaveRequests = leaveRequests.filter(item => {
        return item.month === month + 1;
    });
    if (filterAttendance) {
        // Add the days of the month
        for (let j = 1; j <= numDays; j++) {
            const dateCell = document.createElement("div");
            dateCell.classList.add("date");
            dateCell.textContent = j;

            if (filterAttendance.length > 0) {
                let objDay = filterAttendance.find(filter => filter.day === j);
                if (objDay !== undefined && objDay !== null) {
                    if (objDay.isPresent === '0') {
                       // dateCell.style.backgroundColor = 'red';
                        const dot = document.createElement("div");
                        dot.classList.add("event-dot");
                        dot.style.backgroundColor = 'red'; // Set the dot's color to the event's background color
                        dateCell.appendChild(dot); // Append the dot to the date cell
                    }
                }
            }

            datesContainer.appendChild(dateCell);
        }
        displayAttendanceSummary();
    }
    if (filterleaveRequests) {
       
        displayLeaveRequests();
    }   
}


function getMonthName(month) {
    const monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    return monthNames[month];
}


// AJAX function to get total attendance data with dynamic URL and studentId
function fetchAttendanceData(month, year, attendUrl, leaveUrl, token, userparam) {
    $.ajax({
        url: attendUrl,
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        headers: {
            'Authorization': 'Bearer ' + token,
            'UserParam': userparam
        },
        success: function (response) {

            totalAttendance = response;
            const grandsummaryContainer = document.getElementById("grandAttendanceSummary");
            grandsummaryContainer.innerHTML = `                      

            <div style="display: flex; justify-content: space-between; gap: 5px;">
                <div>
                    Total Working: <span class="badge bg-blue-light font-11">${totalAttendance.length}</span>
                </div>
                <div>
                    Total Absent: <span class="badge bg-red-light font-11">${totalAttendance.filter(x => x.isPresent === '0').length}</span>
                </div>
                <div>
                    Total Present: <span class="badge bg-green-light font-11">${totalAttendance.filter(x => x.isPresent === '1').length}</span>
                </div>
            </div>`;


            fetchLeaveRequests(month, year, attendUrl, leaveUrl, token, userparam)

        },
        error: function () {
            console.error("Error fetching attendance data");
        }
    });
}

function fetchLeaveRequests(month, year, attendUrl, leaveUrl, token, userparam) {
    $.ajax({
        url: leaveUrl,
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        headers: {
            'Authorization': 'Bearer ' + token,
            'UserParam': userparam
        },
        success: function (childresponse) {            
            leaveRequests = childresponse;
            renderCalendar(month, year, attendUrl, leaveUrl, token, userparam); // Re-render calendar with data
        },
        error: function () {
            console.error("Error fetching Leave Request data");
        }
    });
}

// Function to display the attendance summary
function displayAttendanceSummary() {
    const summaryContainer = document.getElementById("attendanceSummary");
    summaryContainer.innerHTML = `

 <div style="display: flex; justify-content: space-between; gap: 5px;">
                <div>
                    Working: <span class="badge bg-blue-light font-11">${filterAttendance.length}</span>
                </div>
                <div>
                    Absent: <span class="badge bg-red-light font-11">${filterAttendance.filter(x => x.isPresent === '0').length}</span>
                </div>
                <div>
                    Present: <span class="badge bg-green-light font-11">${filterAttendance.filter(x => x.isPresent === '1').length}</span>
                </div>
            </div>`;     
   
}
function displayLeaveRequests() {
    $('#leaveRequests').empty();
    var data = filterleaveRequests;
    var anchors = "";
    for (var i = 0; i < data.length; i++) {
        var statusName = "";
        var statusDot = "";
        if (data[i].statusId == 1) {
            statusName = `<i class="fa fa-clock color-yellow-dark" style="font-size : 15px"></i>`;
           // statusName = `<div class="event-dot"></div>`;
        }
        if (data[i].statusId == 2) {
            statusName = `<i class="fa fa-times-circle color-red-dark" style="font-size : 15px"></i>`;
        }
        if (data[i].statusId == 3) {
            statusName = `<i class="fa fa-check-circle color-green-dark" style="font-size : 15px"></i>`;
        }
        // Determine the leave type (Casual or Sick)
        if (data[i].leaveRequestType === '1') {
            statusDot = `<i class="fa fa-star color-blue-dark"></i>`;
           /* leaveType = `<span class="status-dot bg-blue-dark">Casual</span>`;*/
        } else if (data[i].leaveRequestType === '2') {
           // leaveType = `<span class="status-dot bg-orange-dark">Sick</span>`;
            statusDot = `<i class="fa fa-star color-orange-dark"></i>`;
        }

        // Generate the HTML for the leave request
        anchors += `<a href="#">
                           ${statusDot}
                            <span> ${data[i].reason} (${data[i].dayCount})</span>
                            <strong> ${data[i].strFromDate} - ${data[i].strToDate}</strong>
                            ${statusName}
                        </a>`;
    }  
    $('#leaveRequests').append(anchors);
}



