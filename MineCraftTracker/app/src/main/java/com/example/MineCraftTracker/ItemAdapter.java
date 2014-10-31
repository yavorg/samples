package com.example.MineCraftTracker;

import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.ImageView;
import android.widget.TextView;

/**
 * Adapter to bind a Item List to a view
 */
class ItemAdapter extends ArrayAdapter<Item> {

	/**
	 * Adapter context
	 */
    private final Context mContext;

	/**
	 * Adapter View layout
	 */
    final int mLayoutResourceId;

	public ItemAdapter(Context context, int layoutResourceId) {
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

		final Item currentItem = getItem(position);

		if (row == null) {
			LayoutInflater inflater = ((Activity) mContext).getLayoutInflater();
			row = inflater.inflate(mLayoutResourceId, parent, false);
		}

		row.setTag(currentItem);

        final TextView description = (TextView) row.findViewById(R.id.itemDescription);
        String currentDescription = currentItem.getDescription();
        description.setText(currentDescription);

        final ImageView image = (ImageView) row.findViewById(R.id.itemImage);

        if("Cobblestone".equalsIgnoreCase(currentDescription)){
            image.setImageResource(R.drawable.cobblestone);
        } else if ("Diamond".equalsIgnoreCase(currentDescription)){
            image.setImageResource(R.drawable.diamond);
        } else if ("Dirt".equalsIgnoreCase(currentDescription)){
            image.setImageResource(R.drawable.dirt);
        } else if ("Redstone".equalsIgnoreCase(currentDescription)){
            image.setImageResource(R.drawable.redstone);
        }

        final TextView quantity = (TextView) row.findViewById(R.id.itemQuantity);
        quantity.setText(String.format("%d", currentItem.getQuantity()));

		return row;
	}

}
