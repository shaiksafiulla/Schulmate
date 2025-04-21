
document.addEventListener("DOMContentLoaded", function () {
   
   
    let allEvents = []; // Global variable to hold all events
    // Handle navigation between months
    let currentMonth = new Date().getMonth();
    let currentYear = new Date().getFullYear();

    function fetchEventData() {
        var calpath = window.location.origin + window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/'));
        // Perform the AJAX call to get the event data
        return new Promise((resolve, reject) => {
            $.ajax({
                type: 'POST',
                async: false, // Make it asynchronous
                url: calpath + '/Calender/GetAll',
                Accept: 'application/json, text/javascript',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (response) {
                    if (response != null) {
                        allEvents = response; // Store the event data in the global variable
                        resolve(response); // Resolve the promise with the event data
                    } else {
                        showToast('2', `@_localization.Getkey("Error")` + response);
                        reject('No events found'); // Reject the promise if no events are found
                    }
                },
                error: function (response) {
                    showToast('2', `@_localization.Getkey("Error")` + response);
                    reject('Error fetching events'); // Reject the promise in case of an error
                }
            });
        });
    }

// Call the fetchEventData function once and store the events
fetchEventData().then(events => {
    allEvents = events; // Store events in global variable
    renderCalendar(currentMonth, currentYear); // Initially render calendar for current month
    bindEventsToCalendar(allEvents, currentMonth, currentYear); // Bind events to the calendar
});

    // Function to safely parse date string (in YYYY-MM-DD format) into a Date object
    function parseDate(dateStr) {
        return new Date(dateStr + "T00:00:00Z"); // Ensuring it's UTC
    }

    // Function to bind events to the calendar and render event cards below for a specific month and year
    function bindEventsToCalendar(events, month, year) {
        const datesContainer = document.getElementById("dates");
        const dateCells = datesContainer.getElementsByClassName("date"); // Get all date cells

        // Empty the event cards container before rendering new ones
        const eventCardsContainer = document.getElementById("events");
        eventCardsContainer.innerHTML = "";

        // Filter events for the selected month and year
        const filteredEvents = events.filter(event => {
            const eventStartDate = parseDate(event.start);
            return eventStartDate.getFullYear() === year && eventStartDate.getMonth() === month;
        });

        // Loop over each filtered event to bind it to the calendar
        filteredEvents.forEach(event => {
            const eventStartDate = parseDate(event.start); // Parsing startDate
            const eventEndDate = parseDate(event.end); // Parsing endDate

            // Loop over the event's days and add color to the corresponding dates on the calendar
            let currentDate = new Date(eventStartDate); // Start with the event's start date
            while (currentDate <= eventEndDate) {
                const day = currentDate.getDate(); // The actual day number (1, 2, 3, ...)

                // Calculate the correct index of the date cell for this day
                const firstDayOfMonth = new Date(year, month, 1).getDay(); // Get the first day of the month
                const dateCellIndex = firstDayOfMonth + (day - 1); // Adjusting for the first day offset

                const dateCell = dateCells[dateCellIndex]; // Get the corresponding calendar cell for the day

                // Check if the dateCell exists (it may not for empty cells)
                if (dateCell) {
                    // Create a small dot and append it to the top-right corner of the date cell
                    const dot = document.createElement("div");
                    dot.classList.add("event-dot");
                    dot.style.backgroundColor = event.backgroundColor; // Set the dot's color to the event's background color

                    dateCell.appendChild(dot); // Append the dot to the date cell
                    dateCell.classList.add("event-day"); // Optional: add a class to highlight the day
                }
                currentDate.setDate(currentDate.getDate() + 1); // Move to the next day
            }

            // Now render the event card for the event
            renderEventCard(event);
        });
    }

    // Function to render the event card below the calendar
    function renderEventCard(event) {
        console.log(event);
        const eventCardsContainer = document.getElementById("events");

        // Create a new card element for the event
        const card = document.createElement("div");
        card.classList.add("event-card");
        card.style.borderLeft = `5px solid ${event.backgroundColor}`; // Add color to the left border

        // Create the title element
        const title = document.createElement("h3");
        title.textContent = event.title;

        // Create the description element
        const description = document.createElement("p");
        description.textContent = event.description;

        

        // Append title and description to the card
        card.appendChild(title);
        card.appendChild(description);
       

        // Append the card to the event cards container
        eventCardsContainer.appendChild(card);
    }

    // Function to render the calendar (simplified)
    function renderCalendar(month, year) {
        const monthYear = document.getElementById("monthYear");
        const datesContainer = document.getElementById("dates");

        monthYear.textContent = `${getMonthName(month)} ${year}`;
        datesContainer.innerHTML = ""; // Clear the previous month's dates

        const firstDay = new Date(year, month, 1).getDay(); // Get the first day of the month
        const numDays = new Date(year, month + 1, 0).getDate(); // Get the number of days in the month

        // Add empty cells before the first day of the month
        for (let i = 0; i < firstDay; i++) {
            const emptyCell = document.createElement("div");
            emptyCell.classList.add("date");
            datesContainer.appendChild(emptyCell);
        }

        // Add the days of the month
        for (let day = 1; day <= numDays; day++) {
            const dateCell = document.createElement("div");
            dateCell.classList.add("date");
            dateCell.textContent = day;

            datesContainer.appendChild(dateCell);
        }
    }

    // Function to get the month name
    function getMonthName(month) {
        const monthNames = [
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        ];
        return monthNames[month];
    }  

    // Handle the next and previous month navigation
    document.querySelector(".prev-month").addEventListener("click", function () {
        currentMonth--;
        if (currentMonth < 0) {
            currentMonth = 11;
            currentYear--;
        }
        renderCalendar(currentMonth, currentYear);
        bindEventsToCalendar(allEvents, currentMonth, currentYear);        
    });

    document.querySelector(".next-month").addEventListener("click", function () {
        currentMonth++;
        if (currentMonth > 11) {
            currentMonth = 0;
            currentYear++;
        }
        renderCalendar(currentMonth, currentYear);
        bindEventsToCalendar(allEvents, currentMonth, currentYear);       
    });

    
});
