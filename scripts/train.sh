source scripts/source_path.sh

today=`date +"%y-%m-%d_%H-%M"`

# output_dir=/home/lab/counting-steel-5/checkpoints/$today
# config=/home/lab/demthep_lab_faster_rcnn_counting_steel.py
# mkdir -p $output_dir
# python -m tools.train $config --work-dir $output_dir --gpus 1 # --resume-from /home/vstech-data/COCO/lab/checkpoints/latest.pth/epoch_8.pth

output_dir=/home/lab/cvat_counting_steel_3/checkpoints/$today
config=/home/lab/demthep_lab_faster_rcnn_counting_steel.py
mkdir -p $output_dir
python -m tools.train $config --work-dir $output_dir --gpus 1 # --resume-from /home/vstech-data/COCO/lab/checkpoints/latest.pth/epoch_8.pth