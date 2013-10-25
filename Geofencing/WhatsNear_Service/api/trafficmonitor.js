
exports.get = function(request, response) {
    if (request.query !== null)
    {
        console.log("trafficMonitor main thing");
        var campaignTraffic = request.service.tables.getTable('campaignTraffic');
        var id = request.query.campaignId;
        var campaign = request.service.tables.getTable('campaign');
        campaign.where({ id: id }).read({
            success: function (results) {
                if (results.length === 0) {
                    request.respond(400, 'No data in table');
                } else {
                    var url = results[0].url;
                    campaignTraffic.insert(
                    {
                        time: new Date(),
                        campaignId: id,
                    }, { success: function() {
                            response.set("Location", url);
                            response.send(302);
                        }, 
                        error: function() {
                            response.send(500, { message : 'problem update' });    
                        }
                    })
                }
            }
        });         
    } else {
        response.send(statusCodes.Error, { message : 'Missing parameter' });    
    } 
};