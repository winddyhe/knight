var logger = require('pomelo-logger').getLogger(__filename);
var userDao = require('./../../../db/user.js');
var util = require('./../../../common/util.js');
var Code = require('./../../../common/code.js');
var async = require('async');

module.exports = function (app) {
    return new Handler(app);
};

var Handler = function (app) {
    this.app = app;
};

/**
 * 账户登录
 */
Handler.prototype.ClientLoginRequest = function (msg, session, next) {
    logger.info(msg);
    var token = msg.token, self = this;

    if (!token) {
        next(new Error('invalid entry request: empty token.'), { code: Code.FAIL });
        return;
    }

    var uid, players, player;
    async.waterfall([
        // Token认证
        function (cb) {
            self.app.rpc.auth.authRemote.auth(session, token, cb);
        },
        // 通过UserID查找player
        function (code, user, cb) {
            if (code !== Code.OK) {
                next(null, { code: code });
                return;
            }
            if (!user) {
                next(null, { code: Code.ENTRY.FA_USER_NOT_EXIST });
                return;
            }
            uid = user.id;
            players = user.actors;
            cb();
        },
        // // 生成Session并注册会话状态
        // function (cb) {
        //     self.app.get('sessionService').kick(uid, cb);
        // },
        // function (cb) {
        //     session.bind(uid, cb);
        // },
        function (cb) {
            logger.info("---- ClientLoginRequest: " + uid);
            next(null, { code: Code.OK, uid: uid, actors: players });
        }
    ]);
};

/**
 * 创建角色
 */
Handler.prototype.ClientCreatePlayerRequest = function (msg, session, next) {
    logger.info(msg);
    // 创建角色
    userDao.InsertActor(msg.accountID, msg.playerName, 1, msg.professionalID, msg.serverID, function (actorID, code) {
        logger.info("---- ClientCreatePlayerRequest: " + code + ", actorID: " + actorID);
        next(null, { code: code, actorID: actorID });
    });
};

/**
 * Publish route for mqtt connector.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.publish = function (msg, session, next) {
    var result = {
        topic: 'publish',
        payload: JSON.stringify({ code: 200, msg: 'publish message is ok.' })
    };
    next(null, result);
};

/**
 * Subscribe route for mqtt connector.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.subscribe = function (msg, session, next) {
    var result = {
        topic: 'subscribe',
        payload: JSON.stringify({ code: 200, msg: 'subscribe message is ok.' })
    };
    next(null, result);
};
