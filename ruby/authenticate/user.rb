require 'json'

class User
	def initialize username
		@username = username
	end

	def username
		@username
	end

	def to_json options
		{'username' => @username}.to_json options
	end

	def self.from_json string
		data = JSON.load string
		self.new data['username']
	end
end