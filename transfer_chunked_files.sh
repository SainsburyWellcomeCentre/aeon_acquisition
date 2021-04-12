#!/bin/bash

# <h
# Given an experiment name and devices, create folders on acquisition computer
# and mounted ceph server. While experiment is running, check for new files; if 
# found, copy over from acquisition computer to ceph server.
#
# Usage: `transfer_chunked_files <local_path> <remote_path> <exp_name> \
#         <devices>`
# /h>

# Read input args.
local_path=$1
remote_path=$2
exp_name=$3
devices=($4)

# Define script constants and params.
# time of files in seconds
CHUNK_TIME=$(( 60 * 60 * 3 ))
# flag to indicate experiment is running
exp_on=1

# Set paths.
local_exp_dir="${local_path}${exp_name}/"
remote_exp_dir="${remote_path}${exp_name}/"

# <s Make directories on acquisition computer and ceph server

# Ensure user-defined directories exist
test ! -d ${local_path} \
&& echo "User-specified local path ${local_path} DOES NOT exist." && exit 1
test ! -d ${remote_path} \
&& echo "User-specified remote path ${remote_path} DOES NOT exist." && exit 1

# make dirs
mkdir ${remote_exp_dir}
cd ${remote_exp_dir}
mkdir ${devices[*]}
mkdir ${local_exp_dir}
cd ${local_exp_dir}
mkdir ${devices[*]}

#for device in ${devices[@]}; do
    #mkdir("${local_exp_dir}/${device}/")
    #mkdir("${remote_exp_dir}/${device}/")
#done
# /s>

# <s Check for and copy written out files
while [[ exp_on ]]; do
	for device in "${devices[@]}"; do
		cur_time_s=$(date +"%s")  # seconds since 1970-01-01
		end_time_s=$(( ${cur_time_s} - ${CHUNK_TIME} ))
		end_time_fmt=$(date -d @${end_time_s} +"%c")  # formatted for `find`
		#start_time=$(( ${end_time}-${CHUNK_TIME} ))
		#files = find -newermt "${start_time}" ! -newermt "${end_time}"
		files=($(find ${device} ! -newermt "${end_time_fmt}" -type f))
		# Ensure files found before attempting to copy.
		if (( ${#files[@]} )); then
			cp -n ${files[*]} "${remote_exp_dir}${device}/"
		fi
	done
	# Exit when user quits Bonsai or stops workflow
	#if [ bonsai_exits ]; exp_on=0; fi
done
# /s>
