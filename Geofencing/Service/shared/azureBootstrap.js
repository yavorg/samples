var azure = require('azure');
var qs = require('querystring');
var accountName = 'whatsnear';
var accountKey = '<TODO: REPLACE WITH REAL KEY>';
var host = accountName + '.blob.core.windows.net';

exports.createBlobService = function () {
    return azure.createBlobService(accountName, accountKey, host);
}

exports.createSasUrl = function (path) {
    // Container exists now define a policy for write access
    // that starts immediately and expires in 5 mins
    var sharedAccessPolicy = createAccessPolicy();

    // Create the blobs urls with the SAS
    var url = createResourceURLWithSAS(accountName, accountKey, path, sharedAccessPolicy, host);

}

function createResourceURLWithSAS(accountName, accountKey, blobRelativePath, sharedAccessPolicy, host) {
    // Generate the SAS for your BLOB
    var sasQueryString = getSAS(accountName,
                        accountKey,
                        blobRelativePath,
                        azure.Constants.BlobConstants.ResourceTypes.BLOB,
                        sharedAccessPolicy);

    // Full path for resource with SAS
    return 'https://' + host + blobRelativePath + '?' + sasQueryString;
}

function createAccessPolicy() {
    return {
        AccessPolicy: {
            Permissions: azure.Constants.BlobConstants.SharedAccessPermissions.WRITE,
            // Start: use for start time in future, beware of server time skew 
            Expiry: formatDate(new Date(new Date().getTime() + 5 * 60 * 1000)) // 5 minutes from now
        }
    };
}

function getSAS(accountName, accountKey, path, resourceType, sharedAccessPolicy) {
    return qs.encode(new azure.SharedAccessSignature(accountName, accountKey)
        .generateSignedQueryString(path, {}, resourceType, sharedAccessPolicy));
}

function formatDate(date) {
    var raw = date.toJSON();
    // Blob service does not like milliseconds on the end of the time so strip
    return raw.substr(0, raw.lastIndexOf('.')) + 'Z';
}

