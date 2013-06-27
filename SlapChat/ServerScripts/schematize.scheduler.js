function schematize() {
    var usersTable = tables.getTable('users'),
        friendsTable = tables.getTable('friends'),
        emailsTable = tables.getTable('emails'),
        photoContentsTable = tables.getTable('photoContents'),
        photoRecordsTable = tables.getTable('photoRecords');

    usersTable.insert(
    	{
    	    name: 'Delete me',
    	    userId: 'Delete me',
    	    mpnsChannel: 'Delete me'
    	});

    friendsTable.insert({
        userId: 'Delete me',
        friendId: 'Delete me'
    });

    emailsTable.insert({
        emailAddress: 'Delete me',
        userId: 'Delete me'
    });

    /*
    photoContentsTable.insert({
    	uri: 'Delete me',
        uploaded: false,
        
    });
    
    photoRecordsTable.inert({});
        */



}