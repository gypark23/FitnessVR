import argparse
from glob import glob
import time
from scipy.signal import find_peaks
import data_analysis as DA
import numpy as np
from sklearn import tree 
from sklearn.model_selection import train_test_split 
from sklearn.metrics import confusion_matrix 
from sklearn import metrics 
import pandas as pd
import os
from collections import defaultdict
import feature_list
import pickle
"""
Create a non-deep learning classifier (e.g. multiclass SVM, decision tree, random forest)
to perform activity detection that improves upon your prior algorithm.

Usage:
    
    python3 Python/predict_shallow.py <sensor .csv sample>

    python3 Python/predict_shallow.py --label_folder <folder with sensor .csv samples>
"""
start = time.time()
# Creates a DF given a directory
# Columns of means, variances, and peaks on 36 attributes
def createDF(directory = "Data/FitnessVR/Train/") -> pd.DataFrame:
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
def learn() -> tree.DecisionTreeClassifier:
    train_df = createDF()
    columns = train_df.columns
    
    # features from classify.py; I included all mean, variance, peaks for these -Kyu
    features_cols = [columns[num * 3 + alpha] for alpha in (0,1,2) for num in (6,7,8,15,21,33,27,19,30)]
    
    X = train_df[features_cols]
    y = train_df.category
    clf = tree.DecisionTreeClassifier()
    clf = clf.fit(X, y)

    return clf

# Global variable so that you don't spend much time
# reconstructing model every time it is called
clf = learn()

# Save the trained model to a file in the .pkl format
with open("model.pkl", "wb") as f:
    pickle.dump(clf, f)