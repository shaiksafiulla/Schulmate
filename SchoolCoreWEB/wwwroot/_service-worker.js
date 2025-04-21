
self.addEventListener('push', function (event) {
    let notificationData = event.data ? event.data.json() : {};    
    const options = {
        body: notificationData.Message,
        icon: '/assets/app/icons/icon-72x72.png',
        badge: '/assets/app/icons/icon-72x72.png',
    };

    event.waitUntil(
        self.registration.showNotification(notificationData.Title, options)
    );
    // Update notification count in memory (using localStorage or sessionStorage)
    event.waitUntil(
        self.clients.matchAll().then(clients => {
            // If we have open tabs, send a message to update the notification count
            clients.forEach(client => {
                client.postMessage({
                    type: 'UPDATE_NOTIFICATION_COUNT'
                });
            });
        })
    );   
});

 //Optionally handle the notification click event
self.addEventListener('notificationclick', function (event) {
    event.notification.close(); 
    event.waitUntil(
        clients.openWindow('/')  // Open your app or a specific URL
    );
});
