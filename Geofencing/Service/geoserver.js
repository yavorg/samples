var fs = require('fs');
var http = require('http');
var url = require('url');
var argv = process.argv.slice(2);
var help =
  "usage: geoserver path [options] \n\n" +
  "Takes a dataset that has already been partitioned using " +
  "the geopartition tool and hosts it as an API." + 
  "options:\n" +
  "-h, [--help] # Show this help message and quit"
var tasks = {};
tasks.help = function(){
  console.log(help);
};
tasks.startServer = function(f){
	http.createServer(function (req, res) {
		var query = url.parse(req.url, true).query;
		var lat = Number(query.lat);
		var lon = Number(query.lon);
		if(lat && lon && !isNaN(lat) && !isNaN(lon)){
			var filename = hashCoordinates(lat, lon) + '.part';
			fs.readFile(f + '/' + filename, 'utf8', function(error, data){
				if(!error){
					var rows = data.split('\n');
					var response = [];
					console.log(rows);
					rows.forEach(function(row){
						if(row !== ''){
							var cols = row.split('|');
							response.push({
								name: cols[1],
								lat: Number(cols[9]),
								lon: Number(cols[10]) 
							});
						}
					});
					res.writeHead(200, {'Content-Type': 'text/plain'});
					var responseString = JSON.stringify(response) 
					console.log(responseString);
					res.end(responseString);
				} else {
					// No known geofences
					res.writeHead(200, {'Content-Type': 'text/plain'});
					var responseString = JSON.stringify([])
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
};
function hashCoordinates(lat, lon){
	// The hashing strategy is to go within 2 decimal geo-servers
	// of a degree
	return "lat" + Math.floor(lat * 100).toString() + 
	"lon" + Math.floor(lon * 100).toString();
}

if(argv[0] === "--help" || argv[0] === "-h") {
	tasks.help();
} else if (argv[0] === "" || argv[0] === undefined || argv[0] === null) {
	console.log('Path parameter is required.');
} else {
	tasks.startServer(argv[0]);
}

