var Pusher = require('pusher'),
	pusher = new Pusher(
	{
        appId: '{get from portal}',
        key: '{get from portal}',
        secret: '{get from portal}'
	});


function insert(item, user, request) {
    if (item.text.toLowerCase().indexOf('work') > -1) {
        request.respond(statusCodes.BAD_REQUEST, {
            error: "You're not allowed to talk about this in Las Vegas"
        });
    } else {
    	item.userId = user.userId;
    	pusher.trigger('todo', 'refresh', {});
        request.execute();
    }
}