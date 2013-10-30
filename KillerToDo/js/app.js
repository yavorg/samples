$(function() {
    var client = new WindowsAzure.MobileServiceClient(
        killertodoUrl, 
        killertodoKey),
        todoItemTable = client.getTable('todoitem');

    // Read current data and rebuild UI.
    // If you plan to generate complex UIs like this, consider using a JavaScript templating library.
    function refreshTodoItems() {
        var filter = $('#query-filter')
        var query = todoItemTable.where(function(val){
                return this.complete == false &&
                this.text.indexOf(val) == 0;
            }, filter.val())
            .skip(skip*take).take(take).includeTotalCount();

        query.read().then(function(todoItems) {
            var listItems = $.map(todoItems, function(item) {
                return $('<li>')
                    .attr('data-todoitem-id', item.id)
                    .append($('<button class="item-delete">Delete</button>'))
                    .append($('<input type="checkbox" class="item-complete">').prop('checked', item.complete))
                    .append($('<div>').append($('<input class="item-text">').val(item.text)));
            });

            $('#todo-items').empty().append(listItems).toggle(listItems.length > 0);
            $('#summary').html('Showing page <strong>' + (skip + 1) + '</strong> out of ' +
                '<strong>' + Math.ceil(todoItems.totalCount / take) + '</strong>');
        });
    }

    function getTodoItemId(formElement) {
        return Number($(formElement).closest('li').attr('data-todoitem-id'));
    }

    // Handle insert
    $('#add-item').submit(function(evt) {
        var textbox = $('#new-item-text'),
            itemText = textbox.val();
        if (itemText !== '') {
            todoItemTable.insert({ text: itemText, complete: false }).then(
                refreshTodoItems,
                function(error){
                    alert(JSON.parse(error.request.responseText).error);
                });
        }
        textbox.val('').focus();
        evt.preventDefault();
    });

    // Handle update
    $(document.body).on('change', '.item-text', function() {
        var newText = $(this).val();
        todoItemTable.update({ id: getTodoItemId(this), text: newText });
    });

    $(document.body).on('change', '.item-complete', function() {
        var isComplete = $(this).prop('checked');
        todoItemTable.update({ id: getTodoItemId(this), complete: isComplete }).then(refreshTodoItems);
    });

    // Handle delete
    $(document.body).on('click', '.item-delete', function () {
        todoItemTable.del({ id: getTodoItemId(this) }).then(refreshTodoItems);
    });

    // Handle paging
    var skip = 0,
    take = 3;

    $('#next').click(function(){
        skip++;
        refreshTodoItems();
    });

    $('#prev').click(function(){
        if(skip !== 0){
            skip--;
            refreshTodoItems();
        }
    });

    // Handle filtering
     $('#query').submit(function(evt) {
        var textbox = $('#query-filter'),
            itemText = textbox.val();
        if (itemText !== '') {
            refreshTodoItems();
        }
        textbox.val('').focus();
        evt.preventDefault();
    });

    // Handle auth
    function refreshAuthDisplay() {
        var isLoggedIn = client.currentUser !== null;
        $('#logged-in').toggle(isLoggedIn);
        $('#logged-out').toggle(!isLoggedIn);


        if (isLoggedIn) {
            var facebookId = client.currentUser.userId.split(':')[1],
                facebookGraphUri = 'http://graph.facebook.com/' + facebookId + '/';
            $('#login-picture').attr('src', facebookGraphUri + 'picture?type=square');
            $.get(facebookGraphUri + '?fields=name', function(result){
                $('#login-name').text(result.name);
            })
            refreshTodoItems();
        }
    }


    function logIn() {
        client.login('facebook').then(refreshAuthDisplay, function(error){
            alert('Error during login: ' + error);
        });
    }


    function logOut() {
        client.logout();
        refreshAuthDisplay();
        $('#todo-items').empty();
        $('#summary').html('<strong>You must login to access data.</strong>');
    }
    
    refreshAuthDisplay();
    $('#summary').html('<strong>You must login to access data.</strong>');          
    $("#logged-out button").click(logIn);
    $("#logged-in button").click(logOut);

    // Pusher support
    var pusher = new Pusher(killertodoPusherId),
        channel = pusher.subscribe('todo');

    channel.bind('refresh', refreshTodoItems);
    channel.bind('sendMessage', function(payload){
        alert('Important message! ' + payload);
    })

});