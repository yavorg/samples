function read(query, user, request) {
    // No need for authorizaton here because
    // photoContents link is secret

    request.execute({
        success: function (results) {
            results.forEach(function (content) {
                var recordsTable = tables.getTable('photoRecords');
                recordsTable.where({
                    id: content.photoRecordId
                }).read({
                    success: function (records) {
                        records.forEach(function (record) {
                            record.received = new Date();
                            recordsTable.update(record, {
                                success: function () {
                                    // Schedule deletion in 5 seconds
                                    setTimeout(deleteContent, 5000);
                                    request.respond();
                                }
                            });
                            function deleteContent() {
                                record.expired = true;
                                recordsTable.update(record, {
                                    success: function () {
                                        var contentsTable = tables.getTable('photoContents');
                                        contentsTable.del(content);
                                    }
                                });
                            }
                        });
                    }
                });
            });
        }
    });
}
