source source_path.sh

CONFIG_FILE=/home/vstech-data/COCO/lab/demthep_lab_faster_rcnn.py
CHECKPOINT_FILE=/home/vstech-data/COCO/lab/checkpoints/latest.pth/latest.pth
OUTPUT_FILE=/home/vstech-data/COCO/lab/output/model.onnx
INPUT_IMAGE_PATH=/home/vstech-data/COCO/lab/images/1.jpg
IMAGE_SHAPE="640 800"

python -m tools.deployment.pytorch2onnx \
    ${CONFIG_FILE} \
    ${CHECKPOINT_FILE} \
    --output-file ${OUTPUT_FILE} \
    --input-img ${INPUT_IMAGE_PATH} \
    --shape ${IMAGE_SHAPE}
    # --simplify
    # --verify
    # --dynamic-export \
    # --verify \

python /home/onnxruntime/tools/python/remove_initializer_from_input.py \
    --input ${OUTPUT_FILE} \
    --output ${OUTPUT_FILE}

