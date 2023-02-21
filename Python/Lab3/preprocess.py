import os

import matplotlib.pyplot as plt
import numpy as np

import pandas as pd
import glob

from features import features

import warnings
warnings.filterwarnings("ignore")



# ============================== RAW DATA PROCESSING =================================
# find the difference between every 2 adjacent elements in a list
def calc_diffs(arr):
	arr = list(arr)
	diffs = np.array([arr[i+1] - arr[i] for i in range(len(arr)-1)])
	return diffs


# calculate the distance in 3D between two points
def calc_diff_norms(x, y, z):
	x_diff = calc_diffs(x)
	y_diff = calc_diffs(y)
	z_diff = calc_diffs(z)
	norms = np.sqrt(x_diff**2 + y_diff**2 + z_diff**2)
	return list(norms)


# extract useful features from the raw data
def extract_features(csv_file: str, show=False):
	df = pd.read_csv(csv_file, usecols = features)

	sampled_cols = []
	for (columnName, columnData) in df.items():
		# sample once every 3 timestamps
		sampled_col = columnData[::3]
		sampled_cols.append(sampled_col)

	# extract norm of differences on head and controller pos
	head_norms = calc_diff_norms(sampled_cols[0], sampled_cols[1], sampled_cols[2])
	left_norms = calc_diff_norms(sampled_cols[6], sampled_cols[7], sampled_cols[8])
	right_norms = calc_diff_norms(sampled_cols[12], sampled_cols[13], sampled_cols[14])

	# extract head vertical oscillation
	head_osc =  calc_diffs(sampled_cols[2])

	# store in a new dataframe
	extracted_df = pd.DataFrame()
	extracted_df["head_norms"] = head_norms
	extracted_df["head_osc"] = head_osc
	extracted_df["head_angv_x"] = sampled_cols[3]
	extracted_df["head_angv_y"] = sampled_cols[4]
	extracted_df["head_angv_z"] = sampled_cols[5]

	extracted_df["left_norms"] = left_norms
	extracted_df["left_angv_x"] = sampled_cols[9]
	extracted_df["left_angv_y"] = sampled_cols[10]
	extracted_df["left_angv_z"] = sampled_cols[11]

	extracted_df["right_norms"] = right_norms
	extracted_df["right_angv_x"] = sampled_cols[15]
	extracted_df["right_angv_y"] = sampled_cols[16]
	extracted_df["right_angv_z"] = sampled_cols[17]

	extracted_df = extracted_df.dropna()
	if show:
		print(ret)

	return extracted_df







# ============================== STATISTICAL ANALYSIS OF EXTRACTED FEATURES =================================
# calculate the mean of top 25% data
def calc_quantile(data):
	data = list(data)
	quantile_index = int(len(data) * 0.75)
	sorted_data = np.sort(data)[::-1]
	quantile_mean = np.mean(sorted_data[:quantile_index])
	return quantile_mean


# calculate the entrop of the data
def calc_entropy(data):
    values, counts = np.unique(data, return_counts=True)
    freqs = counts.astype('float') / len(data)
    entropy = -np.sum(freqs * np.log2(freqs))
    return entropy


# extract mean, var, quantile, entropy of the featrues
def calc_feature_stats(df, show=False):
	names = []
	vals = []
	for (columnName, columnData) in df.items():
		# calculate the four features
		mean = columnData.mean()
		var = columnData.var()
		quantile = calc_quantile(columnData)
		entropy = calc_entropy(columnData)

		# store the featues
		names.append(columnName)
		vals.append([mean, var, quantile, entropy])

	stats = pd.DataFrame(vals, columns = ["Mean", "Var", "Quantile", "Entropy"], index = names)
	if show:
		print(stats)
	
	return stats


def load_sample(dir):
	df = extract_features(dir)
	stats_df = calc_feature_stats(df)
	return stats_df





# ============================== TRAINING and VALIDATION DATASET CREATION =================================

# create dataset from a directory
# convert feature df into an 1d array as the data vector
# read the first letter of the csv file as the label. M for male, F for female 
def create_dataset(dir):
	dataset = []
	labels = []

	for filename in os.listdir(dir):
		try:
			df = load_sample(dir + filename)
			if df.isna().any().any():
				continue
			label = filename[0]
			df_matrix = df.to_numpy()
			df_flat = df_matrix.flatten()
			dataset.append(df_flat)
			labels.append(label)
		except:
			continue

	return dataset, labels
	


# ============================== MAIN =================================

# dir = "../../Data/Lab2/Train/JOG_010.csv"
# df = extract_features(dir)
# calc_feature_stats(df, True)

