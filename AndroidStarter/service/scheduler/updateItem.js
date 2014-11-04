function updateItem() {
    var todoItemsTable = tables.getTable('TodoItem');
    todoItemsTable
        .where(function (name) { return this.text.indexOf(name) > -1; }, "Malmo")
        .read({ success: editItem });
        
    function editItem(results){
        // Take just the first item for this example
        var item = results[0];
        item.text += " BORK";
        todoItemsTable.update(item);
    }
}

