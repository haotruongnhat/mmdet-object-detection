import json
import funcy
with open("/home/lab/cvat_counting_steel_2.json") as f:
  data = json.load(f)


with open('/home/lab/cvat_counting_steel_2_steel_bundle.json', 'w') as json_file:
  json.dump(data, json_file)

# images_len = len(data["images"])
# image_by_id = {}
# for i in range(images_len):
#   image_by_id[str(data["images"][i]["id"])] = data["images"][i]["file_name"]

# categories_len = len(data["categories"])
# label_by_id = {}
# for i in range(categories_len):
#   label_by_id[str(data["categories"][i]["id"])] = data["categories"][i]["name"]

# take_class = "steel_bundle"
# annotations_data = data["annotations"]
# annotations_len = len(data["annotations"])
# filtered_annotations_data = []

# funcy.lremove(lambda i: i['id'] not in images_with_annotations, images)
# for i in range(annotations_len):
#   if(annotations_data[i][])


# with open('/home/mmdet-object-detection/IMG-3600_1.json', 'w') as json_file:
#   json.dump(data, json_file)