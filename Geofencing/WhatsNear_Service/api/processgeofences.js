var azure = require('azure');
var batchSize = 10;
    
exports.post = function (request, response) {
    var accountName = 'whatsnear';
    var accountKey = '<TODO: REPLACE WITH REAL KEY>';
    var host = accountName + '.blob.core.windows.net';
    var blobService = azure.createBlobService(accountName, accountKey, host);

    var url = request.body.blobUrl;
    var indexOfQueryStart = url.indexOf("?");
    url = url.substring(0, indexOfQueryStart);
    var urlParts = url.split("/", 5);

    console.log("parts", urlParts);

    var containerName = urlParts[3];
    var blobName = urlParts[4];

    blobService.getBlobToText(containerName, blobName,
        function (error, text, blockBlob, response2) {
            if (error) {
                response.send(500, error);
            } else {
                var items = parseInput(text);
                var geofence = request.service.tables.getTable('geofence');
                var context = {
                    items: items,
                    geofence: geofence,
                    totalCount: 0,
                    errorCount: 0,                    
                };
                console.log("Parsed file and got table. Ready to start inserting");
                insertItems(context,
                    function(context) {
                        response.send(statusCodes.OK, 
                        { message: 'Got ' + items.length + ' items, inserted ' + context.totalCount });
                    });
            }
        }
    );
};

function parseInput(inputBody) {
    var list = [];
    var found = 0;
    var processed = 0;
    var rows = inputBody.split('\n');
    console.log("rows", rows.length);
    rows.forEach(function (row) {
        found++;
        var cols = row.split('|');
        var name = cols[1];
        var lat = Number(cols[9]);
        var lon = Number(cols[10]);
        if (lat && lon && !isNaN(lat) && !isNaN(lon)) {
            processed++;
            var partition = hashCoordinates(lat, lon);
            list[processed - 1] = {
                name: name,
                lat: lat,
                lon: lon,
                partition: partition,
                test: true,
            };
        }
    });
    console.log('Found ' + found + ' points and processed ' + processed + '.');
    return list;
}

function insertItems(context, callback) {        
    var insertComplete = function () {
        context.totalCount++;
        if (context.totalCount === context.items.length) {
            // or we are done, report the status of the job 
            // to the log and don't do any more processing 
            console.log("Insert complete. %d Records processed. There were %d errors.", 
                context.totalCount, context.errorCount);
            callback(context);
        } else if (context.totalCount % batchSize === 0 && 
                   context.totalCount < context.items.length) {
            // Completed one batch, but not done inserting
            // kick off the next batch 
            insertItems(context, callback);
        }
        // else not finished with all, and not finished with batch.
        // So just chill. Other callbacks will do somethng.
    };

    var errorHandler = function (err) {
        context.errorCount++;
        console.warn("Ignoring insert failure as part of batch.", err);
        insertComplete();
    };

    var curTotalCount = context.totalCount;
    for (var i = 0; i < batchSize; i++) {
        var item = context.items[curTotalCount + i];
        if (item) {
            context.geofence.insert(item, {
                success: insertComplete,
                error: errorHandler
            });
        } else {
            insertComplete();            
        }
    }
}

function hashCoordinates(lat, lon) {
    // The hashing strategy is to go within 2 decimal places of a degree
    return "lat" + Math.floor(lat * 100).toString() +
           "lon" + Math.floor(lon * 100).toString();
}

