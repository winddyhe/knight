var logger = require('pomelo-logger').getLogger(__filename);
var pomelo = require('pomelo');

var ActorDAO = function() { };
module.exports = new ActorDAO();

/**
 * 插入角色的信息
 */
ActorDAO.prototype.Insert = function(user, actorName, level, professionalID, serverID, cb) {
    if (user === null) return;
    
    var ActorModel = pomelo.app.get('dbclient').ActorModel;
    var actorEntity = new ActorModel({
        actorName: actorName,
        level: level,
        professionalID: professionalID,
        serverID: serverID
    });
    user.actors.push(actorEntity);
    user.markModified('actors');
    user.save(function(err, data) {
        if (data === null){
            logger.error('Mongodb inster error!');
            cb(null, 'MongodbError');
        }
        else
            cb(data.actors[0], 'Success');
    });
};