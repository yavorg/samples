package com.example.AndroidStarter;

import android.app.IntentService;
import android.content.Intent;

import com.microsoft.windowsazure.mobileservices.MobileServiceClient;
import com.microsoft.windowsazure.mobileservices.table.query.Query;
import com.microsoft.windowsazure.mobileservices.table.sync.MobileServiceSyncContext;
import com.microsoft.windowsazure.mobileservices.table.sync.MobileServiceSyncTable;
import com.microsoft.windowsazure.mobileservices.table.sync.localstore.ColumnDataType;
import com.microsoft.windowsazure.mobileservices.table.sync.localstore.SQLiteLocalStore;

import java.util.HashMap;
import java.util.Map;

public class OfflinePushIntentService extends IntentService {

    public OfflinePushIntentService() {
        super("OfflinePushIntentService");
    }

    @Override
    protected void onHandleIntent(Intent intent) {
        try {
            MobileServiceClient client = new MobileServiceClient(
                    Secrets.MobileServiceUri,
                    Secrets.MobileServiceKey,
                    getApplicationContext());

            MobileServiceSyncTable<ToDoItem> table = client.getSyncTable(ToDoItem.class);
            Query query = OfflineToDoActivity.getQuery(client);
            OfflineToDoActivity.setUpLocalStoreWithHandler(client, null);

            table.pull(query).get();

        } catch (Exception e){
            // Swallow exceptions
        }
    }
}