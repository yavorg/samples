var socket = null;

 
$(function() {

  var name = 'Sleepy User';

  socket = io.connect('http://tr15chat.cloudapp.net');
  
  socket.on('connect', function () {
    socket.emit('setname', name);
    $("#chat").append($("<div class=\"system\">you have joined the party</div>"));
  });


  socket.on('announcement', function(data) {
    $("#chat").append($("<div class=\"system\">" + data.announcement + "</div>"));
  });

  socket.on('message', function (data) {
    $("#chat").append($("<div><span class=\"user\">" + data.message[0] + ":&nbsp;</span>" + data.message[1] + "</div>"));
  });	

  socket.on('messages', function (data) {
  	for (var i=0; i<data.buffer.length; i++) {
    	$("#chat").append($("<div><span class=\"user\">" + data.buffer[i].message[0] + ":&nbsp;</span>" + data.buffer[i].message[1] + "</div>"));
    } 
  });

  $("#send").click(function(e){
    e.preventDefault();
    submitMessage();
  });

  $('#message').keypress(function(e) {
    if(e.which == 13)
      submitMessage();
  });

  $("#message").focus();
  
})

function submitMessage() {
  var msg = $("#message").val();
  $("#message").val('');
  socket.send(msg);
}