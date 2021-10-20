import os
import cv2
import glob
import numpy as np
import matplotlib.pyplot as plt
import argparse

parser = argparse.ArgumentParser(description="Camera checkerboard calibration. Assumes a checkerboard with 8x12 inner corners.")
parser.add_argument('dname', type=str, help="The path to a folder with checkerboard images to use for camera calibration.")
args = parser.parse_args()

patternsize = (8, 12)
pattern = np.zeros((1, patternsize[0] * patternsize[1], 3), np.float32)
pattern[0,:,:2] = np.mgrid[0:patternsize[0], 0:patternsize[1]].T.reshape(-1, 2)
criteria = (cv2.TERM_CRITERIA_EPS + cv2.TERM_CRITERIA_MAX_ITER, 30, 0.001)

objectPoints = []
imagePoints = []
fnames = glob.glob(args.dname + '/*.png')
for fname in fnames:
    print("Processing {0}...".format(os.path.split(fname)[-1]), end=" ")
    img = cv2.imread(fname)
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    
    flags = cv2.CALIB_CB_ADAPTIVE_THRESH + cv2.CALIB_CB_FAST_CHECK + cv2.CALIB_CB_NORMALIZE_IMAGE
    ret, corners = cv2.findChessboardCorners(gray, patternsize, flags)
    print("Success" if ret else "Not found!")
    
    if ret:
        corners = cv2.cornerSubPix(gray, corners, (11, 11), (-1, -1), criteria)
        objectPoints.append(pattern)
        imagePoints.append(corners)
        
        img = cv2.drawChessboardCorners(img, patternsize, corners, ret)
    
    plt.imshow(img)
    plt.draw()
    plt.waitforbuttonpress(timeout=0.1)
    
print("Calibrating camera with {0} points from {1} images...".format(
    np.prod(np.array(imagePoints).shape) // 2,
    len(imagePoints)))
ret, intrinsics, distortion, rvecs, tvecs = cv2.calibrateCamera(
        objectPoints, imagePoints, gray.shape[::-1], None, None)
print("Reprojection error: {0}".format(ret))

fname = "{0}.yml".format(os.path.split(args.dname)[-1])
print("Writing calibration file {0}...".format(fname))
flags = cv2.FILE_STORAGE_FORMAT_YAML + cv2.FILE_STORAGE_WRITE
f = cv2.FileStorage(args.dname + '/' + fname,flags)
f.write('image_width', img.shape[1])
f.write('image_height', img.shape[0])
f.write('camera_matrix', intrinsics)
f.write('distortion_coefficients', distortion)
f.write('reprojection_error', ret)
f.release()

for i,fname in enumerate(fnames):
    print("Undistorting {0}...".format(os.path.split(fname)[-1]))
    img = cv2.imread(fname)
    img = cv2.drawChessboardCorners(img, patternsize, imagePoints[i], True)
    
    uimg = cv2.undistort(img, intrinsics, distortion)
    plt.imshow(uimg)
    plt.draw()
    plt.waitforbuttonpress(timeout=0.1)