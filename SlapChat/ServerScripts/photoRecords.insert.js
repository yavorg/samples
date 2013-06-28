function insert(item, user, request) {
    var contentsTable = tables.getTable('photoContents');
    prepareUpload(function (result) {
        var content = {
            secretId: fakeGuid(),
            uri: result.uri,
        };

        item.sent = new Date();
        item.photoContentSecretId = content.secretId;
        item.expired = false;

        // We don't want to store these
        delete item.uri;
        delete item.uploadKey;

        // Turn on when we have real auth
        // item.senderUserId = user.Id;

        request.execute({
            success: function () {

                content.photoRecordId = item.id;
                contentsTable.insert(content, {
                    success: function () {

                        // These are transient properties so the client
                        // can do the upload, they are not stored for 
                        // security reasons
                        item.uploadKey = result.sasQueryString;
                        item.uri = content.uri;

                        request.respond();

                        sendPushNotification(item.recepientUserId);
                    }
                });
            }
        });
    });
}

function sendPushNotification(id) {
    var usersTable = tables.getTable('users');
    usersTable.where({
        userId: id
    }).read({
        success: function (results) {
            results.forEach(function (result) {
                push.mpns.sendToast(result.mpnsChannel, {
                    text1: 'SlapChat',
                    text2: 'You have a new photo!'
                });
            });
        }
    });
}


function fakeGuid() {
    return new Date().getTime() + '' + Math.random();
}

function prepareUpload(callback) {
    var azure = require('azure');
    var qs = require('querystring');

    var accountName = '';
    var containerName = 'photos';
    var itemName = fakeGuid();
    var accountKey = '';
    var host = accountName + '.blob.core.windows.net';

    var blobService = azure.createBlobService(accountName, accountKey, host);
    blobService.createContainerIfNotExists(containerName, {
        publicAccessLevel: 'blob'
    }, function (error) {
        if (!error) {
            // Provide write access to the container for the next 5 mins.        
            var sharedAccessPolicy = {
                AccessPolicy: {
                    Permissions: azure.Constants.BlobConstants.SharedAccessPermissions.WRITE,
                    Expiry: new Date(new Date().getTime() + 5 * 60 * 1000)
                }
            };

            // Generate the upload URL with SAS for the new image.
            var sasQueryUrl =
                blobService.generateSharedAccessSignature(containerName,
                itemName, sharedAccessPolicy);

            callback({
                sasQueryString: qs.stringify(sasQueryUrl.queryString),
                uri: sasQueryUrl.baseUrl + sasQueryUrl.path
            });

        } else {
            console.error(error);
        }
    });

}