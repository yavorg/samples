function insert(item, user, request) {
    if (item.text.toLowerCase().indexOf('work') > -1) {
        request.respond(statusCodes.BAD_REQUEST, {
            error: "You're not allowed to talk about this in Las Vegas"
        });
    } else {
    	item.userId = user.userId;
        request.execute();
    }
}