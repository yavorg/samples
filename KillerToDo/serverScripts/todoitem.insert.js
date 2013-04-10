var Pusher = require('pusher'),
	pusher = new Pusher(
	{
		appId: '41446',
		key: '20b0fbcf18822724d672',
		secret: '24a044b957aa9aefcb13'
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