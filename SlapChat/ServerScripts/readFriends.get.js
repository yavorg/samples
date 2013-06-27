exports.get = function (request, response) {
    request.service.mssql.query(
    	'select * from users where userId in ' +
        '(select friendId from friends where userId in (?))',
        request.query.userId, {
            success: function (results) {
                response.send(200, results)
            }
        }
    );
};