var Promise = require('promise');
var readFile = Promise.denodeify(require('fs').readFile);
var writeFile = Promise.denodeify(require('fs').writeFile);

var fs = require('fs');

var Database = function Database (data) {
  this.users = data && data.users || {};
  this.save = function save(path){
  	// this doesn't write out the file
  	//return writeFile(path, JSON.stringify(this));

    // this works
    fs.writeFileSync(path, JSON.stringify(this));
  }
};

module.exports.Database = Database;

module.exports.load = function load (path) {
  return readFile(path, 'utf8').then(function (res){
    return new Database(JSON.parse(res));
  });
}