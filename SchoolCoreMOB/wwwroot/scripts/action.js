//var basepath = window.location.origin + window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/'));
var rootpath = window.location.pathname.split('/')[1];  // Get the first part of the path
//var tmprootpath = window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/'));
// Check if the basepath exists (i.e., if the application is deployed under a subdirectory)
if (!rootpath) {
    rootpath = '';  // Default to an empty string if no subdirectory is present (localhost)
} else {
    rootpath = '/' + rootpath; // Make sure it's prefixed with a single slash
}

//var basepath = window.location.origin + rootpath; //if under schoolcoremob
var basepath = window.location.origin;



var uploadType = { student: 1, teacher: 2, questionPaper: 3, organization: 4, attachment: 5, announcement: 6, assignment : 7 }
var uploadFileType = { img: 1, pdfDoc: 2 }

function validateuploadFile(type, filetype, file) {
    var ext;
    var size;
    var errMsg = "";
    if (file.files.length == 0) {
        errMsg = "No file selected";
    }
    let typexists = Object.values(uploadType).includes(type);
    if (typexists) {
       // ext = file.value.split('.').pop();
        ext = file.files[0].name.split('.').pop().toLowerCase();

       // console.log(ext, filetype);
        size = (file.files[0].size / 1024 / 1024).toFixed(2);
        if (size > 4) {
            errMsg = "File size must be less than 4 MB";
        }
        else {            
            switch (filetype) {
                case '1':
                    if (ext !== "jpg" & ext !== "jpeg" & ext !== "png") {
                        errMsg = "only jpg,jpeg,png are allowed";
                    }                   
                    break;
                case '2':
                    if (ext !== "pdf") {
                        errMsg = "only pdf is allowed";
                    }
                    break;
            }
        }
    }
    //console.log(typexists);
    //if (errMsg !== "") {
    //    toastr.error(errMsg);
    //}
    return errMsg;  

}
function setfooterActiveAfterNavigation() {
    const navLinks = document.querySelectorAll('#footer-bar .nav-link');
    navLinks.forEach(nav => nav.classList.remove('active-nav'));
    var urlpath = window.location.pathname;
    var urlpathlast = window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/'));
    navLinks.forEach(link => {
        const linkurl = link.getAttribute('data-url')
        if (linkurl === urlpath) {
            link.classList.add('active-nav'); // Add active class                
        }
    });
    var footerBar1 = document.querySelectorAll('.footer-bar-5')[0];
    if (footerBar1) {
        var footerBarselect = document.querySelectorAll('#footer-bar .active-nav')[0];
        footerBarselect.insertAdjacentHTML('beforeend', '<strong></strong>');
    }
}
function setActiveNav(dataUrl) {  
    const navLinks = document.querySelectorAll('#footer-bar .nav-link');
    navLinks.forEach(nav => nav.classList.remove('active-nav'));
    navLinks.forEach(link => {
        const linkurl = link.getAttribute('data-url')
        if (linkurl === dataUrl) {
            link.classList.add('active-nav');
        }
    });
    var footerBar1 = document.querySelectorAll('.footer-bar-5')[0];
    if (footerBar1) {
        var footerBarselect = document.querySelectorAll('#footer-bar .active-nav')[0];
        footerBarselect.insertAdjacentHTML('beforeend', '<strong></strong>');
    }
}
function setChildNavigation(navUrl) {    
    window.location.href = navUrl;
}

function setHomeChildNavigation(navUrl) {
   // console.log(basepath + navUrl)
    window.location.href = basepath + navUrl;
}
function setNewChildNavigation(navUrl) {    
    //  console.log(basepath + navUrl)
    
    window.location.href = basepath + navUrl;
    console.log('navurl:' + navUrl);
    console.log('rootpath:' + rootpath);
    console.log('basepath:' + basepath);

}

function showloginLoading() {
    document.getElementById('divloginloader').style.display = 'flex';
}
//function onMenuLoad(page) {
//    switch (page) {
//        case '1':
//            window.location.href = basepath + '/MenuMain'; // `@Url.Action("Index", "Profile")`;
//            break;       
//    }
//}
function onMenuClick(page) {
    switch (page) {
        case '1':
            window.location.href = basepath + '/Profile'; // `@Url.Action("Index", "Profile")`;
            break;
        case '2':
            window.location.href = basepath + '/Teacher'; //`@Url.Action("Index", "Teacher")`;
            break;
        case '3':
            window.location.href = basepath + '/Attendence'; //`@Url.Action("Index", "Attendence")`;
            break;
        case '4':
            window.location.href = basepath + '/Calender'; //`@Url.Action("Index", "Calender")`;
            break;
        //case '5':
        //    window.location.href = basepath + '/Schedule'; //`@Url.Action("Index", "Schedule")`;
        //    break;
        case '6':
            window.location.href = basepath + '/Notification'; //`@Url.Action("Index", "Notification")`;
            break;
        case '7':
            window.location.href = basepath + '/Login'; //`@Url.Action("Index", "Login")`;
            break;
    }
    
}

