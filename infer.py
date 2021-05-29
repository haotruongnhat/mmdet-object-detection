from mmdet.apis import init_detector, inference_detector
import mmcv

# Specify the path to model config and checkpoint file
config_file = '/home/vstech-data/COCO/lab/checkpoints/latest.pth/demthep_lab_faster_rcnn.py'
checkpoint_file = '/home/vstech-data/COCO/lab/checkpoints/latest.pth/latest.pth'

# build the model from a config file and a checkpoint file
model = init_detector(config_file, checkpoint_file, device='cuda:0')

# test a single image and show the results
img = '/home/vstech-data/COCO/lab2/images/156.jpg'  # or img = mmcv.imread(img), which will only load it once
result = inference_detector(model, img)
# visualize the results in a new window
# model.show_result(img, result)
# or save the visualization results to image files
model.show_result(img, result, font_size=5, out_file='result.jpg')