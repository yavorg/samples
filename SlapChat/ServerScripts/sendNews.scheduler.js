function sendNews() {
    var azure = require('azure');

    var notificationHubService = azure.createNotificationHubService('slapchat',
    'Endpoint=sb://slapchat-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=zFIVKN/ZbmmcpW1fnp6PF3E224rIHqxMawJsMUx31p4=');

    notificationHubService.mpns.sendToast(null, {
        text1: 'SlapChat',
        text2: 'Our outage is over!'
    });
}