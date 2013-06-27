function insert(item, user, request) {
    var usersTable = tables.getTable('users');

    // Add this line when auth is on
    //item.userId = user.userId

    usersTable.where({
        userId: item.userId
    }).read({
        success: function (results) {
            var me = results[0];
            if (!me) {
                extractEmailAddresses(item);
                request.execute();
            } else {
                item.id = me.id;
                me.name = item.name;
                me.mpnsChannel = item.mpnsChannel;
                me.emailAddresses = item.emailAddresses;
                extractEmailAddresses(me);
                usersTable.update(me, {
                    success: function () {
                        request.respond(200, me);
                    }
                });


            }
        }
    });
}

function extractEmailAddresses(item) {
    var emailsTable = tables.getTable('emails');
    var addresses = item.emailAddresses;
    delete item.emailAddresses;

    addresses.split(' ').forEach(function (address) {
        emailsTable.where({
            emailAddress: address
        }).read({
            success: function (results) {
                var result = results[0];
                if (result) {
                    result.userId = item.userId;
                    emailsTable.update(result);
                } else {
                    emailsTable.insert({
                        emailAddress: address,
                        userId: item.userId
                    });
                }
            }
        })
    });
}