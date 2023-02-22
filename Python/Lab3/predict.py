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
import matplotlib.pyplot as plt
from sklearn.tree import plot_tree
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
f1 = f1_score(new_labels, pred_labels)

end_time = time.time()


print(f"\nTraining time: {train_time - start_time} seconds")
print(f"Testing time: {end_time - train_time} seconds","\n")

print('Random Forest accuracy:', accuracy)
print("\nF1 Score:", f1)
print("Confusion Matrix:\n", conf_matrix, "\n")


# Export

features =  [
   "head_norms",
	"head_osc", 
	"head_angv_x", 
	"head_angv_y",
	"head_angv_z", 

	"left_norms", 
	"left_angv_x",
	"left_angv_y", 
	"left_angv_z",

	"right_norms", 
	"right_angv_x", 
	"right_angv_y",
	"right_angv_z",
]

attr = [f + "_"+e for f in features for e in ["Mean", "Var", "Quantile", "Entropy"]]

plt.figure(figsize=(20,20))
plot_tree(rf.estimators_[0], filled=True, rounded=True, feature_names = attr, class_names = ["Male", "Female"])
plt.savefig("../../Reports/random_forest.png")










