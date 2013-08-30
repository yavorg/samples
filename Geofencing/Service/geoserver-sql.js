var sql = require('sqlserver');
var conn_str = "Driver={SQL Server Native Client 11.0};Server={tcp:dd5nq7bq2w.database.windows.net,1433};UID={int7bn10015@dd5nq7bq2w};PWD={JAngtJvMCawSy343};Encrypt={Yes};Database={GeoPrototype}";
var http = require('http');
var url = require('url');

http.createServer(function (req, res) {
    var query = url.parse(req.url, true).query;         
    var lat = Number(query.lat);                
    var lon = Number(query.lon);
 
    if(lat && lon && !isNaN(lat) && !isNaN(lon)){
        var sqlQuery = "DECLARE @deviceLocation geography = geography::Point(?, ?, 4326);" +
        "SELECT top (10) [fenceId], [lat], [long] from GeoFences" +
        "WHERE @deviceLocation.STDistance(fence) < 5000" +
        "ORDER BY @deviceLocation.STDistance(fence)";

        sql.query(conn_str, sqlQuery, [lat, lon], function (err, results) {
            if (err || !result || !result.length) {
                if(err){
                    console.log(err);
                }
                res.writeHead(200, {'Content-Type': 'text/plain'});                                 
                var responseString = JSON.stringify([])                                      
                console.log(responseString);                                 
                res.end(responseString);
            } else {
                res.writeHead(200, { 'Content-Type': 'text/plain' });
                var response = [];
                for (var i = 0; i < results.length; i++) {
                    response.push({
                        name: results[i].fenceId,               
                        lat: results[i].lat,
                        lon: results[i].long});
                }
                var responseString = JSON.stringify(response); 
                console.log(responseString);                                    
                res.end(responseString);
            }
        });
    } else {
        res.writeHead(404, {'Content-Type': 'text/plain'});
        res.end('Please specify valid lat and lon paramegers as part of the query string.');
    }
}).listen(1337, '127.0.0.1');
console.log('Server running at http://127.0.0.1:1337/');    