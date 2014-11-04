package com.example.AndroidStarter;

import junit.runner.Version;

/**
 * Represents an item in a ToDo list
 */
class ToDoItem {
    public final static String Name = "ToDoItem";
    public final static String TextPropertySerializedName = "text";
    public final static String IdPropertySerializedName = "id";
    public final static String CompletePropertySerializedName = "complete";
    public final static String VersionPropertySerializedName = "version";

	/**
	 * Item text
	 */
	@com.google.gson.annotations.SerializedName(TextPropertySerializedName)
	private String mText;

	/**
	 * Item Id
	 */
	@com.google.gson.annotations.SerializedName(IdPropertySerializedName)
	private String mId;

	/**
	 * Indicates if the item is completed
	 */
	@com.google.gson.annotations.SerializedName(CompletePropertySerializedName)
	private boolean mComplete;

    /**
     * The version of the item in the database
     */
    @com.google.gson.annotations.SerializedName(VersionPropertySerializedName)
    private String mVersion;

	/**
	 * ToDoItem constructor
	 */
	public ToDoItem() {

	}

	@Override
	public String toString() {
		return getText();
	}

	/**
	 * Initializes a new ToDoItem
	 * 
	 * @param text
	 *            The item text
	 * @param id
	 *            The item id
	 */
	public ToDoItem(String text, String id) {
		this.setText(text);
		this.setId(id);
	}

	/**
	 * Returns the item text
	 */
	public String getText() {
		return mText;
	}

	/**
	 * Sets the item text
	 * 
	 * @param text
	 *            text to set
	 */
	public final void setText(String text) {
		mText = text;
	}

	/**
	 * Returns the item id
	 */
	public String getId() {
		return mId;
	}

	/**
	 * Sets the item id
	 * 
	 * @param id
	 *            id to set
	 */
	public final void setId(String id) {
		mId = id;
	}

	/**
	 * Indicates if the item is marked as completed
	 */
	public boolean isComplete() {
		return mComplete;
	}

	/**
	 * Marks the item as completed or incomplete
	 */
	public void setComplete(boolean complete) {
		mComplete = complete;
	}

    /**
     * Gets the version of the item in the database
     *
     * @return the version of the item in the database
     */
    public String getVersion() {
        return mVersion;
    }

    /**
     * Sets the version of the item in the database
     *
     * @param mVersion the version of the item in the database
     */
    public void setVersion(String mVersion) {
        this.mVersion = mVersion;
    }


	@Override
	public boolean equals(Object o) {
		return o instanceof ToDoItem && ((ToDoItem) o).mId.equals(mId);
	}
}
