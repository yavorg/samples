function read(query, user, request) {
    var userId = request.parameters.userId;

    // Turn on when we have auth
    // userId = user.userId;

    query.where(function (userId) {
        return this.senderUserId === userId ||
        this.recepientUserId === userId;
    }, userId);
    request.execute();

}