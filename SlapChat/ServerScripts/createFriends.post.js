exports.post = function (request, response) {
    var userId = request.query.userId,
        emailAddresses = request.query.emailAddresses,
        friendsTable = request.service.tables.getTable('friends');

    request.service.mssql.query(
        'select * from users where userId <> (?) and userId in ' +
        '(select userId from emails where emailAddress in (' +
        serializeEmailAddresses(emailAddresses.split(' ')) +
        '))', [userId], {
            success: function (friends) {
                friends.forEach(function (friend) {
                    friendsTable.insert({
                        userId: userId,
                        friendId: friend.userId
                    });
                });
                response.send(200, friends);
            }
        }
    );
};

function serializeEmailAddresses(emailAddresses) {
    var arg = ''
    emailAddresses.forEach(function (address) {
        arg += '\'' + address + '\', ';
    });
    arg = arg.substring(0, arg.length - 2);
    return arg;
}