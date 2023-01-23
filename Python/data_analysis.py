import pandas as pd
import os
import glob
import matplotlib.pyplot as plt
import numpy as np

def summarize_sensor_trace(csv_file: str):
    df = pd.read_csv(csv_file)
    #list of lists of means and variances
    vals = []
    #list of column (attribute) names
    names = []
    for (columnName, columnData) in df.iteritems():
        #exclude average of time or unnamed column
        if columnName in ('time', 'Unnamed: 37'):
            continue
        
        names.append(columnName)
        vals.append([columnData.mean(), columnData.var()])
        # print("Mean of " + columnName + ": " + str(columnData.mean()))
        # print("Variance of " + columnName + ": " + str(columnData.var()))
    
    ret = pd.DataFrame(vals, columns = ['Mean', 'Variance'], index = names)
    #print(ret)
    #returns a dataframe of means and variances of each attribute
    return ret
    
    pass

# prints a plot of time vs any given attribute if single_attribute_mode
# compares multiple attributes and activities if single_attribute_mode is False
def visualize_sensor_trace(csv_file: str = "", attribute: str = "", single_attribute_mode : bool = True):
    #if the function is required to plot a time vs single attribute
    if single_attribute_mode:
        df = pd.read_csv(csv_file)
        ax = df.plot(x = 'time', y = attribute)
        ax.set_xlabel("Time (milliseconds)")
        ax.set_ylabel(attribute)
        plt.show()
    else:
        Standing = combine_samples("STD")
        Sitting = combine_samples("SIT")
        Jogging = combine_samples("JOG")
        Stretching = combine_samples("STR")
        Overhead = combine_samples("OHD")
        Twisting = combine_samples("TWS")

        dataframes = [Standing, Sitting, Jogging, Stretching, Overhead, Twisting]

        # Output graphs for variance of different activities
        compare_attributes(dataframes, 0, 3, "Mean")
        compare_attributes(dataframes, 3, 6, "Mean")
        compare_attributes(dataframes, 6, 9, "Mean")
        compare_attributes(dataframes, 9, 12, "Mean")
        compare_attributes(dataframes, 12, 15, "Mean")
        compare_attributes(dataframes, 15, 18, "Mean")
        compare_attributes(dataframes, 18, 21, "Mean")
        compare_attributes(dataframes, 21, 24, "Mean")
        compare_attributes(dataframes, 24, 27, "Mean")
        compare_attributes(dataframes, 27, 30, "Mean")
        compare_attributes(dataframes, 30, 33, "Mean")
        compare_attributes(dataframes, 33, 36, "Mean")

        # Output graphs for variance of different activities
        compare_attributes(dataframes, 0, 3, "Variance")
        compare_attributes(dataframes, 3, 6, "Variance")
        compare_attributes(dataframes, 6, 9, "Variance")
        compare_attributes(dataframes, 9, 12, "Variance")
        compare_attributes(dataframes, 12, 15, "Variance")
        compare_attributes(dataframes, 15, 18, "Variance")
        compare_attributes(dataframes, 18, 21, "Variance")
        compare_attributes(dataframes, 21, 24, "Variance")
        compare_attributes(dataframes, 24, 27, "Variance")
        compare_attributes(dataframes, 27, 30, "Variance")
        compare_attributes(dataframes, 30, 33, "Variance")
        compare_attributes(dataframes, 33, 36, "Variance")
                
    pass
            
        
#demonstration of summarize_sensor_trace and visualize_sensor_trace

summarize_sensor_trace("../Data/Lab1/JOG_P1_01.csv")
visualize_sensor_trace("../Data/Lab1/JOG_P1_01.csv", 'controller_left_vel.x')


# Define additional functions as needed here!


# returns a dataframe given the activity that has average mean and average variance
# for each attribute
def combine_samples(activity: str):
    # get all csv files
    path = "../Data/Lab1/"
    csv_files = glob.glob(os.path.join(path, "*.csv"))
    df_list = []

    for file in csv_files:
        # df = pd.read_csv(file)

        name = file.split("\\")[-1].split('_')[0].rsplit('/', 1)[-1]
        #for the activity csv
        if name == activity:
            #calculate average and variance for each attribute
            df = summarize_sensor_trace(file)          
            df_list.append(df)
    #combine samples and calculate mean of means and mean of variances
    activity_df = pd.concat(df_list).groupby(level=0, sort = False).mean()
    
    print("------ Summary of " + activity + "-------")
    print(activity_df)

    return activity_df
    # for (columnName, columnData) in activity_df.iteritems():
    #     if columnName in ('time', 'Unnamed: 37'):
    #         continue
        
    #     names.append(columnName)
    #     vals.append([columnData.mean(), columnData.var()])

    # ret = pd.DataFrame(vals, columns = ['Mean', 'Variance'], index = names)
    # print(ret)
    # return ret

# STD	Standing	VideoLinks to an external site.
# SIT	Sitting	VideoLinks to an external site.
# JOG	Jogging	VideoLinks to an external site.
# STR	Arms stretching	VideoLinks to an external site.
# OHD	Arms overhead	VideoLinks to an external site.
# TWS	Twisting

def compare_attributes(dataframes, start, end, stat):

    # Get the number of indexes in a dataframe
    num_indexes = end - start
    # Create a list of colors for the bars
    colors = ['b', 'g', 'r', 'c', 'm', 'y']

    activities = ["Standing", "Sitting", "Jogging", "Stretching", "Overhead", "Twisting"]

    # Set width of each bar
    barWidth = 0.4/num_indexes

    # Set position of bar on X axis
    r = np.arange(num_indexes)

    # Create a loop to iterate through each dataframe
    for i in range(len(dataframes)):
        # Initialize an empty list to store the mean values of the current dataframe
        means = []
        # Iterate through each index
        for j in range(start, end):
            # Extract the mean values of the current index in the dataframe
            means.append(dataframes[i].iloc[j][stat])
        # Make the plot
        plt.bar(r + i*barWidth, means, color=colors[i%6], width=barWidth, edgecolor='white', label=activities[i])

    # Add labels to the x-axis
    plt.xticks(r + len(dataframes)*barWidth/2, dataframes[0].index[start:end])

    # Add a y-axis label
    plt.ylabel(stat)

    # Add a legend
    plt.legend()

    # Display the bar chart
    plt.show()


# use visualize_sensor_trace again, but this time print a summary of comparison of multiple attributes and activities
visualize_sensor_trace(single_attribute_mode = False)