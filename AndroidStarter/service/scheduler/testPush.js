function testPush() {
    var payload = '{"data":{"message" : "' + "Hello world" + '" }}';
    push.gcm.send(null, payload, {
        success: function(pushResponse) {
            console.log("Sent push:", pushResponse);
            },              
        error: function (pushResponse) {
            console.log("Error sending push:", pushResponse);
            }
        });
   
}