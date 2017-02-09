var tokenService = require('../../../common/token.js');
var userDao = require('../../../db/user.js');
var Code = require('../../../common/code.js');
var Encrypt = require('../../../common/encrypt.js');

var DEFAULT_SECRET = 'pomelo_session_secret';
var DEFAULT_EXPIRE = 6 * 60 * 60 * 1000;	// 默认的session的过期时间: 6 hours

module.exports = function (app) {
    return new Remote(app);
};

var Remote = function (app) {
    this.app = app;
    var session = app.get('session') || {};
    this.secret = session.secret || DEFAULT_SECRET;
    this.expire = session.expire || DEFAULT_EXPIRE;
};

var pro = Remote.prototype;

/**
 * 用户认证
 */
pro.auth = function (token, cb) {
    var res = tokenService.parse(token, this.secret);
    if (!res) {
        cb(null, Code.ENTRY.FA_TOKEN_EXPIRE);   // TOKEN不合法
        return;
    }

    if (!checkExpire(res, this.expire)) {
        cb(null, Code.ENTRY.FA_TOKEN_EXPIRE);   // TOKEN已经过期
        return;
    }
    
    // 查询User是否存在
    userDao.FindUser(res.userName, function (err, user) {
        if (err) {
            cb(null, Code.ENTRY.FA_USER_NOT_EXIST);
            return;
        }
        
        // 验证密码是否正确
        var password = user.password;
        var clientPassword = Encrypt.MD5(res.password);
        if (password !== clientPassword){
            cb(null, Code.ENTRY.FA_USER_PASS_ERROR);
            return;
        }
        cb(null, Code.OK, user);
    });
};


/**
 * 检查Token是否到期
 * true - 到期, false - 未到期
 */
var checkExpire = function (token, expire) {
    if (expire < 0) return true;

    return (Date.now() - token.timestamp) < expire;
};