var crypto = require('crypto');

var Encrypt = function() {};
module.exports = new Encrypt();

Encrypt.prototype.MD5 = function(text) {
    var hasher = crypto.createHash('md5');
    hasher.update(text);
    var hashmsg = hasher.digest('hex'); //加密之后的数据
    return hashmsg;
};
