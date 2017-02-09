var settings = require("./settings.js");
var mongoose = require("mongoose");
var pomelo = require('pomelo');

var dbclient = module.exports;

mongoose.connect("mongodb://" + settings.host + ":" + settings.port + "/" + settings.db);
dbclient.mongoose = dbclient;
var Schema = mongoose.Schema;

///////////////////////////////////////////////////////////////////////////////////////////
/**
 * Actor Schema
 */
var ActorSchema = new Schema({
    actorID: Number,    //角色ID
    actorName: { type: String, unique: true },  //角色名
    level: Number,      //等级
    professionalID: Number, //职业类型
    serverID: Number,   //服务器ID
});

var ActorModel = mongoose.model('Actor', ActorSchema);
dbclient.ActorModel  = ActorModel;
dbclient.ActorSchema = ActorSchema;

/**
 * 自增ID模块
 */
var autoIncrement = require('mongoose-auto-increment');
autoIncrement.initialize(mongoose.connection);
ActorSchema.plugin(autoIncrement.plugin, {
    model: 'Actor',
    field: 'actorID',
    startAt: 20000,
    incrementBy: 1
});


///////////////////////////////////////////////////////////////////////////////////////////////
/**
 * UserSchema
 */
var UserSchema = new Schema({
    id: Number,
    name: { type: String, unique: true },
    password: String,
    actors: [ActorSchema]
});
var UserModel = mongoose.model('User', UserSchema);
dbclient.UserModel  = UserModel;
dbclient.UserSchema = UserSchema;

/**
 * User的自增模块
 */
autoIncrement.initialize(mongoose.connection);
UserSchema.plugin(autoIncrement.plugin, {
    model: 'User',
    field: 'id',
    startAt: 1000,
    incrementBy: 1
});