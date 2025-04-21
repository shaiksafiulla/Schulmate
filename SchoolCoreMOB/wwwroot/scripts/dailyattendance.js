// Initialize the current date and other variables
let currentMonth = new Date().getMonth();
let currentYear = new Date().getFullYear();

// Global variables to hold attendance and leave data
let totalAttendance = [];
let filterAttendance = [];


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
    if (filterAttendance) {
        // Add the days of the month
        for (let j = 1; j <= numDays; j++) {
            const dateCell = document.createElement("div");
            dateCell.classList.add("date");
            dateCell.textContent = j;

            if (filterAttendance.length > 0) {
                let objDay = filterAttendance.find(filter => filter.day === j);
                if (objDay !== undefined && objDay !== null) {                    
                    const dot = document.createElement("div");                    
                    dot.classList.add("event-dot");
                    dot.style.backgroundColor = 'blue'; // Set the dot's color to the event's background color

                    dateCell.addEventListener("click", function () {
                        setNewChildNavigation(`/StudentAttendence/AddStudentAttendence?Id=${objDay.id}`);
                    });
                    dateCell.appendChild(dot); // Append the dot to the date cell                   
                }
            }
            datesContainer.appendChild(dateCell);
        }
        displayAttendanceSummary();
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
function fetchAttendanceData(month, year, attendUrl, token, userparam) {
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
            const el = document.getElementById('divaddAttendence');
            if (!response.isAttendenceExist) {
                el.style.display = "block";
            }
            else {
                el.style.display = "none";
            }
           
            totalAttendance = response.lstDailyAttendence;           
            const grandsummaryContainer = document.getElementById("grandAttendanceSummary");
            grandsummaryContainer.innerHTML = `                      

            <div style="display: flex; justify-content: space-between; gap: 5px;">
                <div>
                    Total Working Days: <span class="badge bg-blue-light font-11">${totalAttendance.length}</span>
                </div>               
            </div>`;          
            renderCalendar(month, year); // Re-render calendar with data
        },
        error: function () {
            console.error("Error fetching attendance data");
        }
    });
}



// Function to display the attendance summary
function displayAttendanceSummary() {
    const summaryContainer = document.getElementById("attendanceSummary");
    summaryContainer.innerHTML = `

 <div style="display: flex; justify-content: space-between; gap: 5px;">
                <div>
                    Working Days: <span class="badge bg-blue-light font-11">${filterAttendance.length}</span>
                </div>               
            </div>`;     
   
}




