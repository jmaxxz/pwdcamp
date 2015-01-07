#!/usr/bin/env node
var cli = require('cli');
var db = require('./database');
var User = require('./user');
var path = 'database.json';

cli.parse({
  add : ['a', 'Adds a new user'],
  init : ['i', 'Does not attempt to load existing database']
});

cli.main(function(args, options) {
  var database = !options.init && db.load(path) || new db.Database();
  var username = args[0] || '';
  var password = args[1] || '';

  if (options.add) {
    if(database.users[username]) return this.fatal('User Exists');
    //TODO: the following line adds a new user to the database
    //      you may with to change how this works for your program.
    database.users[username] = new User({ username: username });

    if (!database.save(path)) this.fatal("Could not save database.");
  } else {//Authenticate mode
    //TODO: Add some form of authentication to determine if correct
    //      credentials were presented.
    if (!database.users[username]) this.fatal("Invalid user");
  }

  this.output('Success');
  this.exit();
});