function sendNews() {
    var azure = require('azure');

    var notificationHubService = azure.createNotificationHubService('',
    '');

    notificationHubService.send(
        null,
        '{msg: "Our outage is resolved!"}',
        function (error) {

        });
}