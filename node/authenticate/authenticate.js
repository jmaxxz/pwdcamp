#!/usr/bin/env node
var Promise = require('promise');
var cli = require('cli');
var db = require('./database');
var User = require('./user');
var path = 'database.json';

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
        //TODO: the following line adds a new user to the database
        //      you may with to change how this works for your program.
        database.users[username] = new User({ username: username });

        database.save(path).then(fulfill, function () { reject("Could not save database."); });
      } else {//Authenticate mode
        //TODO: Add some form of authentication to determine if correct
        //      credentials were presented.
        if (!database.users[username]) reject("Invalid user");
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