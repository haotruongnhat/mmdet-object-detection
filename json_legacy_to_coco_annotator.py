import json

with open("/home/vstech-data/COCO/lab/lab_coco.json") as f:
  data = json.load(f)

annotations_len = len(data["annotations"])

def bbox_to_segmentation(bbox):
    return [bbox[0],            bbox[1], 
            bbox[0] + bbox[2],  bbox[1],
            bbox[0] + bbox[2],  bbox[1] +  bbox[3],
            bbox[0],            bbox[1] +  bbox[3]
            ]

for i in range(annotations_len):
    data["annotations"][i]["segmentation"].append(
            bbox_to_segmentation(data["annotations"][i]["bbox"])
            )

data["categories"][0]["name"] = "round_steel"
with open('/home/vstech-data/COCO/lab/lab_coco.json', 'w') as json_file:
  json.dump(data, json_file)