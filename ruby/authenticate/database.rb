require 'json'

class Database
	def initialize users={}
		@users = users
	end

	def users
		@users
	end

	def to_json options
		return {'users' => @users}.to_json options
	end

	def save path
		begin
			File.open(path, 'w+') { |f| f.write(JSON.dump(self)) }
			return true
		rescue
			return false
		end
	end

	def self.load(path)
		begin
			data = File.open(path, 'r') { |f| JSON.load(f) }
			return Database.new data['users']
		rescue
			return nil
		end
	end
end