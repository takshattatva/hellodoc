// service-worker.js

self.addEventListener('push', event => {
    const data = event.data.json();
    self.registration.showNotification(data.title, {
        body: data.body,
        // Additional options can be added here
    });
});

self.addEventListener('notificationclick', event => {
    event.notification.close();
    // Add logic to handle notification clicks, such as opening a chat window
    clients.openWindow('/chatHub'); // Example: Open chat window
});
