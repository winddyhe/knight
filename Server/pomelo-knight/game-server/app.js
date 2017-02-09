var pomelo = require('pomelo');
var logger = require('pomelo-logger').getLogger(__filename);
var routeUtil = require('./app/common/routeUtil.js');

/**
 * Init app for client.
 */
var app = pomelo.createApp();
app.set('name', 'pomelo-knight');

app.configure('production|development', function () {
  app.route('connector', routeUtil.connector);
});

// Session的配置
app.configure('production|development', 'auth', function () {
  app.set('session', require('./config/session.json'));
});

// gate服务器的初始化
app.configure('production|development', 'gate', function () {
  app.set('connectorConfig', {
    connector: pomelo.connectors.hybridconnector,
    useProtobuf: true
		});
});

// connector服务器的配置
app.configure('production|development', 'connector', function () {
  app.set('connectorConfig', {
    connector: pomelo.connectors.hybridconnector,
    heartbeat: 3,
    useDict: true,
    useProtobuf: true
  });
});

// 数据库的初始化和配置
app.configure('production|development', 'connector|auth|gate', function () {
  var dbclient = require('./app/db/mongoose/mongoose.js');
  app.set('dbclient', dbclient);

  var user = require('./app/db/user.js');
  user.Insert('Test00218', '123');
});

// app启动
app.start();

process.on('uncaughtException', function (err) {
  console.error(' Caught exception: ' + err.stack);
});
