var fs = require('fs')

var Database = function Database (data) {
  this.users = data && data.users || {};
  this.save = function save(path){
    try {
      fs.writeFileSync(path, JSON.stringify(this));
      return true;
    } catch (err) {
      return false;
    }
  }
};

module.exports.Database = Database;

module.exports.load = function load (path) {
  try {
    return new Database(JSON.parse(fs.readFileSync(path, 'utf8')));
  } catch (err) {
    return;
  }
}