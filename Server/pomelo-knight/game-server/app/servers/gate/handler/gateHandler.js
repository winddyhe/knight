var Code = require('../../../common/code.js');
var dispatcher = require('../../../common/dispatcher.js');

/**
 * Gate Handler 用来转化user的connectors.
 */
module.exports = function (app) {
    return new Handler(app);
};

var Handler = function (app) {
    this.app = app;
};

Handler.prototype.queryEntry = function (msg, session, next) {
    var userName = msg.userName;
    if (!userName) {
        next(null, { code: Code.FAIL });
        return;
    }
    var connectors = this.app.getServersByType('connector');
    if (!connectors || connectors.length === 0) {
        next(null, { code: Code.GATE.FA_NO_SERVER_AVAILABLE });
        return;
    }
    dispatcher.dispatch(userName, connectors, function (res) {
        next(null, { code: Code.OK, host: res.host, port: res.clientPort });
    });
};