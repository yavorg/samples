function sendNews() {
    var pusher = require('../shared/pusher.js');
	pusher.triggerSendMessage('Head to the slots');
}