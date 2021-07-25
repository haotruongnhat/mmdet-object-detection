from pathlib import Path
import mmcv
import numpy as np

f = Path("/home/full_datasets_split/IMG_2693.jpg")
img = mmcv.imread(str(f))

##IMG_3600
# bboxes = np.array([
#     [32, 575, 890, 621],
#     [905, 606, 890, 621],
#     [1815, 646, 633, 621],
#     [0, 1315, 925, 524],
#     [921, 1349, 787, 518],
#     [1704, 1377, 744, 518],
#     [0, 2017, 450, 518],
#     [457, 2072, 826, 450],
#     [1274, 2051, 749, 429],
#     [2023, 2079, 425, 429],
#     [0, 2633, 419, 453],
#     [698, 2636, 783, 453],
#     [1558, 2719, 866, 429],
# ])

##IMG_3602
# bboxes = np.array([
#     [181, 511, 903, 625],
#     [1252, 523, 1196, 625],
#     [43, 1459, 890, 506],
#     [930, 1450, 697, 506],
#     [1615, 1441, 700, 506],
#     [0, 2059, 819, 433],
#     [820, 2068, 1269, 433],
#     [2097, 1998, 351, 433],
#     [0, 2589, 1157, 462],
#     [1154, 2617, 1157, 438],
# ])


##IMG_3601
# bboxes = np.array([
#     [0, 468, 747, 861],
#     [719, 493, 1191, 915],
#     [1910, 528, 538, 915],
#     [895, 1636, 1116, 749],
#     [0, 1583, 794, 749],
#     [0, 2776, 1126, 488],
#     [1121, 2776, 1327, 488],
# ])

# ##IMG_3603
# bboxes = np.array([
#     [422, 483, 1291, 959],
#     [1861, 547, 587, 851],
#     [214, 1753, 1159, 625],
#     [1362, 1665, 887, 716],
#     [0, 2341, 1129, 667],
#     [1123, 2420, 1325, 566]
# ])

##IMG_2693
bboxes = np.array([
    [276, 552, 2419, 2007],
])

bboxes[:, 2] += bboxes[:, 0]
bboxes[:, 3] += bboxes[:, 1]

patches = mmcv.imcrop(img, bboxes)
# print(patches[2].shape)
for index, patch in enumerate(patches):
    mmcv.imwrite(patch, "out/" + f.stem + "_" + str(index) +".jpg")

##########################
# files = Path("/home/coco-annotator/datasets/counting-steel/new").glob("*.JPG")

# out_folder = Path("/home/coco-annotator/datasets/counting-steel/new2")

# for f in files:
#     img = mmcv.imread(str(f))
#     bboxes = np.array([0, 0, 1000, 700])
#     patch = mmcv.imcrop(img, bboxes)

#     mmcv.imwrite(img,str(out_folder / (f.stem + ".jpg")))

    