import pandas as pd
import os
import glob
import matplotlib.pyplot as plt

def summarize_sensor_trace(csv_file: str):
    df = pd.read_csv(csv_file)
    vals = []
    names = []
    for (columnName, columnData) in df.iteritems():
        if columnName in ('time', 'Unnamed: 37'):
            continue
        
        names.append(columnName)
        vals.append([columnData.mean(), columnData.var()])
        # print("Mean of " + columnName + ": " + str(columnData.mean()))
        # print("Variance of " + columnName + ": " + str(columnData.var()))
    
    ret = pd.DataFrame(vals, columns = ['Mean', 'Variance'], index = names)
    print(ret)
    return ret
    
    pass

def visualize_sensor_trace(csv_file: str, attribute: str):
    df = pd.read_csv(csv_file)
    ax = df.plot(x = 'time', y = attribute)
    ax.set_xlabel("Time (milliseconds)")
    ax.set_ylabel(attribute)
    plt.show()
    pass
            
        


#summarize_sensor_trace("../Data/Lab1/JOG_P1_01.csv")
#visualize_sensor_trace("../Data/Lab1/JOG_P1_01.csv", 'controller_left_vel.x')
# Define additional functions as needed here!

def combine_samples(activity: str):
    # get all csv files
    path = "../Data/Lab1/"
    csv_files = glob.glob(os.path.join(path, "*.csv"))
    df_list = []
    # combine data for activity of interest into one df
    for file in csv_files:
        df = pd.read_csv(file)

        name = file.split("\\")[-1].split('_')[0].rsplit('/', 1)[-1]
        if name == activity:
            df_list.append(df)
    activity_df = pd.concat(df_list)
    
    vals = []
    names = []
    for (columnName, columnData) in activity_df.iteritems():
        if columnName in ('time', 'Unnamed: 37'):
            continue
        
        names.append(columnName)
        vals.append([columnData.mean(), columnData.var()])

    ret = pd.DataFrame(vals, columns = ['Mean', 'Variance'], index = names)
    print(ret)
    return ret

combine_samples("JOG")