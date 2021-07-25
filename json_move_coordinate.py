import json

with open("/home/mmdet-object-detection/IMG-3600.json") as f:
  data = json.load(f)

annotations_len = len(data["annotations"])

def bbox_to_segmentation(bbox):
  return [bbox[0],            bbox[1], 
          bbox[0] + bbox[2],  bbox[1],
          bbox[0] + bbox[2],  bbox[1] +  bbox[3],
          bbox[0],            bbox[1] +  bbox[3]
          ]

new_coor = [905, 606]
new_id = 1073

def move_bbox(bbox):
  return [
    bbox[0] - new_coor[0],
    bbox[1] - new_coor[1],
    bbox[2], bbox[3]
  ]

for i in range(annotations_len):
  data["annotations"][i]["bbox"] = move_bbox(data["annotations"][i]["bbox"])
  data["annotations"][i]["image_id"] = new_id

for i in range(annotations_len):
  data["annotations"][i]["segmentation"] = \
        [bbox_to_segmentation(data["annotations"][i]["bbox"])]

data["images"][0]["id"] = new_id
data["images"][0]["width"] = 891
data["images"][0]["height"] = 622
data["images"][0]["file_name"] = "IMG-3600_1.jpg"
data["images"][0]["path"] = "/datasets/counting-steel/new/IMG-3600_1.jpg"

with open('/home/mmdet-object-detection/IMG-3600_1.json', 'w') as json_file:
  json.dump(data, json_file)