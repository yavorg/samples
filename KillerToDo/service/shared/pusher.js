exports.triggerRefresh = function() {
	var Pusher = require('pusher'),
	pusher = new Pusher(
	{
		appId: '{get from portal}',
		key: '{get from portal}',
		secret: '{get from portal}'
	});
	pusher.trigger('todo', 'refresh', {});
	
};