function hideloginLoading() {
    document.getElementById('divloginloader').style.display = 'none';
}
function showLoading() {
    document.getElementById('divloader').style.display = 'flex';
}
function hideLoading() {
    document.getElementById('divloader').style.display = 'none';
}

function showToast(statusId, newText) {
   
    const snackbar = document.getElementById('divtoast');
    var icon = $(snackbar).find('i');
    
    if (statusId == '1') {
        snackbar.className = 'snackbar-toast bg-green-dark';
        icon.removeClass('fa fa-times me-3').addClass('fa fa-check me-3'); 
    }
    else {
        snackbar.className = 'snackbar-toast bg-red-dark';
        icon.removeClass('fa fa-check me-3').addClass('fa fa-times me-3');
    }
    snackbar.innerHTML = newText; 
    var notificationToast = new bootstrap.Toast(snackbar);  
    notificationToast.show();
}
function showLoginToast(statusId, newText) {

    const snackbar = document.getElementById('divlogintoast');
    var icon = $(snackbar).find('i');
    if (statusId == '1') {
        snackbar.className = 'snackbar-toast bg-green-dark';
        icon.removeClass('fa fa-times me-3').addClass('fa fa-check me-3');
    }
    else {
        snackbar.className = 'snackbar-toast bg-red-dark';
        icon.removeClass('fa fa-check me-3').addClass('fa fa-times me-3');
    }
    snackbar.innerHTML = newText;
    var notificationToast = new bootstrap.Toast(snackbar);
    notificationToast.show();
}
function getShortDayOfWeekString(dayInt) {
    const daysOfWeek = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    // Ensure that the passed integer is within the valid range (0-6)
    if (dayInt >= 0 && dayInt <= 6) {
        return daysOfWeek[dayInt];
    } else {
        return "Invalid day";  // Handle invalid inputs
    }
}
function getDayOfWeekInt(dayName) {
    const daysOfWeek = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    // Convert the passed dayName to a capitalized form (in case of case mismatch)
    const index = daysOfWeek.indexOf(dayName.charAt(0).toUpperCase() + dayName.slice(1).toLowerCase());

    // Ensure that the dayName is valid
    if (index !== -1) {
        return index;
    } else {
        return "Invalid day";  // Handle invalid inputs
    }
}
function getTodayDayInt() {
    const today = new Date();
    let dayOfWeek = today.getDay();  // Get the day of the week (0 = Sunday, 1 = Monday, ..., 6 = Saturday)

    // If it's Sunday (day 0), return 7 (or you can return 0 depending on your requirements)
    if (dayOfWeek === 0) {
        return 7; // Exclude Sunday by treating it as 7, or you can return 0 if Sunday is considered a special case.
    }
    return dayOfWeek;  // Return the day number (1 for Monday, 2 for Tuesday, ..., 6 for Saturday)
}
function getTodayWeekdayName() {
    const days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    const today = new Date();
    return days[today.getDay()];
}
function setMinDate(inputId) {
    // Get today's date
    const today = new Date();

    // Format the date to yyyy-mm-dd (required format for the 'min' attribute)
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0'); // months are zero-indexed
    const day = String(today.getDate()).padStart(2, '0');

    // Set the 'min' attribute to today's date
    const formattedDate = `${year}-${month}-${day}`;

    // Get the input element by ID and set its 'min' attribute
    const inputElement = document.getElementById(inputId);
    if (inputElement) {
        inputElement.setAttribute('min', formattedDate);
    } else {
        console.error('Element not found with id:', inputId);
    }
}
function performSearch(search, tablename) {
    const searchBox = document.getElementById(search);
    const table = document.getElementById(tablename);
    const trs = table.tBodies[0].getElementsByTagName("tr");
    // Declare search string 
    var filter = searchBox.value.toUpperCase();

    // Loop through first tbody's rows
    for (var rowI = 0; rowI < trs.length; rowI++) {

        // define the row's cells
        var tds = trs[rowI].getElementsByTagName("td");

        // hide the row
        trs[rowI].style.display = "none";

        // loop through row cells
        for (var cellI = 0; cellI < tds.length; cellI++) {

            // if there's a match
            if (tds[cellI].innerHTML.toUpperCase().indexOf(filter) > -1) {

                // show the row
                trs[rowI].style.display = "";

                // skip to the next row
                continue;

            }
        }
    }

}




