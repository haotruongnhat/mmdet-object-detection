import json

with open("/home/lab/counting-steel-7.json") as f:
  data = json.load(f)

annotations_len = len(data["annotations"])

for i in range(annotations_len):
  data["annotations"][i]["segmentation"] = []

with open('/home/lab/counting-steel-7.json', 'w') as json_file:
  json.dump(data, json_file)