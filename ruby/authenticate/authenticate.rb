#!/usr/bin/ruby
require 'optparse'

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
options.username = ARGV.pop()
options.password = ARGV.pop()

puts !options.initMode ? 'read from disk' : 'make it up'

if options.addMode
	puts 'I see you want to add a user'
else
	puts 'Authentication time'
end

