package com.example.MineCraftTracker;

/**
 * Represents a MineCraft item
 */
class Item {

	/**
	 * Item description
	 */
	private String mDescription;

	/**
	 * The quantity available
	 */
	private int mQuantity;

	/**
	 * Item constructor
	 */
	public Item() {

	}

	@Override
	public String toString() {
		return String.format("%d %s", this.getQuantity(), this.getDescription());
	}

	/**
	 * Initializes a new Item
     */
	public Item(String description, int quantity) {
		this.setDescription(description);
        this.setQuantity(quantity);
	}

	/**
	 * Returns the item description
	 */
	public String getDescription() {
		return mDescription;
	}

	/**
	 * Sets the item description
	 */
	public final void setDescription(String description) {
		mDescription = description;
	}

	/**
	 * Returns the available quantity
	 */
	public int getQuantity() {
		return mQuantity;
	}

	/**
	 * Sets the available quantity
	 */
	public final void setQuantity(int quantity) {
		mQuantity = quantity;
	}

}
