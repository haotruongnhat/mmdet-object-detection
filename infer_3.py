from mmdet.apis import init_detector, inference_detector
import mmcv
import funcy
import os
from pathlib import Path
from tqdm import tqdm
import numpy as np
import json

# Specify the path to model config and checkpoint file
config_file = '/home/lab/demthep_lab_faster_rcnn_counting_steel.py'
checkpoint_folder = Path('/home/lab/cvat_counting_steel_3/checkpoints/21-07-25_08-03')

# unlabelled_images = [image.stem for image in list(Path("/home/lab/cvat_counting_steel_1/test").glob("*.jpg"))]
checkpoint_file = str(checkpoint_folder / "latest.pth")

# build the model from a config file and a checkpoint file
model = init_detector(config_file, checkpoint_file, device='cuda:0')

dataset_folder = Path("/home/full_datasets_split")

image_files = list(dataset_folder.glob("**/*.jpg"))
image_files = funcy.lremove(lambda img: ".thumbnail" in str(img), image_files)

output_folder = checkpoint_folder / "output"
os.makedirs(output_folder, exist_ok = True)

output_json_file = output_folder / "out.json"
output_dict = dict(
    annotations=[],
    categories=[{"id": 1, "name": "round_steel"}],
    images=[]
)

def bbox_to_segmentation(bbox):
    return [bbox[0],            bbox[1], 
            bbox[0] + bbox[2],  bbox[1],
            bbox[0] + bbox[2],  bbox[1] +  bbox[3],
            bbox[0],            bbox[1] +  bbox[3]
            ]

bbox_id = 1
for index, image_file in enumerate(tqdm(image_files)):
    temp_output_folder = str(image_file.parents[0]).replace(str(dataset_folder), str(output_folder))
    os.makedirs(temp_output_folder, exist_ok = True)

    img = mmcv.imread(image_file)
    result = inference_detector(model, img)
    model.show_result(img, result, font_size=10, out_file=str(Path(temp_output_folder) /  image_file.name))

    shape = img.shape

    ## Output json
    output_dict["images"].append(
        dict(
            file_name = image_file.name,
            height = shape[0],
            width = shape[1],
            id = index + 1
        )
    )

    bboxes = result[0]
    bboxes[:, 2] = bboxes[:, 2] - bboxes[:, 0]
    bboxes[:, 3] = bboxes[:, 3] - bboxes[:, 1]
    bboxes = bboxes[:, :-1].astype(int)

    # for bbox in result:
    for bbox in bboxes:
        bbox = [int(b) for b in bbox.tolist()]
        output_dict["annotations"].append(
            dict(
                id = bbox_id,
                bbox = bbox,
                segmentation = [],
                image_id = index + 1,
                category_id = 1,
                iscrowd = 0
            )
        )
        bbox_id += 1

with open(str(output_json_file), 'w') as outfile:
    json.dump(output_dict, outfile)

    