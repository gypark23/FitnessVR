import os
import time
import warnings
warnings.filterwarnings("ignore")

import pandas as pd
import numpy as np
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from preprocess import create_dataset
from sklearn.metrics import accuracy_score
from sklearn.metrics import confusion_matrix
from sklearn.metrics import f1_score

dir_1 = "../../Data/Lab3/Train/"
dir_2 = "../../Data/Lab3/Test/"


start_time = time.time()

rf = RandomForestClassifier(n_estimators=100)

# Train the model on the entire dataset
training_set, labels = create_dataset(dir_1)

rf.fit(training_set, labels)
train_time = time.time()

# Testing
testing_set, new_labels = create_dataset(dir_2)
pred_labels = rf.predict(testing_set)

accuracy = accuracy_score(new_labels, pred_labels)
conf_matrix = confusion_matrix(new_labels, pred_labels)





end_time = time.time()

print(f"Training time: {train_time - start_time} seconds")
print(f"Testing time: {end_time - train_time} seconds","\n")

print('Random Forest accuracy:', accuracy)
print("F1 Score:", f1_score(new_labels, pred_labels))
print(conf_matrix)














