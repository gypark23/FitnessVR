import argparse
from glob import glob
from scipy.signal import find_peaks
import data_analysis as DA
import numpy as np
from sklearn import tree 
from sklearn.model_selection import train_test_split # Import train_test_split function
from sklearn import metrics #Import scikit-learn metrics module for accuracy calculation
import pandas as pd
import os
from collections import defaultdict
import feature_list
"""
Create a non-deep learning classifier (e.g. multiclass SVM, decision tree, random forest)
to perform activity detection that improves upon your prior algorithm.

Usage:
    
    python3 Python/predict_shallow.py <sensor .csv sample>

    python3 Python/predict_shallow.py --label_folder <folder with sensor .csv samples>
"""
# Creates a DF given a directory
# Columns of means, variances, and peaks on 36 attributes
def createDF(directory = "Data/Lab2/Train/") -> pd.DataFrame:
    data = []
    columns = []
    for filename in os.listdir(directory):
        df = DA.summarize_sensor_trace2(directory + filename)
        elem = []
        for _, row in df.iterrows():
            elem.extend(row.tolist())
        elem.append(filename[:3])
        data.append(elem)

    columns = [lab + cat for lab in feature_list.features for cat in ("_mean", "_var", "_peak")]
    columns.append("category")
    df = pd.DataFrame(data, columns = columns)
    return df

# Creates a DF given a file path (not directory)
# the same manner as above
def createDF2(file:str) -> pd.DataFrame:
    data = []
    columns = []

    df = DA.summarize_sensor_trace2(file)
    elem = []
    for _, row in df.iterrows():
        elem.extend(row.tolist())
    elem.append("NULL")
    data.append(elem)

    columns = [lab + cat for lab in feature_list.features for cat in ("_mean", "_var", "_peak")]
    columns.append("category")
    df = pd.DataFrame(data, columns = columns)
    return df


# returns a Decision Tree Model
# using features selected from classify.py 
# feel free to modify features -Kyu
def learn() -> tree.DecisionTreeClassifier:
    train_df = createDF()
    columns = train_df.columns
    
    # features from classify.py; I included all mean, variance, peaks for these -Kyu
    features_cols = [columns[num * 3 + alpha] for alpha in (0,1,2) for num in (6,7,8,15,21,33,27,19,30)]
    
    X = train_df[features_cols]
    y = train_df.category
    clf = tree.DecisionTreeClassifier()
    clf = clf.fit(X, y)

    # valid_df = createDF("Data/Lab2/Validation/")
    # x_valid, y_valid = valid_df[features_cols], valid_df.category
    # y_pred = clf.predict(x_valid)
    # print("Accuracy:",metrics.accuracy_score(y_valid, y_pred))

    return clf

# Global variable so that you don't spend much time
# reconstructing model every time it is called
clf = learn()

def predict_shallow(sensor_data: str) -> str:
    """Run prediction on an sensor data sample.

    Replace the return value of this function with the output activity label
    of your shallow classifier for the given sample. Feel free to load any files and write
    helper functions as needed.
    """
    # create a DF 
    df = createDF2(sensor_data)
    columns = df.columns
    # extract significant features
    features_cols = [columns[num * 3 + alpha] for alpha in (0,1,2) for num in (6,7,8,15,21,33,27,19,30)]
    
    ret = clf.predict(df[features_cols])[0]
    print(ret)
    return ret


def predict_shallow_folder(data_folder: str, output: str):
    """Run the model's prediction on all the sensor data in data_folder, writing labels
    in sequence to an output text file."""

    data_files = sorted(glob(f"{data_folder}/*.csv"))
    labels = map(predict_shallow, data_files)

    with open(output, "w+") as output_file:
        output_file.write("\n".join(labels))


if __name__ == "__main__":
    # Parse arguments to determine whether to predict on a file or a folder
    # You should not need to modify the below starter code, but feel free to
    # add more arguments for debug functions as needed.
    parser = argparse.ArgumentParser()
    sample_input = parser.add_mutually_exclusive_group(required=True)
    sample_input.add_argument(
        "sample", nargs="?", help="A .csv sensor data file to run predictions on"
    )
    sample_input.add_argument(
        "--label_folder",
        type=str,
        required=False,
        help="Folder of .csv data files to run predictions on",
    )

    parser.add_argument(
        "--output",
        type=str,
        default="Data/Lab2/Labels/shallow.txt",
        help="Output filename of labels when running predictions on a directory",
    )

    args = parser.parse_args()

    if args.sample:
        print(predict_shallow(args.sample))

    elif args.label_folder:
        predict_shallow_folder(args.label_folder, args.output)