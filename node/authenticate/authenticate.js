#!/usr/bin/env node
var Promise = require('promise');
var cli = require('cli');
var db = require('./database');
var User = require('./user');
var path = 'database.json';
var bcrypt = require('bcrypt');

cli.parse({
  add : ['a', 'Adds a new user'],
  init : ['i', 'Does not attempt to load existing database']
});

cli.main(function(args, options) {
  var username = args[0] || '';
  var password = args[1] || '';

  var afterDbLoad = function afterDbLoad (database) {
    return new Promise(function (fulfill, reject) {
      if (options.add) {
        if(database.users[username]) reject('User Exists');

        var salt = bcrypt.genSaltSync(10);
        var hash = bcrypt.hashSync(password, salt);
        
        database.users[username] = new User({ username: username, password: hash });

        database.save(path);//.then(fulfill, function () { console.dir('fails'); reject("Could not save database."); });
      } else {//Authenticate mode
        var user = database.users[username];

        if (!user) 
          reject("Invalid user");

        if (!bcrypt.compareSync(password, user.password))
          reject("Login failed");
      }

      fulfill();
    });
  };

  //Execution starts here
  var run;
  if(!options.init)
    run = db.load(path).then(afterDbLoad, function () {
      afterDbLoad(new db.Database());
    });
  else
    run = afterDbLoad(new db.Database());

  run.then(function(){ 
    cli.output('Success');
    cli.exit();
  }, function(msg){ 
    cli.fatal(msg);
  });

});