#!/usr/bin/ruby
require 'optparse'
require 'json'
load 'database.rb'
load 'user.rb'

Options = Struct.new(:addMode, :initMode, :username, :password)

class AuthenticateOpts
	def self.parse(options)
		args = Options.new(false, false, '', '')

		opt_parser = OptionParser.new do |opts|
			opts.banner = 'Usage: authenticate.rb [options] username password'

			opts.on('-i', '--init', 'Does not attempt to load existing database') do
				args.initMode = true
			end

			opts.on('-a', '--add', 'Adds a new user') do
				args.addMode = true
			end

			opts.on('-h', '-?', '--help', 'Prints this help') do
				puts opts
				exit
			end
		end

		opt_parser.parse!(options)
		return args
	end
end

options = AuthenticateOpts.parse(ARGV)
options.password = ARGV.pop()
options.username = ARGV.pop()
path = 'database.json'

database = !options.initMode && Database.load(path) || Database.new()

if options.addMode
	if database.users.has_key?(options.username)
		$stderr.puts 'User Exists'
		exit 1
	end
	database.users[options.username] = User.new options.username
	if !database.save(path)
		$stderr.puts 'Could not save database.'
		exit 1
	end
else
	if !database.users.has_key?(options.username)
		$stderr.puts 'Invalid user'
		exit 1
	end

	puts 'Success'
end

