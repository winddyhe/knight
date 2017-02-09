var logger = require('pomelo-logger').getLogger(__filename);
var pomelo = require('pomelo');
var encrypt = require('../common/encrypt.js');
var actorDao = require('./actor.js');
var Code = require('../common/code.js');

var UserDAO = function () { };
module.exports = new UserDAO();

/**
 * 插入一个用户数据
 */
UserDAO.prototype.Insert = function (userName, password) {
    var encryptPassword = encrypt.MD5(password);

    var UserModel = pomelo.app.get('dbclient').UserModel;
    var userEntity = new UserModel({ name: userName, password: encryptPassword });
    userEntity.save(function (error, doc) {
        if (error)
            logger.error('----- error:  ' + error);
        else
            logger.info(doc);
    });
};

/**
 * 查找账户下的所有角色，一个服务器对应多个角色
 */
UserDAO.prototype.FindActorInServer = function (id, serverID, cb) {
    FindUser(id, function (err, user) {
        if (user === null) return;

        // 找该用户下的ServerID下的所有角色
        user.actors.find({ 'serverID': serverID }, function (err, actors) {
            if (cb) cb(err, actors);
        });
    });
};

/**
 * 查找一个User根据AccountName
 */
UserDAO.prototype.FindUser = function (userName, cb) {
    var UserModel = pomelo.app.get('dbclient').UserModel;
    UserModel.findOne({ 'name': userName }, function (err, user) {
        if (user === null) {
            logger.error('Account ' + userName + ' not found!');
        }
        cb(err, user);
    });
};


/**
 * 查找一个user根据ID
 */
UserDAO.prototype.FindUserByID = function (uid, cb) {
    var UserMode = pomelo.app.get('dbclient').UserModel;
    UserMode.findOne({ 'id': uid }, function (err, user) {
        if (user === null) {
            logger.error('Account id = ' + uid + ' not found!');
        }
        cb(err, user);
    });
};


/**
 * 根据UserName找UserID
 */
UserDAO.prototype.FindUserIDByName = function (userName, cb) {
    this.FindUser(userName, function (err, user) {
        if (err) {
            cb(err, null);
            return;
        }
        cb(null, user.id);
    });
};

/******************************************************************************************************/
/******************************************************************************************************/
/******************************************************************************************************/

/**
 * 插入角色的信息
 */
UserDAO.prototype.InsertActor = function (id, actorName, level, professionalID, serverID, cb) {
    var UserModel = pomelo.app.get('dbclient').UserModel;
    this.FindUserByID(id, function (err, user) {
        if (user === null) {
            logger.error('Account not found!');
            cb(0, Code.ENTRY.FA_USER_NOT_EXIST);
            return;
        }
        UserModel.findOne({ 'actors.actorName': actorName }, function (err, actors) {
            if (actors !== null) {
                logger.error('Actor name is exsist!');
                cb(0, Code.ENTRY.FA_ACTOR_IS_EXSIST);
                return;
            } else {
                actorDao.Insert(user, actorName, level, professionalID, serverID, function (actor, resultMsg) {
                    if (actor !== null) {
                        cb(actor.actorID, Code.OK);
                    } else {
                        cb(0, resultMsg);
                    }
                });
            }
        });
    });
};
