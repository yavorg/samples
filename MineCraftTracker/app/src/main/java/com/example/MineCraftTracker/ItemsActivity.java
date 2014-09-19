package com.example.MineCraftTracker;

import android.app.Activity;
import android.app.AlertDialog;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ListView;
import android.widget.ProgressBar;

import com.google.common.util.concurrent.FutureCallback;
import com.google.common.util.concurrent.Futures;
import com.google.common.util.concurrent.ListenableFuture;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.microsoft.windowsazure.mobileservices.MobileServiceClient;
import com.microsoft.windowsazure.mobileservices.MobileServiceList;
import com.microsoft.windowsazure.mobileservices.http.NextServiceFilterCallback;
import com.microsoft.windowsazure.mobileservices.http.ServiceFilter;
import com.microsoft.windowsazure.mobileservices.http.ServiceFilterRequest;
import com.microsoft.windowsazure.mobileservices.http.ServiceFilterResponse;
import com.microsoft.windowsazure.mobileservices.table.MobileServiceJsonTable;

import com.microsoft.windowsazure.mobileservices.table.MobileServiceSystemProperty;
import com.microsoft.windowsazure.mobileservices.table.query.ExecutableJsonQuery;
import com.microsoft.windowsazure.mobileservices.table.query.QueryOrder;
import com.microsoft.windowsazure.notifications.NotificationsManager;
import com.microsoft.windowsazure.mobileservices.notifications.Registration;
import com.microsoft.windowsazure.mobileservices.notifications.RegistrationCallback;

import java.net.MalformedURLException;
import java.util.ArrayList;
import java.util.EnumSet;
import java.util.List;
import java.util.Map;

import static com.microsoft.windowsazure.mobileservices.table.query.QueryOperations.val;


public class ItemsActivity extends Activity {

	/**
	 * Mobile Service Client reference
	 */
	private MobileServiceClient mClient;

	/**
	 * Mobile Service Table used to access data
	 */
	private MobileServiceJsonTable mItemTable;

	/**
	 * Adapter to sync the items list with the view
	 */
	private ItemAdapter mAdapter;


	/**
	 * Progress spinner to use for table operations
	 */
	private ProgressBar mProgressBar;

    /**
     * For notifications
     */
    public static final String SENDER_ID = Secrets.GoogleProjectNumber;

	/**
	 * Initializes the activity
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_to_do);

        NotificationsManager.handleNotifications(this, SENDER_ID, PushHandler.class);
		
		mProgressBar = (ProgressBar) findViewById(R.id.loadingProgressBar);

		// Initialize the progress bar
		mProgressBar.setVisibility(ProgressBar.GONE);
		
		try {
			// Create the Mobile Service Client instance, using the provided
			// Mobile Service URL and key
			mClient = new MobileServiceClient(
					Secrets.MobileServiceUri,
				    Secrets.MobileServiceKey,
					this).withFilter(new ProgressFilter());

			// Get the Mobile Service Table instance to use
			mItemTable = mClient.getTable("Inventory");

			// Create an adapter to bind the items with the view
			mAdapter = new ItemAdapter(this, R.layout.row_list_to_do);
			ListView listViewToDo = (ListView) findViewById(R.id.listViewToDo);
			listViewToDo.setAdapter(mAdapter);

			// Load the items from the Mobile Service
			refreshItemsFromTable();

		} catch (MalformedURLException e) {
			createAndShowDialog(
                    new Exception(
                            "There was an error creating the Mobile Service. Verify the URL"),
                    "Error");
		}
	}
	
	/**
	 * Initializes the activity menu
	 */
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}
	
	/**
	 * Select an option from the menu
	 */
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		if (item.getItemId() == R.id.menu_refresh) {
			refreshItemsFromTable();
		}
		
		return true;
	}

	/**
	 * Refresh the list with the items in the Mobile Service Table
	 */
	private void refreshItemsFromTable() {

        // Get the items that weren't marked as completed and
        // add them in the adapter
        new AsyncTask<Void, Void, Void>(){
            @Override
            protected Void doInBackground(Void... params) {
                try {

                    final ArrayList<Item> items = new ArrayList<Item>();

                    // The table contains the different items as COLUMNS and there is one
                    // row in the table for each time the list of items changed. We get
                    // just the last row which has the latest status.
                    final JsonArray entities = mItemTable
                            .orderBy("__createdAt", QueryOrder.Descending)
                            .top(1).execute().get().getAsJsonArray();

                    if(entities.size() > 0){
                        JsonObject lastResult = (JsonObject) entities.get(0);

                        // Create an item for each column (ignoring system properties)
                        for (Map.Entry<String, JsonElement> item : lastResult.entrySet()){
                            String key = item.getKey();
                            // Ignore system properties
                            if(!"id".equalsIgnoreCase(key) &&
                                    !key.startsWith("__"))
                            {
                                JsonElement val = lastResult.get(key);
                                if(!val.isJsonNull()) {
                                    items.add(new Item(key, val.getAsInt()));
                                }
                            }
                        }

                    }

                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            mAdapter.clear();

                            for(Item item : items){
                                mAdapter.add(item);
                            }
                        }
                    });
                } catch (Exception e){
                    createAndShowDialog(e, "Error");

                }

                return null;
            }
        }.execute();
	}

    /**
     * Registers mobile services client to receive GCM push notifications
     * @param gcmRegistrationId The Google Cloud Messaging session Id returned
     * by the call to GoogleCloudMessaging.register in NotificationsManager.handleNotifications
     */
    public void registerForPush(final String gcmRegistrationId)
    {
        try {
            mClient.getPush().register(gcmRegistrationId, null).get();
        } catch (Exception e) {
            createAndShowDialog(e, "Error");
        }
    }

    /**
	 * Creates a dialog and shows it
	 *
	 * @param exception
	 *            The exception to show in the dialog
	 * @param title
	 *            The dialog title
	 */
	private void createAndShowDialog(Exception exception, String title) {
		Throwable ex = exception;
		if(exception.getCause() != null){
			ex = exception.getCause();
		}
		createAndShowDialog(ex.getMessage(), title);
	}

	/**
	 * Creates a dialog and shows it
	 * 
	 * @param message
	 *            The dialog message
	 * @param title
	 *            The dialog title
	 */
	private void createAndShowDialog(final String message, final String title) {
        final AlertDialog.Builder builder = new AlertDialog.Builder(this);

        runOnUiThread(new Runnable() {
            @Override
            public void run() {

                builder.setMessage(message);
                builder.setTitle(title);
                builder.create().show();
            }
        });
	}

    private class ProgressFilter implements ServiceFilter {

        @Override
        public ListenableFuture<ServiceFilterResponse> handleRequest(
                ServiceFilterRequest request, NextServiceFilterCallback next) {

            runOnUiThread(new Runnable() {

                @Override
                public void run() {
                    if (mProgressBar != null){
                        mProgressBar.setVisibility(ProgressBar.VISIBLE);
                    }
                }
            });

            ListenableFuture<ServiceFilterResponse> result = next.onNext(request);

            Futures.addCallback(result, new FutureCallback<ServiceFilterResponse>() {
                @Override
                public void onFailure(Throwable exc) {
                    dismissProgressBar();
                }

                @Override
                public void onSuccess(ServiceFilterResponse resp) {
                    dismissProgressBar();
                }

                private void dismissProgressBar() {
                    runOnUiThread(new Runnable() {

                        @Override
                        public void run() {
                            if (mProgressBar != null) {
                                mProgressBar.setVisibility(ProgressBar.GONE);
                            }
                        }
                    });
                }
            });

            return result;
        }
    }
}
