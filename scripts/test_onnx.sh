source source_path.sh

CONFIG_FILE=/home/vstech-data/COCO/lab/demthep_lab_faster_rcnn.py
ONNX_FILE=/home/vstech-data/COCO/lab/checkpoints/latest.onnx
OUTPUT_FILE=/home/vstech-data/COCO/lab/checkpoints/result.pkl
FORMAT_ONLY=False
EVALUATION_METRICS=bbox
SHOW_DIRECTORY=/home/vstech-data/COCO/lab/checkpoints/show_dir

python -m tools.deployment.test \
    ${CONFIG_FILE} \
    ${ONNX_FILE} \
    --out ${OUTPUT_FILE} \
    --eval ${EVALUATION_METRICS} \
    --show-dir ${SHOW_DIRECTORY} \
