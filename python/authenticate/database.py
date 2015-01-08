import json
def create_database():
	return {'users':{}}

def load_database(path):
	try:
		with open(path, 'r') as json_data:
			data = json.load(json_data)
			return data;
	except:
		return None;

def save_database(database, path):
	try:
		with open(path, 'wb') as outfile:
			json.dump(database, outfile, indent=2)
		return True;
	except:
		return False;
