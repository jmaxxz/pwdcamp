var Promise = require('promise');
var readFile = Promise.denodeify(require('fs').readFile);
var writeFile = Promise.denodeify(require('fs').readFile);

var Database = function Database (data) {
  this.users = data && data.users || {};
  this.save = function save(path){
    return writeFile(path, JSON.stringify(this));
  }
};

module.exports.Database = Database;

module.exports.load = function load (path) {
  return readFile(path, 'utf8').then(function (res){
    return new Database(JSON.parse(res));
  });
}