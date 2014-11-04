package com.example.AndroidStarter;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import com.microsoft.windowsazure.notifications.NotificationsHandler;

public class OfflinePushHandler extends NotificationsHandler {

    @Override
    public void onRegistered(Context context, String gcmRegistrationId) {
        super.onRegistered(context, gcmRegistrationId);
        OfflineToDoActivity toDoActivity = (OfflineToDoActivity) context;
        toDoActivity.registerForPush(gcmRegistrationId);
    }

    @Override
    public void onReceive(Context context, Bundle bundle) {
        String nhMessage = bundle.getString("message");
        if(nhMessage.equals("sync")) {
            Intent sync = new Intent(context, OfflinePushIntentService.class);
            context.startService(sync);
        }
    }
}
