var crc = require('crc');
var userDao = require('../db/user.js');
var util = require('./util.js');

module.exports.dispatch = function (userName, connectors, cb) {
	var index = util.RandomNumber(0, connectors.length);

	userDao.FindUserIDByName(userName, function (err, uid) {
		if (uid) {
			index = Number(uid) % connectors.length;
		}
		cb(connectors[index]);
	});
};