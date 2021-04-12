#!/bin/bash

# <h
# Check ceph server for files, and if found, delete them from local.
#
# Usage: `rm_redundant_local_files <local_path> <remote_path> <exp_name> \
#         <devices>` 
# /h>

# Read input args
local_path=$1
remote_path=$2
exp_name=$3
devices=($4)

# Set paths
local_exp_dir="${local_path}${exp_name}/"
remote_exp_dir="${remote_path}${exp_name}/"

# <s Delete redundant files

# Ensure user-defined directories exist.
test ! -d ${local_exp_dir} \
&& echo "Local exp dir ${local_exp_dir} DOES NOT exist." && exit 1
test ! -d ${remote_exp_dir} \
&& echo "Remote exp dir ${remote_exp_dir} DOES NOT exist." && exit 1

# Remove files, device-by-device.
for device in "${devices[@]}"; do
	#files_local=($(find "${local_exp_dir}${device}/" -type f -printf "%f\n"))
	files_local=($(find "${local_exp_dir}${device}/" -type f))
	files_remote=($(find "${remote_exp_dir}${device}/" -type f))
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
