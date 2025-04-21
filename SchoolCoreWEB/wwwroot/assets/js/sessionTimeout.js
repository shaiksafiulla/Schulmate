var timeout;
var timeoutpath = window.location.origin + window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/'));
function resetSessionTimeout() {
    clearTimeout(timeout);
    timeout = setTimeout(logout, 20 * 60 * 1000); // 20 minutes in milliseconds
}

function logout() {
    // Redirect to the logout or login page
    window.location.href = timeoutpath + '/Login/LogOut'; // Adjust the URL as needed
}

document.addEventListener('mousemove', resetSessionTimeout);
document.addEventListener('keypress', resetSessionTimeout);

// Initialize the session timeout on page load
resetSessionTimeout();
