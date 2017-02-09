var crypto = require('crypto');

/**
 * Create token by uid. Encrypt uid and timestamp to get a token.
 * 
 * @param  {String} uid user id
 * @param  {String|Number} timestamp
 * @param  {String} pwd encrypt password
 * @return {String}     token string
 */
module.exports.create = function (uid, timestamp, pwd) {
	var msg = uid + '|' + timestamp;
	var cipher = crypto.createCipher('aes-256-cbc', pwd);
	var enc = cipher.update(msg, 'utf8', 'base64');
	enc += cipher.final('base64');
	return enc;
};

/**
 * Parse token to validate it and get the uid and timestamp.
 * 
 * @param  {String} token token string
 * @param  {String} pwd   decrypt password
 * @return {Object}  uid and timestamp that exported from token. null for illegal token.     
 */
module.exports.parse = function (token, pwd) {
	var decipher = crypto.createDecipher('aes-256-cbc', pwd);
	var dec;
	try {
		dec = decipher.update(token, 'base64', 'utf8');
		dec += decipher.final('utf8');
	} catch (err) {
		console.error('[token] fail to decrypt token. %j', token);
		return null;
	}
	var ts = dec.split('|');
	if (ts.length !== 3) {
		// illegal token
		return null;
	}
	return { userName: ts[0], password: ts[1], timestamp: Number(ts[2]) };
};
