import json
import argparse
import funcy
from sklearn.model_selection import train_test_split
import shutil, os
from pathlib import Path

parser = argparse.ArgumentParser(description='Splits COCO annotations file into training and test sets.')
parser.add_argument('annotations', metavar='coco_annotations', type=str,
                    help='Path to COCO annotations file.')
parser.add_argument('full_datasets', type=str,
                    help='Path to full image datasets.')
parser.add_argument('output_folder', type=str,
                    help='Path to image folder')
# parser.add_argument('train', type=str, help='Where to store COCO training annotations')
# parser.add_argument('test', type=str, help='Where to store COCO test annotations')
parser.add_argument('-s', dest='split', type=float, required=True,
                    help="A percentage of a split; a number in (0, 1)")
parser.add_argument('--having-annotations', dest='having_annotations', action='store_true',
                    help='Ignore all images without annotations. Keep only these with at least one annotation')
parser.add_argument("-exclude", required=False, 
                                nargs='+',
                                help="Exclude image from annotations")

args = parser.parse_args()

def save_coco(file, images, annotations, categories):
    with open(file, 'wt', encoding='UTF-8') as coco:
        json.dump({ 'images': images, 
            'annotations': annotations, 'categories': categories}, coco, indent=2, sort_keys=True)

def filter_annotations(annotations, images):
    image_ids = funcy.lmap(lambda i: int(i['id']), images)
    return funcy.lfilter(lambda a: int(a['image_id']) in image_ids, annotations)

def is_in_filter(excludes, image_name):
    for ex in excludes:
        if ex in image_name:
            return True

    return False

def main(args):
    full_datasets_path = Path(args.full_datasets)
    all_images = list(full_datasets_path.glob("**/*.jpg"))
    all_images = funcy.lremove(lambda img: ".thumbnail" in str(img), all_images)

    with open(args.annotations, 'rt', encoding='UTF-8') as annotations:
        coco = json.load(annotations)
        images = coco['images']
        annotations = coco['annotations']
        categories = coco['categories']

        images_with_annotations = funcy.lmap(lambda a: int(a['image_id']), annotations)

        if args.having_annotations:
            images = funcy.lremove(lambda i: i['id'] not in images_with_annotations, images)

        if args.exclude:
            exclude_image_id = funcy.lmap(lambda a: int(a['id']) if is_in_filter(args.exclude, a['file_name']) else -1, images)
            exclude_image_id = funcy.lremove(lambda i: i == -1, exclude_image_id)

            ### Remove images
            images = funcy.lremove(lambda i: int(i["id"]) in exclude_image_id, images)
            annotations = funcy.lremove(lambda a: int(a["image_id"]) in exclude_image_id, annotations)

        x, y = train_test_split(images, train_size=args.split)

        os.makedirs(args.output_folder, exist_ok = True)
        ### Move image folder
        output_train_images_folder = "{}/{}".format(args.output_folder, "train")
        output_test_images_folder = "{}/{}".format(args.output_folder, "test")
        output_unlabelled_images_folder = "{}/{}".format(args.output_folder, "unlabelled")

        os.makedirs(output_train_images_folder, exist_ok = True)
        os.makedirs(output_test_images_folder, exist_ok = True)
        os.makedirs(output_unlabelled_images_folder, exist_ok = True)

        unlabelled_images = all_images

        for image in x:
            unlabelled_images = funcy.lremove(lambda i: i.stem in image["file_name"], unlabelled_images)
            shutil.copy(str(full_datasets_path / image["file_name"]), output_train_images_folder)
        for image in y:
            unlabelled_images = funcy.lremove(lambda i: i.stem in image["file_name"], unlabelled_images)
            shutil.copy(str(full_datasets_path / image["file_name"]), output_test_images_folder)

        for unlabelled_image in unlabelled_images:
            shutil.copy(str(unlabelled_image), output_unlabelled_images_folder)

        train = "{}/{}.json".format(args.output_folder, "train")
        test = "{}/{}.json".format(args.output_folder, "test")
        save_coco(train, x, filter_annotations(annotations, x), categories)
        save_coco(test, y, filter_annotations(annotations, y), categories)

        print("Saved {} entries in {} and {} in {}".format(len(x), train, len(y), test))

if __name__ == "__main__":
    main(args)