exports.post = function(request, response) {

    var push = request.service.push;
    var message = request.body.message;

    console.log("Received notification with message " + message);

    var payload = '{"data":{"message" : "' + message + '" }}';
    push.gcm.send(null, payload, {
        success: function(pushResponse) {
            console.log("Sent push:", pushResponse);
            request.respond(200);
            },              
        error: function (pushResponse) {
            console.log("Error sending push:", pushResponse);
            request.respond(500, { error: pushResponse });
            }
        });
};

exports.get = function(request, response) {
    response.send(statusCodes.OK, { message : 'Hello World!' });
};