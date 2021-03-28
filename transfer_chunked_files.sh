# Given an experiment name and devices, create folders on acquisition computer
# and mounted ceph server. While experiment is running, check for new files; if 
# found, copy over from acquisition computer to ceph server.

# <s User modifiable constants
LOCAL_PATH='/mnt/c/ProjectAeon/'
REMOTE_PATH='/mnt/z/'  # \\ceph-gw01.hpc.swc.ucl.ac.uk\aeon\test2\
EXP_NAME='experiment0'
DEVICES=('camera_side' 'camera_top' 'microphone' 'harp_patch1' 'harp_patch2' \
	     'harp_patch3')
# time of files in seconds
CHUNK_TIME=3600
# /s>

# <s Script constants (do not edit!)
# flag to indicate experiment is running
EXP_ON=1
LOCAL_EXP_DIR="${LOCAL_PATH}${EXP_NAME}/"
REMOTE_EXP_DIR="${REMOTE_PATH}${EXP_NAME}/"
# /s>

# <s Make directories on acquisition computer and ceph server

# Ensure user-defined directories exist
test ! -d ${LOCAL_PATH} \
&& echo "User-specified local path ${LOCAL_PATH} DOES NOT exist." && exit 1
test ! -d ${REMOTE_PATH} \
&& echo "User-specified remote path ${REMOTE_PATH} DOES NOT exist." && exit 1

# make dirs
cd ${REMOTE_EXP_DIR}
mkdir ${DEVICES[*]}
cd ${LOCAL_EXP_DIR}
mkdir ${DEVICES[*]}

# for device in ${DEVICES[@]}; do
# 		mkdir("${LOCAL_EXP_DIR}/${device}/")
# 		mkdir("${REMOTE_EXP_DIR}/${device}/")
# done
# /s>

# <s Check for and copy written out files
while [[ EXP_ON ]]; do
	for device in ${DEVICES[@]}; do
		cur_time_s=$(date +"%s")  # seconds since 1970-01-01
		end_time_s=$(( ${cur_time} - ${CHUNK_TIME} ))
		end_time_fmt=$(date -d @${end_time_s} +"%c")  # formatted for `find`
		# start_time=$(( ${end_time}-${CHUNK_TIME} ))
		files=$(find ./${device} ! -newermt "${end_time_fmt}")
		# files = find -newermt "${start_time}" ! -newermt "${end_times}"
		cp -n ${files[*]} "${REMOTE_EXP_DIR}/${device}/"
	
	# Exit when user quits Bonsai or stops workflow
	# if [ bonsai_exits ]; EXP_ON=0; fi
done
# /s>
