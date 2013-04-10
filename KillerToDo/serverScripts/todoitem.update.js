var Pusher = require('pusher'),
	pusher = new Pusher(
	{
		appId: '41446',
		key: '20b0fbcf18822724d672',
		secret: '24a044b957aa9aefcb13'
	});

function update(item, user, request) {

	pusher.trigger('todo', 'refresh', {});
    request.execute();

}