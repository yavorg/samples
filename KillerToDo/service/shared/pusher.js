function generateClient(){
	var Pusher = require('pusher');
	return new Pusher(
	{
		appId: '{get from portal}',
		key: '{get from portal}',
		secret: '{get from portal}'
	});
}

exports.triggerRefresh = function() {
	var pusher = generateClient();
	pusher.trigger('todo', 'refresh', {});
	
};

exports.triggerSendMessage = function(message){
	var pusher = generateClient();
	pusher.trigger('todo', 'sendMessage', message);
}