function update(item, user, request) {

	var pusher = require('../shared/pusher.js');
	pusher.triggerRefresh();
    request.execute();

}