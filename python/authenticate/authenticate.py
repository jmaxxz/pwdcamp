#!/usr/bin/python
# Filename: authenticate.py
import argparse
import json
import sys
from user import create_user
from database import create_database
from database import load_database
from database import save_database

path = 'database.json'
parser = argparse.ArgumentParser(description='Creates and manages a simple database of users.')
parser.add_argument('username', metavar='username', help='The username to be used')
parser.add_argument('password', metavar='password', help='The password of the user')
parser.add_argument('-i','--init', help='Does not attempt to load existing database', action='store_true')
parser.add_argument('-a','--add', help='Adds a new user', action='store_true')

args = parser.parse_args()

database = not args.init and load_database(path) or create_database()

if args.add:
	if args.username in database['users']:
		sys.stderr.write('User Exists\n')
		sys.exit(1)

	# TODO: the following line adds a new user to the database
	#      you may with to change how this works for your program.
	database['users'][args.username] = create_user(args.username)
	if not save_database(database, path):
		sys.stderr.write('Could not save database.\n')
		sys.exit(1)
else:
	# TODO: Add some form of authentication to determine if correct
	#      credentials were presented.
	if args.username not in database['users']:
		sys.stderr.write('Invalid user\n')
		sys.exit(1)	


print 'Success'
sys.exit()
