#!/bin/bash

# Check ceph server for files, and if found, delete them from local.

# <s User modifiable constants
LOCAL_PATH='/mnt/c/ProjectAeon/'
#REMOTE_PATH='/mnt/z/'  # \\ceph-gw01.hpc.swc.ucl.ac.uk\aeon\test2\
REMOTE_PATH='/mnt/c/ProjectAeon/remote/'
EXP_NAME='experiment0'
# hardware devices as top-level folders
DEVICES=('camera_side' 'camera_top' 'microphone' 'harp_patch1' 'harp_patch2' \
         'harp_patch3')
# /s>

# <s Script constants (do not edit!)
LOCAL_EXP_DIR="${LOCAL_PATH}${EXP_NAME}/"
REMOTE_EXP_DIR="${REMOTE_PATH}${EXP_NAME}/"
# /s>

# <s Delete redundant files

# Ensure user-defined directories exist.
test ! -d ${LOCAL_EXP_DIR} \
&& echo "Local exp dir ${LOCAL_EXP_DIR} DOES NOT exist." && exit 1
test ! -d ${REMOTE_EXP_DIR} \
&& echo "Remote exp dir ${REMOTE_EXP_DIR} DOES NOT exist." && exit 1

# Remove files, device-by-device.
for device in "${DEVICES[@]}"; do
	#files_local=($(find "${LOCAL_EXP_DIR}${device}/" -type f -printf "%f\n"))
	files_local=($(find "${LOCAL_EXP_DIR}${device}/" -type f))
	files_remote=($(find "${REMOTE_EXP_DIR}${device}/" -type f))
	# todo: could try to use associative arrays (or something else) instead of
	# nested loop
	for fl in "${files_local[@]}"; do
		match_found=0
		for fr in "${files_remote[@]}"; do
			if [ $(basename ${fl}) == $(basename ${fr}) ]; then 
				rm ${fl}
				match_found=1
				break
			fi
		done
		if [ ! match_found ]; then echo "${fl} not found on remote server"; fi
	done
done
# /s>
