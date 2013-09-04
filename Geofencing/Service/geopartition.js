var fs = require('fs');
var argv = process.argv.slice(2);
var help =
  "usage: geopartition input [options] \n\n" +
  "Takes a pipe-delimited geo data file as documented here " +
  "http://geonames.usgs.gov/domestic/download_data.htm and partitions" + 
  "it for easier geofence lookups\n"+
  "options:\n" +
  "-h, [--help] # Show this help message and quit"
var tasks = {};
tasks.help = function(){
  console.log(help);
};
tasks.partition = function(f){
	var found = 0;
	var processed = 0;
 	var stream = fs.createReadStream(f, {
		encoding: 'utf8'
	});
	stream.on('data', function(chunk){
		var rows = chunk.split('\n');
		rows.forEach(function(row){
			found++;
			var cols = row.split('|');
			var lat = Number(cols[9]);
			var lon = Number(cols[10]);
			if(lat && lon && !isNaN(lat) && !isNaN(lon)){
				processed++;
				var filename = hashCoordinates(lat, lon) + '.part';
				var f = fs.appendFile(filename, row + '\n', 
					function(error){
						if(error){
							console.log(error);
						} else {
						}
				});
			}
		});
			console.log('Found ' + found + ' points and processed ' + processed + '.');
	});

};

function hashCoordinates(lat, lon){
	// The hashing strategy is to go within 2 decimal places
	// of a degree
	return "lat" + Math.floor(lat * 100).toString() + 
	"lon" + Math.floor(lon * 100).toString();
}

if(argv[0] === "--help" || argv[0] === "-h") {
	tasks.help();
} else if (argv[0] === "" || argv[0] === undefined || argv[0] === null) {
	console.log('Path parameter is required.');
} else {
	tasks.partition(argv[0]);
}

