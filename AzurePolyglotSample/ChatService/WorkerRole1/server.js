var app = require('http').createServer(handler)
  , io = require('socket.io').listen(app)
  , fs = require('fs')
  , uuid = require('node-uuid')
  , nconf = require('nconf')
  , azure = require('azure');

app.listen(process.env.port);

nconf.env().file({file: 'settings.json'});

function handler (req, res) {
  fs.readFile(__dirname + '/index.html',
  function (err, data) {
    if (err) {
      res.writeHead(500);
      return res.end('Error loading index.html');
    }

    res.writeHead(200);
    res.end(data);
  });
}

var serviceBusSubscription = uuid.v4();
var serviceBusClient = azure.createServiceBusService(
  nconf.get('azure:serviceBusNamespace'), 
  nconf.get('azure:serviceBusAccessKey'));


io.configure(function () {
  io.set("transports", ["websocket"]); 
  serviceBusCreateSubscriptions();
});

function setUpSocketIo(){

  serviceBusReceive('message');
  serviceBusReceive('announcement');

  io.sockets.on('connection', function (socket) {
    socket.on('setname', function(name) {
      socket.set('name', name, function() {
        serviceBusSend({announcement: name + ' connected'}, 'announcement');
      });
    });
    socket.on('message', function (message) {
      socket.get('name', function(err, name){
        serviceBusSend({ message: [name, message] }, 'message');        
      });
    });
    socket.on('disconnect', function() {
      socket.get('name', function(err, name) {
        serviceBusSend({announcement: name + ' disconnected' }, 'announcement');
      })
    })
  });
}

function serviceBusCreateSubscriptions()
{
  console.log('About to create subscriptions');
  serviceBusClient.createSubscription('message', 
    serviceBusSubscription, function messageSubscriptionCreated(error) {
      if (error) {
        throw error;
      } else {
        console.log('Message subscription exists');
        serviceBusClient.createSubscription('announcement', serviceBusSubscription,
          function announcementSubscriptionCreated(error){
            if(error){
              throw error;
            } else {
              console.log('Announcement subscription exists');
              setUpSocketIo();
            }
          });
      }
  });
}

function serviceBusSend(message, topic){
  var msg = JSON.stringify(message);
  console.log('About to queue message to ServiceBus: ' + msg);
  serviceBusClient.sendTopicMessage(topic, 
    msg, 
    function messageSent(error) {
      if (error) {
        console.log(JSON.stringify(error));
        throw error;
      } else {
        console.log('Message queued up to Service Bus: ' + msg);
      }
    });
}

function serviceBusReceive(topic){
  console.log('About to receive message');
  serviceBusClient.receiveSubscriptionMessage(topic,
    serviceBusSubscription, {timeoutIntervalInS: 5}, 
    function messageReceived(error, message) {
      if (error) {
        if(error === 'No messages to receive'){
          console.log('Resetting Service Bus receive');
          serviceBusReceive(topic);
        } else {
          console.log(JSON,stringify(error));
          throw error;
        }
      } else {
        console.log('Received Service Bus message ' + 
          JSON.stringify(message));
        io.sockets.emit(topic, JSON.parse(message.body));
        serviceBusReceive(topic);
      }
    });
}
