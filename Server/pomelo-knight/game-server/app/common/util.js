
var logger = require('pomelo-logger').getLogger(__filename);
var Util = function () { };
module.exports = new Util();

Util.prototype.RandomNumber = function (min, max) {
    if (min > max){
        var temp = min;
        min = max;
        max = temp;
    }
    
    var range = max - min;
    var rand = Math.random();
    return (min + Math.round(rand * range));
};