exports.post = function(request, response) {
    var tables = request.service.tables;
    tables.getTable("Inventory").insert(request.body, {
        success: function() {
            request.respond(statusCodes.OK);
            }
    });
};

exports.get = function(request, response) {
    response.send(statusCodes.OK, { message : 'Hello World!' });
};