package com.example.MineCraftTracker;

import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.NotificationCompat;
import com.microsoft.windowsazure.notifications.NotificationsHandler;

public class PushHandler extends NotificationsHandler {
    public static final int NOTIFICATION_ID = 1;

    @Override
    public void onRegistered(Context context, String gcmRegistrationId) {
        super.onRegistered(context, gcmRegistrationId);
        ItemsActivity itemsActivity = (ItemsActivity) context;
        itemsActivity.registerForPush(gcmRegistrationId);
    }

    @Override
    public void onReceive(Context context, Bundle bundle) {
        String nhMessage = bundle.getString("message");
        sendNotification(nhMessage, context);
    }

    private void sendNotification(String msg, Context ctx) {
        NotificationManager mNotificationManager = (NotificationManager)
                ctx.getSystemService(Context.NOTIFICATION_SERVICE);

        PendingIntent contentIntent = PendingIntent.getActivity(ctx, 0,
                new Intent(ctx, ItemsActivity.class), 0);

        NotificationCompat.Builder mBuilder =
                new NotificationCompat.Builder(ctx)
                        .setSmallIcon(R.drawable.ic_launcher)
                        .setContentTitle("New Notification")
                        .setStyle(new NotificationCompat.BigTextStyle()
                                .bigText(msg))
                        .setContentText(msg);

        mBuilder.setContentIntent(contentIntent);
        mNotificationManager.notify(NOTIFICATION_ID, mBuilder.build());
    }


}
