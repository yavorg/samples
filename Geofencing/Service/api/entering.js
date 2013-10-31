var http = require('http');
var url = require('url');

// Called when entering a geo-fence
// Registers the client's channel for a push based on geo-fence tag
// Returns campaigns relevant to the geo-fence
exports.post = function(request, response) {
    var parameters = request.query;
    var body = request.body;
    
    var inputFence = body.fenceName;    
    var inputToken = body.token;
    
    console.log("fenceName: " , inputFence);
    
    if (inputFence) {
        var campaignTable = request.service.tables.getTable('campaign');
        campaignTable.where({fenceName: inputFence}).read({ 
            success: function (results) 
            {
              if(results.length > 0)
              {
                console.log("retrieved: " + JSON.stringify(results));
                if (inputToken) 
                {
                    // register this in NH for additional capability
                }
              }
              else
              {
                console.log("No campaigns for fenceName "+ inputFence);
                results = "[]";
              }
              request.respond(200, results);
            }
         },
         {
            error: function() {request.respond(500, {result: 'Internal error, query failed'});}
         }
        );
    }
    else
    {
        request.respond(400, {result: 'Expected fenceName'});
    }
};
