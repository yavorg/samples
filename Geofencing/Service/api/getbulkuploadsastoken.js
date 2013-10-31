var uuid = require('uuid');
var azureBootstrap = require('../shared/azureBootstrap.js');

exports.get = function(request, response) {
    var containerName = 'uploads';
    var name = uuid.v4();
    var pictureRelativePath = '/' + containerName + '/' + name;

    // Create the container if it does not exist
    // Use public read access for the blobs, and the SAS to upload        
    var blobService = azureBootstrap.createBlobService();
    blobService.createContainerIfNotExists(
        containerName, 
        { publicAccessLevel: 'blob' }, 
        function (error) {
            if (!error) {
                var url = azureBootstrap.createSasUrl(pictureRelativePath);
                response.send(statusCodes.OK, url);
            }
            else {
                response.send(500, error);
            }
        }
    );
}


