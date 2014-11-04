package com.example.AndroidStarter;

import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.TextView;

/**
 * Adapter to bind a ToDoItem List to a view
 */
public class OfflineToDoItemAdapter extends ArrayAdapter<ToDoItem> {

	/**
	 * Adapter context
	 */
	Context mContext;

	/**
	 * Adapter View layout
	 */
	int mLayoutResourceId;

	public OfflineToDoItemAdapter(Context context, int layoutResourceId) {
		super(context, layoutResourceId);

		mContext = context;
		mLayoutResourceId = layoutResourceId;
	}

	/**
	 * Returns the view for a specific item on the list
	 */
	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		View row = convertView;

		final ToDoItem currentItem = getItem(position);

		if (row == null) {
			LayoutInflater inflater = ((Activity) mContext).getLayoutInflater();
			row = inflater.inflate(mLayoutResourceId, parent, false);
		}

		row.setTag(currentItem);

        final CheckBox checkBox = (CheckBox) row.findViewById(R.id.checkToDoItem);
        checkBox.setChecked(false);
        checkBox.setEnabled(true);

        checkBox.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View arg0) {
                if (checkBox.isChecked()) {
                    checkBox.setEnabled(false);
                    if (mContext instanceof OfflineToDoActivity) {
                        OfflineToDoActivity activity = (OfflineToDoActivity) mContext;
                        currentItem.setComplete(true);
                        activity.updateItem(currentItem);
                    }
                }
            }
        });

        final EditText itemTextBox = (EditText)row.findViewById(R.id.textBoxEditItem);
        itemTextBox.setText(currentItem.getText());
        itemTextBox.setOnFocusChangeListener(new View.OnFocusChangeListener() {
            @Override
            public void onFocusChange(View arg0, boolean arg1){

                // Lost focus
                if (arg1 == false){
                    if (mContext instanceof OfflineToDoActivity) {
                        OfflineToDoActivity activity = (OfflineToDoActivity) mContext;
                        currentItem.setText(itemTextBox.getText().toString());
                        activity.updateItem(currentItem);
                    }
                }

            }
        });

        return row;
	}

}
