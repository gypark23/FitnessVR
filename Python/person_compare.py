# This file provides functions to compare personal differences when doing the same activities
# It outputs a series of plots of the comparison results 


import pandas as pd
import os
import glob
import matplotlib.pyplot as plt
import numpy as np
import data_analysis as DA
from feature_list import features


dir = "../Data/Lab1_divided/"


# combine the samples from the same person
def combine_personal_samples(activity, path):
    # get all csv files
    path = dir + path
    csv_files = glob.glob(os.path.join(path, "*.csv"))
    df_list = []

    for file in csv_files:

        name = file.split("\\")[-1].split('_')[0].rsplit('/', 1)[-1]
        
        # for the activity csv
        if name == activity:
            df = DA.summarize_sensor_trace(file)          
            df_list.append(df)

    # combine samples and calculate mean of means and mean of variances
    activity_df = pd.concat(df_list).groupby(level=0, sort = False).mean()

    return activity_df


# compare two persons by difference in percentage
def compare_activity(activity_df1, activity_df2, plot, title):
    mean_array_1 = activity_df1['Mean'].to_numpy()
    var_array_1 = activity_df1['Variance'].to_numpy()

    mean_array_2 = activity_df2['Mean'].to_numpy()
    var_array_2 = activity_df2['Variance'].to_numpy()

    mean_diff = (mean_array_1 - mean_array_2) / mean_array_2
    var_diff = (var_array_1 - var_array_2) / var_array_2

    ind = np.arange(36) 
    width = 0.35

    if plot:
        plt.bar(ind, mean_diff, width, label='Mean')
        plt.bar(ind + width, var_diff, width, label='Var')

        plt.ylabel('Difference in percentage')
        plt.title(title)

        plt.xticks(ind + width / 2, features, rotation=90)
        plt.legend(loc='best')
        plt.show()

    return mean_diff, var_diff




# ============================ testing ==============================
# loading averaged data from all users
standard_jog = DA.combine_samples("JOG")
standard_str = DA.combine_samples("STR")
standard_ohd = DA.combine_samples("OHD")
standard_tws = DA.combine_samples("TWS")
standard_std = DA.combine_samples("STD")

# P2 vs P3 for overhead
p2_ohd = combine_personal_samples("OHD", "P2")
p3_ohd = combine_personal_samples("OHD", "P3")
mean_diff, var_diff = compare_activity(p2_ohd, p3_ohd, True, "p2_ohd vs p3_ohd")

# P1 vs P3 for standing
p1_std = combine_personal_samples("STD", "P1")
p3_std = combine_personal_samples("STD", "P3")
mean_diff, var_diff = compare_activity(p1_std, p3_std, True, "p1_std vs p3_std")

# P2 vs standard for stretching
p2_str = combine_personal_samples("STR", "P2")
mean_diff, var_diff = compare_activity(p2_str, standard_str, True, "p2_str vs standard_str")

# P3 vs standard for twisting
p3_tws = combine_personal_samples("TWS", "P3")
mean_diff, var_diff = compare_activity(p3_tws, standard_tws, True, "p3_tws vs standard_tws")
