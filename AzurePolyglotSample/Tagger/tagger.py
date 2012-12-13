from azure.storage import TableService
from azure.storage import QueueService

import time
import string

account_name = "tr15sharemyphoto"
storage_key = "4yhu8YT3Y6A3do0s+anHFAX6ZUA11V2NJJNhjmJc0iAgSLW8Xwk3QvVQn2Um+hgMmO+vGf0UFd2zOo8K63PD4w=="

queue_service = QueueService(account_name, storage_key)
table_service = TableService(account_name, storage_key)

tags = ('pillow', 'soft', 'white')

def tag_text(text):
	t = []
	words = string.split(text, " ")
	for w in words:
		if w in tags:
			t.append(w)

	ret = set(t)
	print "keywords: "
	print ret
	return ret



def tag_table_entry(rowKey, entry):
	
	tags = tag_text(entry.title + " " + entry.message)

	update = {'title': entry.title, 'message': entry.message}

	for i in ['keyword1', 'keyword2', 'keyword3']:
		if (not hasattr(entry, i)) and len(tags) > 0:
			update[i] = tags.pop()
		elif (not hasattr(entry, i)) and len(tags) == 0 :
			pass
		else:	
			update[i] = getattr(entry, i)
	
	table_service.update_entity('photos', 'pk', rowKey, update)
	print "keywords updated"


def tag_table_entries(rowKey):
	filter = "RowKey eq '" + rowKey + "'"
	entries = table_service.query_entities('photos', filter)

	for entry in entries:
		tag_table_entry(rowKey, entry)

def run():
	messages = queue_service.get_messages('tagger')

	for message in messages:
		rowKey = message.message_text
		print "New image to process RowKey:" + rowKey
		tag_table_entries(rowKey)
		queue_service.delete_message('tagger', message.message_id, message.pop_receipt)

while True:
	run()
	time.sleep(3)

