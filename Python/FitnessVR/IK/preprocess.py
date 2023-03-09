import os

import matplotlib.pyplot as plt
import numpy as np

import pandas as pd
import glob

import warnings
warnings.filterwarnings("ignore")



features = [
	"controller_left_pos.x",
	"controller_left_pos.y",
	"controller_left_pos.z",
	"controller_left_rot.x",
	"controller_left_rot.y",
	"controller_left_rot.z",

	"controller_right_pos.x",
	"controller_right_pos.y",
	"controller_right_pos.z",
	"controller_right_rot.x",
	"controller_right_rot.y",
	"controller_right_rot.z",
]



# ============================== RAW DATA PROCESSING =================================
# extract useful features from the raw data
def load_poses(csv_file, sample_rate=5, show=False):
	df = pd.read_csv(csv_file, usecols = features)


	# sample with a given sample rate
	sampled_cols = []
	for (columnName, columnData) in df.items():
		sampled_col = columnData[::sample_rate]
		sampled_cols.append(sampled_col)

	# load components of each pose
	lx = sampled_cols[0].to_list()
	ly = sampled_cols[1].to_list()
	lz = sampled_cols[2].to_list()
	la = sampled_cols[3].to_list()
	lb = sampled_cols[4].to_list()
	lg = sampled_cols[5].to_list()
	
	rx = sampled_cols[6].to_list()
	ry = sampled_cols[7].to_list()
	rz = sampled_cols[8].to_list()
	ra = sampled_cols[9].to_list()
	rb = sampled_cols[10].to_list()
	rg = sampled_cols[11].to_list()

	# convert from degree to rad
	la = [np.deg2rad(x) for x in la]
	lb = [np.deg2rad(x) for x in lb]
	lg = [np.deg2rad(x) for x in lg]
	ra = [np.deg2rad(x) for x in ra]
	rb = [np.deg2rad(x) for x in rb]
	rg = [np.deg2rad(x) for x in rg]
	
	# convert to poses
	time = len(lx)
	l_pos = [[lx[i], ly[i], lz[i], la[i], lb[i], lg[i]] for i in range(time)]
	r_pos = [[rx[i], ry[i], rz[i], ra[i], rb[i], rg[i]] for i in range(time)]

	if show:
		print("Left Hand: ")
		for pos in l_pos:
			print("    ", pos)
		print("Right Hand: ")
		for pos in r_pos:
			print("    ", pos)

	return l_pos, r_pos



# ============================== MAIN =================================
#dir = "../../../Data/FitnessVR/Cleaned/SIT_P1_02.csv"
#l_pos, r_pos = load_poses(dir,show=True)


